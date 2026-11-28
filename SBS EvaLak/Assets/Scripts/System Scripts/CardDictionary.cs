using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDictionary : MonoBehaviour 
{
    [Header("Card Strings")]
    public List<string> cardNameList; //names of cards
    public List<string> cardDescList; //description of cards
    public List<string> componentCardList;

    [Header("Card Ints")]
    public List<int> cardValueList; //card value of effect
    public List<int> cardCostList; //card cost to play

    [Header("Card Sounds")]
    public List<AudioClip> soundList;

    [Header("Card Sprites")]
    public List<Sprite> cardSpriteList;
    public List<Sprite> componentSpriteList;

    [Header("Inventory")]
    public Inventory inventory;

    public void Start()
    {
        if (inventory == null) //initialize
        {
            inventory = GetComponent<Inventory>();
            if (cardNameList.Count < inventory.fullDeckList.Count) { cardNameList = inventory.fullDeckList; }
            if(componentCardList.Count < inventory.fullmechDeckList.Count) { componentCardList = inventory.fullmechDeckList; }
        }
    }
}
