using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class DeckBuilderManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] Inventory inventory;
    public List<string> fullDeckList; //all possible cards
    public List<string> deckList; //this is what player owns

    public List<bool> checkLockList; //bool for if its locked
    public List<string> unlockedDeckList; //strings of unlocked cards

    [Header("Touch Controls")]
    public int swipeX; //-1 = left, 0 default, 1 = right 
    public int swipeY; //-1 = down, 0 default, 1 = up
    private TouchManager touchManager;

    [Header("Music")]
    public AudioSource audio;
    public AudioMixerGroup musicMixerGroup;

    [Header("Start Screen")]
    [SerializeField] private bool finishedTyping = false;
    [SerializeField] private GameObject bootText;
    [SerializeField] private TypeText typetext;
    bool startScreen = true; //turns false when unloaded, is start screen on

    [Header("DB Screen")]
    [SerializeField] private bool DBActive = false; //if full normal db is up (place where we do stuff)
    [SerializeField] private bool dbLoad = false; //if load screen
    [SerializeField] private GameObject loadDBScene;
    [SerializeField] private GameObject dbScreen;
    [SerializeField] private GameObject quitButton;

    [SerializeField] bool doOnce = false;
    [SerializeField] bool waitDone = false;

    [SerializeField] private List<GameObject> CardSlots; //UI game object where cards go
    [SerializeField] private List<Sprite> CardSprites; //UI sprites to update card with sprite
    [SerializeField] private GameObject GridParent; //parent gameobject that holds grid of card slots
    [SerializeField] private Sprite tempsprite;
    [SerializeField] private Sprite backsprite;
    [SerializeField] private Color tempcolor;

    [SerializeField] private GameObject TemplateCard; //what cards copy
    [SerializeField] private GameObject deckArea; //where deck moves to
    [SerializeField] private List<GameObject> PhysicalInventory;
    private float moveSpeed = 10f;
    private float t;
    private float duration = 1f;

    [SerializeField] private int page = 0; //this is the page number, for the scroll, each time you scroll it increases by CardSlot.Count (12)

    public void Awake()
    {
        audio = GetComponent<AudioSource>();

        if (audio != null && musicMixerGroup != null)
        {
            audio.outputAudioMixerGroup = musicMixerGroup;
        }

        Init();
    }

    public void Update()
    {
        swipeX = touchManager.SwipeX();
        swipeY = touchManager.SwipeY();

        t = 1f - Mathf.Exp(-moveSpeed * Time.deltaTime);

        if (touchManager.clickHeld) { typetext = null; finishedTyping = true; } //skip cutscene if you press
        if (typetext != null) { finishedTyping = typetext.Finish; }
        if(finishedTyping) { Cutscene(); }

        DisplayCards();
        ScrollCards(swipeY);
    }


    public void HangarReturn()
    {
        SceneManager.LoadScene("HangarLevel");
    }

    public void MainMenuReturn()
    {
        SceneManager.LoadScene("PT_MainMenu");
    }

    public void Init()
    {
        if (bootText == null) { bootText = GameObject.Find("ScreenBootUpText"); }
        if (typetext == null) { bootText.GetComponent<TypeText>(); }

        if(quitButton == null) { quitButton = GameObject.Find("ReturnToHangar"); }
        if (loadDBScene == null) { loadDBScene = GameObject.Find("LoadDB"); }
        if (dbScreen == null) { dbScreen = GameObject.Find("OnScreen"); }

        if (inventory == null)
        {
            GameObject temp = GameObject.Find("DDOL");
            inventory = temp.GetComponent<Inventory>();
        }

        if(touchManager == null) { touchManager = GameObject.Find("TouchManager").GetComponent<TouchManager>(); }

        if (inventory != null) 
        { 
            fullDeckList = inventory.fullDeckList;
            deckList = inventory.deckList;
            unlockedDeckList = inventory.unlockedDeckList;
            checkLockList = inventory.checkLockList;
        }

        if(GridParent == null) { GridParent = GameObject.Find("CardLayoutGrid"); }
        if(TemplateCard == null) { TemplateCard = GameObject.Find("TemplateCard"); }

        //Set up DB cards
        PhysicalInventory = new List<GameObject>();
        UpdateCardUI();

    }

    public void UpdateCardData()
    {
        fullDeckList = inventory.fullDeckList;
        deckList = inventory.deckList;
        unlockedDeckList = inventory.unlockedDeckList;
        checkLockList = inventory.checkLockList;
    }

    public void UpdateCardUI()
    {
        GetCards();
        DisplayCards();
    }

    void GetCards() //get all card slots from ui grid, put them in list
    {
        GameObject temp;
        foreach (Transform child in GridParent.transform) //get children buttons from the grid
        {
            temp = child.gameObject;
            CardSlots.Add(temp);
        }

        foreach (GameObject CardSlot in CardSlots) //set them all inactive ...
        {
            CardSlot.SetActive(true);
        }
    }

    void DisplayCards()
    {
        int unlockNum = unlockedDeckList.Count; //grab how many cards are unlocked

        string cardName; //text string of card
        TextMeshProUGUI cardNameText; //tmp of card

        Transform tempTransform;
        Image tempimage;
        GameObject temp;

        for (int i = 0; i < CardSlots.Count; i++) //for each card unlocked, set it to game object and set the card slot to active
        {
            if ((i + page) >= unlockedDeckList.Count) { cardName = "Card Name"; } //if out of range
            else //if in range
            {
                cardName = unlockedDeckList[i + page]; //set the text on the card to be the card name
            }

            if (deckList.Contains(cardName)) { cardName = "Card Name"; } //if card is in deck, show blank card

            //SET SPRITE
            tempTransform = CardSlots[i].GetComponent<Transform>(); //grab transform of card slot
            temp = tempTransform.gameObject;
            tempimage = temp.GetComponent<Image>();
            tempsprite = GetSprite(cardName);
            tempimage.sprite = tempsprite;
            tempimage.color = tempcolor;

            //SET NAME
            temp = tempTransform.GetChild(0).gameObject; //grab the child of card slot
            cardNameText = temp.GetComponent<TextMeshProUGUI>();
            cardNameText.text = cardName;
        }
    }

    Sprite GetSprite(string card) //grab the sprite data for the card and update it when rendering the cards
    {
        if (inventory.fullDeckList.Contains(card))
        {
            int index = inventory.fullDeckList.IndexOf(card);
            return CardSprites[index];
        }

        else
        {
            return backsprite;
        }
    }

    public void Cutscene()
    {
        doOnce = true;
        if (startScreen)
        {
            if(touchManager.clickHeld) { waitDone = true; }
            //UnloadTextFunction(bootText);
            StartCoroutine(Wait(0.5f)); 
            if (waitDone)
            {
                bootText.SetActive(false);

                dbLoad = true;
                waitDone = false;
            }
        }

        if (dbLoad) 
        { 
            startScreen = false;

            loadDBScene.SetActive(true);
            //Animation to move from text to sprites
            loadDBScene.SetActive(false);

            DBActive = true; 
        }

        if (DBActive)
        {
            dbLoad = false;
            quitButton.SetActive(true);
            dbScreen.SetActive(true);
            DisplayCards();
        }
    }

    public void UnloadTextFunction(GameObject o) //this brings an error, but id like to be able to make text seem to scroll up out of sight by deleting top line over and over
    {
        TextMeshProUGUI text = o.GetComponent<TextMeshProUGUI>();
        string currentText = text.text;
        int startLength = currentText.Length;

        StartCoroutine(UnloadText(currentText, startLength));
    }

    public IEnumerator UnloadText(string text, int length)
    {
        char[] newLineChars = { '\r', '\n' }; //indicators that line is entered
        int index = 0;

        bool unloaded = false;
        while(!unloaded)
        {
            index = text.IndexOfAny(newLineChars, index); //find index of line break
            if (index > 0 && text.Length > 0) { text = text.Substring(index, text.Length); } //new text is now without the area before first line break

            if (text.Length == 0) { unloaded = true; break; }
            yield return new WaitForSeconds(0.001f);
        }
    }

    public IEnumerator Wait(float delay)
    {
        yield return new WaitForSeconds(delay);
        waitDone = true;
    }

    public void ScrollCards(int swipeY)
    {
        if(swipeY == 1 && (page <= unlockedDeckList.Count)) //if swipe down and the page number is not more than possible cards
        {
            page = page + CardSlots.Count;
            if (page > unlockedDeckList.Count) { float pageNumber = (float)unlockedDeckList.Count / page; page = (Mathf.CeilToInt(pageNumber)) * CardSlots.Count; }
            DisplayCards();
        }

        if(swipeY == -1 && (page >= 0)) //if swipe up and the page number is not less than 0
        {
            page = page - CardSlots.Count;
            if(page < 0) { page = 0; }
            DisplayCards();
        }
    }

    public void AddCardToDeckUI(string card, GameObject button)
    {
        GameObject temp = Instantiate(TemplateCard, deckArea.transform);
        CardLocation(temp, button.transform.position);
        PhysicalInventory.Add(temp);
        //SET SPRITE
        Transform tempTransform = temp.transform; //grab transform of card slot
        Image tempimage = temp.GetComponent<Image>();
        tempsprite = GetSprite(card);
        tempimage.sprite = tempsprite;
        tempimage.color = Color.white;

        //SET NAME
        temp = tempTransform.GetChild(0).gameObject; //grab the child of card slot
        TextMeshProUGUI cardNameText = temp.GetComponent<TextMeshProUGUI>();
        cardNameText.text = card;
        
        DisplayCards();
    }

    public void CardLocation(GameObject temp, Vector3 position)
    {
        temp.transform.position = position;
        StartCoroutine(MoveCard(temp, deckArea));
    }

    public IEnumerator MoveCard(GameObject card, GameObject DeckArea)
    {
        float elapsedTIme = 0f;
        Vector3 startPosition = card.transform.position;
        Vector3 endPosition = DeckArea.transform.position;

        while (elapsedTIme < duration)
        {
            float t = elapsedTIme / duration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            card.transform.position = Vector3.Lerp(startPosition, endPosition, smoothT);

            elapsedTIme += Time.deltaTime;
            yield return null;
        }
    }

    public void RemoveCardFromDeckUI(string card, GameObject button)
    {
        int listIndex = PhysicalInventory.IndexOf(button);
        Destroy(PhysicalInventory[listIndex]);
        PhysicalInventory.RemoveAt(listIndex);
        
    }
}

