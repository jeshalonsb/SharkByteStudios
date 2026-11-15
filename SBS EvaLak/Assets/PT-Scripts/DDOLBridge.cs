using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DDOLBridge : MonoBehaviour
{
    private GameObject DDOL;
    [SerializeField] private Inventory inventory;

    [SerializeField] private GameObject addRemoveRef;
    [SerializeField] private AddRemoveCardScript addRemoveScript; //script to add/remove cards

    public GameObject myButtonGameObject; //current selected button gameobject
    public Button myButton; //current selected button
    [SerializeField] private string cardType; //name of card
    [SerializeField] TextMeshProUGUI cardTypeText; //tmp on gameobject that stores what card is

    [SerializeField] private int rareCost = 2;
    [SerializeField] private int legendaryCost = 4;

    private string sceneName;

    [SerializeField] DeckBuilderManager deckBuilderManager;
    [SerializeField] ShopManager shopManager;


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
            if (inventory.fullDeckList.Contains(cardType) && !inventory.deckList.Contains(cardType)) { deckBuilderManager.AddCardToDeckUI(cardType, myButtonGameObject); }
            inventory.AddCardByType(cardType);
            return;
        }
        else
        {
            if(inventory.deckList.Contains(cardType)) { deckBuilderManager.RemoveCardFromDeckUI(cardType, myButtonGameObject); }
            inventory.RemoveCardByType(cardType);
            return;
        }
    }

    public void PurchaseCard()
    {
        //inventory = DDOL.GetComponent<Inventory>();

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

        //if contains text that says its legendary or rare, send the legendary or repair cost
        int cost;
        if(cardType.Contains("LDamage") || cardType.Contains("LRepair") || cardType.Contains("LBuff") || cardType.Contains("LCombo")) { cost = legendaryCost; }
        else if (cardType.Contains("RDamage") || cardType.Contains("RRepair") || cardType.Contains("RBuff") || cardType.Contains("RCombo")) { cost = rareCost; }
        else if (cardType.Contains("CDamage") || cardType.Contains("CRepair") || cardType.Contains("CBuff") || cardType.Contains("CCombo")) { cost = 1; }
        else { cost = 0; }

        if (inventory.totalScrap >= cost) { UpdateAndUnlock(cardType); inventory.RemoveScrap(cost); }
    }

    public void ScrapCard()
    {
        //inventory = DDOL.GetComponent<Inventory>();

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

        //if contains text that says its legendary or rare, send the legendary or repair cost
        if (cardType.Contains("LDamage") || cardType.Contains("LRepair") || cardType.Contains("LBuff") || cardType.Contains("LCombo")) { UpdateAndUnlock(cardType); inventory.AddScrap(legendaryCost); }
        if (cardType.Contains("RDamage") || cardType.Contains("RRepair") || cardType.Contains("RBuff") || cardType.Contains("RCombo")) { UpdateAndUnlock(cardType); inventory.AddScrap(rareCost); }
        if (cardType.Contains("CDamage") || cardType.Contains("CRepair") || cardType.Contains("CBuff") || cardType.Contains("CCombo")) { UpdateAndUnlock(cardType); inventory.AddScrap(1); }
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

        if (shopManager == null && (sceneName == "Shop" || sceneName == "PT_Shop"))
        {
            shopManager = GetComponent<ShopManager>();
        }
    }

    private void UpdateAndUnlock(string cardType)
    {
        inventory.unlockedDeckList.Add(cardType);
        inventory.UpdateLockedCards();
        //shopCardsShown.HideUnlockedCards();
    }

    public void ScrapAllCards() //for button to remove all dupe cards at once to turn to scrap
    {
        if (inventory.duplicateList.Count > 0)
        {
            foreach (var card in inventory.duplicateList)
            {
                ScrapSpecificCard(card);
            }
        }
        inventory.duplicateList = new List<string>(); //empty list afterwards
        shopManager.UpdateCardData();
    }

    public void ScrapSpecificCard(string card) //for if we want buttons that individually remove specific cards, or also is called by scrap all cards to remove them all
    {
        int index, scrapValue;
        if (inventory.fullDeckList.Contains(card)) //check to see if string is real card
        {
            index = inventory.fullDeckList.IndexOf(card); //get index of card

            if (cardType.Contains("LDamage") || cardType.Contains("LRepair") || cardType.Contains("LBuff") || cardType.Contains("LCombo")) { scrapValue = legendaryCost; }
            if (cardType.Contains("RDamage") || cardType.Contains("RRepair") || cardType.Contains("RBuff") || cardType.Contains("RCombo")) { scrapValue = rareCost; }
            if (cardType.Contains("CDamage") || cardType.Contains("CRepair") || cardType.Contains("CBuff") || cardType.Contains("CCombo")) { scrapValue = 1; }
            else { scrapValue = 0; }
            inventory.AddScrap(scrapValue);
            shopManager.scrapValueText = inventory.totalScrap;
            shopManager.duplicatesText = inventory.duplicateList.Count;
        }
    }

    public void DebugAdd(int scrap)
    {
        inventory.AddScrap(scrap);
    }
}
