using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class DDOLBridge : MonoBehaviour
{
    private GameObject DDOL;
    [SerializeField] private Inventory inventory;

    [SerializeField] private GameObject addRemoveRef;
    [SerializeField] private AddRemoveCardScript addRemoveScript; //script to add/remove cards
    [SerializeField] private ShopCardsShown shopCardsShown; //script to shop cards

    public GameObject myButtonGameObject; //current selected button gameobject
    public Button myButton; //current selected button
    [SerializeField] private string cardType; //name of card
    [SerializeField] TextMeshProUGUI cardTypeText; //tmp on gameobject that stores what card is

    [SerializeField] private int rareCost = 2;
    [SerializeField] private int legendaryCost = 4;

    private string sceneName;

    [SerializeField] DeckBuilderManager deckBuilderManager;


    void Start()
    {
        //Debug.Log("DDOLBRIDGE START");
        Init();
    }

    public void AddRemoveCard()
    {
        Debug.Log("BUTTON CLICKED");

        myButtonGameObject = EventSystem.current.currentSelectedGameObject;
        if (myButtonGameObject == null) { Debug.Log("Unable to find GameObject."); }
        else 
        { 
            myButton = myButtonGameObject.GetComponent<Button>();
            Transform tempTransform = myButtonGameObject.GetComponent<Transform>();
            GameObject temp = tempTransform.GetChild(0).gameObject; //grab the child of card slot

            cardTypeText = temp.GetComponent<TextMeshProUGUI>();
            cardType = cardTypeText.text;
            Debug.Log("Selected " + cardType); 
        }

        if (cardType == "") { Debug.Log("No card type found!"); }

        if (inventory == null)
        {
            Debug.Log("Inventory not found");
            return;
        }
        if (addRemoveScript.isAdd)
        {
            inventory.AddCardByType(cardType);
            deckBuilderManager.AddCardToDeckUI(cardType);
            return;
        }
        else
        {
            inventory.RemoveCardByType(cardType);
            return;
        }
    }

    public void PurchaseCard()
    {
        //inventory = DDOL.GetComponent<Inventory>();

        myButtonGameObject = EventSystem.current.currentSelectedGameObject; 
        if (myButtonGameObject == null) { Debug.Log("Unable to find GameObject."); }
        else { myButton = myButtonGameObject.GetComponent<Button>(); cardTypeText = myButton.GetComponent<TextMeshProUGUI>(); cardType = cardTypeText.text; Debug.Log("Selected " + cardType); }

        if (cardType == "") { Debug.Log("No card type found!"); }

        if (inventory == null)
        {
            Debug.Log("Inventory not found");
            return;
        }
        
        //if contains text that says its legendary or rare, send the legendary or repair cost
        if(cardType.Contains("LDamage") || cardType.Contains("LRepair") || cardType.Contains("LBuff") || cardType.Contains("LCombo")) { UpdateAndUnlock(cardType); inventory.RemoveScrap(legendaryCost); }
        if (cardType.Contains("RDamage") || cardType.Contains("RRepair") || cardType.Contains("RBuff") || cardType.Contains("RCombo")) { UpdateAndUnlock(cardType); inventory.RemoveScrap(rareCost); }
        //if (cardType.Contains("CDamage") || cardType.Contains("CRepair") || cardType.Contains("CBuff") || cardType.Contains("CCombo")) { UpdateAndUnlock(cardType); inventory.RemoveScrap(1); }
    }

    void Init()
    {


        if (DDOL == null)
        {
            DDOL = GameObject.Find("DDOL");
        }


        if (inventory == null)
        {
            inventory = DDOL.GetComponent<Inventory>();
        }

        sceneName = inventory.sceneName;

        if (addRemoveRef == null && (sceneName == "DeckBuildScene" || sceneName == "PT_Deck_Build"))
        {
            addRemoveRef = GameObject.Find("Add/Remove Cards");
            addRemoveScript = addRemoveRef.GetComponent<AddRemoveCardScript>();
            if(deckBuilderManager == null) { deckBuilderManager = GetComponent<DeckBuilderManager>();}
        }

        if (shopCardsShown == null && (sceneName == "Shop" || sceneName == "PT_Shop"))
        {
            shopCardsShown = GetComponent<ShopCardsShown>();
        }
    }

    private void UpdateAndUnlock(string cardType)
    {
        inventory.unlockedDeckList.Add(cardType);
        inventory.UpdateLockedCards();
        shopCardsShown.HideUnlockedCards();
    }
}
