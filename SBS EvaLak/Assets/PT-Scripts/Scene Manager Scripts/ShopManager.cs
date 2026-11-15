using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ShopManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] Inventory inventory;
    public List<string> fullDeckList; //all possible cards
    public List<string> deckList; //this is what player owns

    public List<bool> checkLockList; //bool for if its locked
    public List<string> unlockedDeckList; //strings of unlocked cards

    public List<string> lockedCards; //list of full deck minus the unlocked cards
    public List<string> duplicateCards;
    public int totalScrap;
    public bool DebugOn = true; //later add debug in pause menu to access this and trigger this true/false

    [Header("Touch Controls")]
    public int swipeX; //-1 = left, 0 default, 1 = right 
    public int swipeY; //-1 = down, 0 default, 1 = up
    [SerializeField] private TouchManager touchManager;

    [Header("Music")]
    public AudioSource audio;
    public AudioMixerGroup musicMixerGroup;

    [Header("Start Screen")]
    [SerializeField] private bool finishedTyping = false;
    [SerializeField] private GameObject bootText;
    [SerializeField] private TypeText typetext;
    bool startScreen = true; //turns false when unloaded, is start screen on

    [Header("Loading Screen")]
    [SerializeField] private bool finishedTyping2 = false;
    [SerializeField] private GameObject loadingText;
    [SerializeField] private TypeText typetext2;
    bool loadingScreen = false; 

    [Header("On Screen")]
    [SerializeField] private GameObject quitButton;
    [SerializeField] private GameObject modeButton;
    [SerializeField] private GameObject debugButton;

    [SerializeField] bool doOnce = false;
    [SerializeField] bool waitDone = false;

    [SerializeField] private int page = 0; //this is the page number, for the scroll, each time you scroll it increases by CardSlot.Count (12)
    [SerializeField] private List<Sprite> CardSprites; //UI sprites to update card with sprite

    [SerializeField] private Sprite tempsprite;
    [SerializeField] private Sprite backsprite;
    [SerializeField] private Color tempcolor;

    public int scrapValueText;
    [SerializeField] private TextMeshProUGUI svText;

    [Header("Shop Screen")]
    [SerializeField] private bool ShopActive = false; //if shop
    [SerializeField] private GameObject onScreen;
    [SerializeField] private GameObject shopScreen;

    [SerializeField] private List<GameObject> ShopSlots; //UI game object where cards go
    [SerializeField] private GameObject GridParentShop; //parent gameobject that holds grid of card slots in shop

    [Header("Scrap Screen")]
    [SerializeField] private bool ScrapActive = false; //if load screen
    [SerializeField] private GameObject scrapScreen;
    
    [SerializeField] private List<GameObject> ScrapSlots;
    [SerializeField] private GameObject GridParentScrap;
    
    public int duplicatesText;
    [SerializeField] private TextMeshProUGUI dText;
    
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

        if (touchManager.clickHeld && startScreen) { typetext = null; finishedTyping = true; } //skip cutscene if you press
        if (typetext != null) { finishedTyping = typetext.Finish; }

        if (touchManager.clickHeld && loadingScreen) { typetext2 = null; finishedTyping2 = true; } //skip cutscene if you press
        if (typetext2 != null) { finishedTyping2 = typetext2.Finish; }

        if (finishedTyping) { Cutscene(); }

        UpdateCardData();
        DisplayCards();
        ScrollCards();
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

        if (loadingText == null) { loadingText = GameObject.Find("ScreenConsoleText"); }
        if (typetext2 == null) { loadingText.GetComponent<TypeText>(); }

        if (quitButton == null) { quitButton = GameObject.Find("ReturnToHangar"); }
        if (modeButton == null) { modeButton = GameObject.Find("Shop/Scrap"); }

        if (shopScreen == null) { shopScreen = GameObject.Find("Shop"); }
        if (scrapScreen == null) { scrapScreen = GameObject.Find("Scrap"); }
        if (onScreen == null) { shopScreen = GameObject.Find("OnScreen"); }

        if (inventory == null)
        {
            GameObject temp = GameObject.Find("DDOL");
            inventory = temp.GetComponent<Inventory>();
        }

        if (touchManager == null) { touchManager = GameObject.Find("TouchManager").GetComponent<TouchManager>(); }

        if (inventory != null)
        {
            UpdateCardData();
        }

        if (GridParentShop == null) { GridParentShop = GameObject.Find("CardLayoutGrid"); }
        if (GridParentScrap == null) { GridParentScrap = GameObject.Find("CardLayoutGrid (1)"); }

        //Set up cards
        UpdateCardUI();
    }

    public void UpdateCardData()
    {
        fullDeckList = inventory.fullDeckList;
        deckList = inventory.deckList;

        unlockedDeckList = inventory.unlockedDeckList;
        checkLockList = inventory.checkLockList;

        duplicateCards = inventory.duplicateList;
        lockedCards = fullDeckList.Except(unlockedDeckList).ToList();

        totalScrap = inventory.totalScrap;
    }

    public void UpdateCardUI()
    {
        GetCards();
        DisplayCards();
    }

    void GetCards() //get all card slots from ui grid, put them in list
    {
        GameObject temp;
        foreach (Transform child in GridParentShop.transform) //get children buttons from the grid
        {
            temp = child.gameObject;
            ShopSlots.Add(temp);
        }

        foreach (GameObject CardSlot in ShopSlots) //set them all active ...
        {
            CardSlot.SetActive(true);
        }

        foreach (Transform child in GridParentScrap.transform) //get children buttons from the grid
        {
            temp = child.gameObject;
            ScrapSlots.Add(temp);
        }

        foreach (GameObject CardSlot in ScrapSlots) //set them all active ...
        {
            CardSlot.SetActive(true);
        }
    }

    void DisplayCards()
    {
        UpdateTextUI();
        //SHOP
        string cardName; //text string of card
        TextMeshProUGUI cardNameText; //tmp of card

        Transform tempTransform;
        Image tempimage;
        GameObject temp;

        for (int i = 0; i < ShopSlots.Count; i++) //for each card unlocked, set it to game object and set the card slot to active
        {
            if ((i + page) >= lockedCards.Count) { cardName = "Card Name"; } //if out of range
            else //if in range
            {
                cardName = lockedCards[i + page]; //set the text on the card to be the card name
            }

            //if (deckList.Contains(cardName)) { cardName = "Card Name"; } //if card is in deck, show blank card

            //SET SPRITE
            tempTransform = ShopSlots[i].GetComponent<Transform>(); //grab transform of card slot
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

        //SCRAP
        cardName = ""; //text string of card
        cardNameText = null; //tmp of card

        tempTransform = null;
        tempimage = null;
        temp = null;

        for (int i = 0; i < ScrapSlots.Count; i++) //for each card unlocked, set it to game object and set the card slot to active
        {
            if ((i + page) >= duplicateCards.Count) { cardName = "Card Name"; } //if out of range
            else //if in range
            {
                cardName = duplicateCards[i + page]; //set the text on the card to be the card name
            }

            //if (deckList.Contains(cardName)) { cardName = "Card Name"; } //if card is in deck, show blank card

            //SET SPRITE
            tempTransform = ScrapSlots[i].GetComponent<Transform>(); //grab transform of card slot
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
            if (touchManager.clickHeld) { waitDone = true; }
            //UnloadTextFunction(bootText);
            StartCoroutine(Wait(0.5f));
            if (waitDone)
            {
                bootText.SetActive(false);

                loadingScreen = true;
                waitDone = false;
            }
        }

        if (loadingScreen)
        {
            startScreen = false;
            loadingText.SetActive(true);
            if (touchManager.clickHeld) { waitDone = true; finishedTyping2 = true; }
            //UnloadTextFunction(bootText);
            StartCoroutine(Wait(0.5f));
            if (finishedTyping2)
            {
                loadingText.SetActive(false);

                ShopActive = true;
                waitDone = false;
            }
        }

        if (ShopActive)
        {
            ScrapActive = false;
            startScreen = false;
            loadingScreen = false;

            onScreen.SetActive(true);
            quitButton.SetActive(true);

            DisplayCards();
        }

        if (ScrapActive)
        {
            ShopActive = false;
            startScreen = false;
            loadingScreen = false;

            onScreen.SetActive(true);
            quitButton.SetActive(true);

            DisplayCards();
        }

        scrapScreen.SetActive(ScrapActive);
        shopScreen.SetActive(ShopActive);
        if (ScrapActive || ShopActive) { debugButton.SetActive(DebugOn); }
    }

    public IEnumerator Wait(float delay)
    {
        yield return new WaitForSeconds(delay);
        waitDone = true;
    }

    public void ScrollCards()
    {
        if (ShopActive) 
        { 
            if (swipeY == 1 && (page <= lockedCards.Count)) //if swipe down and the page number is not more than possible cards
            {
                page = page + ShopSlots.Count;
                if (page > lockedCards.Count) { float pageNumber = (float)lockedCards.Count / page; page = (Mathf.CeilToInt(pageNumber)) * ShopSlots.Count; }
                DisplayCards();
            }

            if (swipeY == -1 && (page >= 0)) //if swipe up and the page number is not less than 0
            {
                page = page - ShopSlots.Count;
                if (page < 0) { page = 0; }
                DisplayCards();
            }
        }

        if(ScrapActive)
        {
            if (swipeY == 1 && (page <= duplicateCards.Count)) //if swipe down and the page number is not more than possible cards
            {
                page = page + ScrapSlots.Count;
                if (page > duplicateCards.Count) { float pageNumber = (float)duplicateCards.Count / page; page = (Mathf.CeilToInt(pageNumber)) * ScrapSlots.Count; }
                DisplayCards();
            }

            if (swipeY == -1 && (page >= 0)) //if swipe up and the page number is not less than 0
            {
                page = page - ScrapSlots.Count;
                if (page < 0) { page = 0; }
                DisplayCards();
            }
        }
    }

    public void UpdateTextUI()
    {
        if(svText != null)
        {
            svText.text = $"Total Scrap: {totalScrap}";
        }

        if(dText != null)
        {
            dText.text = $"Duplicate Cards: {duplicatesText}";
        }
    }

    public void ScrapShopMode()
    {
        ShopActive = !ShopActive;
        ScrapActive = !ScrapActive;

        TextMeshProUGUI modeText;
        GameObject modeTextChild;
        string mode;
        if(ShopActive) { mode = "Shop"; }
        else if (ScrapActive) { mode = "Sell"; }
        else { mode = "Error"; }

        modeTextChild = modeButton.transform.GetChild(0).gameObject; //grab the child of card slot
        modeText = modeTextChild.GetComponent<TextMeshProUGUI>();
        modeText.text = mode;
    }
}

