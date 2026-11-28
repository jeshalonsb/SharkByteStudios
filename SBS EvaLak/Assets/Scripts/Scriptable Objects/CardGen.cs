using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using Unity.VectorGraphics;
using System.Text.RegularExpressions;

public enum CardType { Strike, Combo, Buff, Heal, PersHeal, Flamethrower, Shield, Nothing}
public class CardGen : MonoBehaviour
{
    [Header("Enums")]
    public CardType cardType;

    [Header("Ints")]
    public int cardNumValue; //how much damage the card does, how much you can heal, percent buffs etc.
    public int cardNumCost; //how much energy does the card cost
    public int min;
    public int max;

    [Header("Strings")]
    public string cardName;
    public string cardDescription;

    [Header("Bools")]
    public bool targetPlayer = false;
    public bool damageRange = false;

    [Header("Object References")]
    public Image cardImage;
    public SpriteRenderer spriteRender;
    public Sprite cardSprite;

    public GameObject selectionBorder;
    public BoxCollider2D collider;

    public PersistentHealStatus healstatusPrefab;
    public DamageOverTimeStatus damagestatusPrefab;

    public TextMeshProUGUI tmp;
    public string tmpText;

    [Header("Card Dictionary")]
    public CardDictionary dictionary;

    public List<CardType> cardTypeList; //list of card types
    public List<Sprite> cardSpriteList;

    public List<string> cardNameList; //names of cards
    public List<string> cardDescList; //description of cards

    public List<int> cardValueList; //card value of effect
    public List<int> cardCostList; //card cost to play

    public List<AudioClip> soundList;
    public AudioClip playSound;

    public List<string> componentCardList; //list of component cards
    public List<Sprite> componentSpriteList; 

    [Header("Spawned Cards")]
    public List<GameObject> spawnedCardList;
    [SerializeField] private GameObject temp;
    [SerializeField] private bool canDrag;

    [SerializeField] private GameObject templateCard;
    [SerializeField] private GameObject deckZone;

    [SerializeField] private bool doneGen = false;
    [SerializeField] private bool action;
    [SerializeField] private bool component;
    [SerializeField] private bool needScripts;

    void Start()
    {
        if(dictionary == null) //initialize
        {
            dictionary = GameObject.Find("DDOL").GetComponent<CardDictionary>();
            
            cardNameList = dictionary.cardNameList;
            cardDescList = dictionary.cardDescList;

            cardCostList = dictionary.cardCostList;
            cardValueList = dictionary.cardValueList;

            soundList = dictionary.soundList;
            cardSpriteList = dictionary.cardSpriteList;

            componentCardList = dictionary.componentCardList;
            componentSpriteList = dictionary.componentSpriteList;
        }
        if(deckZone == null) { deckZone = GameObject.Find("DeckZone"); }
        SetUpCardTypeList();
        if (action) { SpawnActionCards(); }
        if (component) { SpawnComponentCards(); }
    }

    public void SetUpCardTypeList()
    {
        foreach(string name in cardNameList)
        {
            CardType type = GetCardType(name);
            cardTypeList.Add(type);
        }
    }

    public CardType GetCardType(string name)
    {
        if(name.Contains("Combo")) { return CardType.Combo; }
        else if(name.Contains("Damage")) { return CardType.Strike; }
        else if(name.Contains("Persistent")) { return CardType.PersHeal; }
        else if (name.Contains("Repair")) { return CardType.Heal; }
        else if(name.Contains("Flamethrower")) { return CardType.Flamethrower; }
        else if(name.Contains("Buff")) { return CardType.Buff; }
        else if(name.Contains("Shield")) { return CardType.Shield; }
        else { return CardType.Nothing; }  
    }

    public void SpawnActionCards()
    {
        for(int i = 0; i < cardNameList.Count; i++) //if it detects card data, do this for each index of decklist
        {
            temp = Instantiate(templateCard, deckZone.transform); //spawn gameObject
            temp.name = cardNameList[i]; //set name of gameObject

            collider = temp.GetComponent<BoxCollider2D>(); //check for collider
            if (collider == null) { temp.AddComponent<BoxCollider2D>(); } //if no collider, add one

            Card card = temp.AddComponent<Card>(); //add card component
            card.energyCost = cardCostList[i]; //add energy cost
            card.description = cardDescList[i]; //add description

            selectionBorder = temp.transform.GetChild(2).gameObject; //get selection border
            card.selectionBorder = selectionBorder; //set selection border

            cardType = cardTypeList[i]; //get card type

            tmp = temp.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>(); //get text object
            tmpText = GetText(cardType);
            tmp.text = tmpText;

            playSound = GetPlaySound(cardType); //get audio
            card.playSound = playSound;
            card.selectSound = soundList[0];

            if(canDrag) //if card spawner has this checked, this is a scene where cards can be dragged, so cards spawn with card drag
            {
                temp.AddComponent<CardDrag>();
            }

            cardSprite = cardSpriteList[i]; //set up card sprite
            cardImage = temp.GetComponent<Image>();
            if(cardImage == null) { cardImage = temp.AddComponent<Image>(); }
            spriteRender = temp.GetComponent<SpriteRenderer>();
            if (spriteRender == null) { spriteRender = temp.AddComponent<SpriteRenderer>(); }
            AddSprite(temp, cardSprite);

            if (needScripts) { SetUp(cardType, cardValueList[i]); }//set up card data

            temp = null; //reset info before next card in for loop
            collider = null;
            playSound = null;
            cardSprite = null;
            cardImage = null;
            spriteRender = null;
        }

        if (!component) { doneGen = true; }
    }

    public AudioClip GetPlaySound(CardType cardType)
    {
        switch (cardType)
            { 
            case CardType.Strike:
                return soundList[1]; //attack basic
            case CardType.Combo:
                return soundList[2]; //attack combo
            case CardType.Heal:
                return soundList[3]; //heal
            case CardType.PersHeal:
                return soundList[3]; //heal
            case CardType.Buff:
                return soundList[4]; //power up
            case CardType.Flamethrower:
                return soundList[1]; //attack basic
            case CardType.Shield:
                return soundList[4]; //power up
            default:
                return null;
            }
    }

    public string GetText(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.Strike:
                return "Basic Strike"; //attack basic
            case CardType.Combo:
                return "Combo Strike"; //attack combo
            case CardType.Heal:
                return "Repair"; //heal
            case CardType.PersHeal:
                return "Persistent Repair"; //heal
            case CardType.Buff:
                return "Mult Buff"; //power up
            case CardType.Flamethrower:
                return "Flamethrower"; //attack basic
            case CardType.Shield:
                return "Shield"; //power up
            default:
                return null;
        }
    }

    public void SetUp(CardType cardType, int cardValue)
    {
        switch (cardType)
        {
            case CardType.Strike:
                SetUpDamage(temp, cardValue);
                break;
            case CardType.Combo:
                SetUpCombo(temp, cardValue);
                break;
            case CardType.Heal:
                SetUpHeal(temp, cardValue);
                break;
            case CardType.PersHeal:
                SetUpPersHeal(temp, cardValue);
                break;
            case CardType.Buff:
                SetUpBuff(temp, cardValue);
                break;
            case CardType.Flamethrower:
                SetUpDamageOverTime(temp, cardValue);
                break;
            case CardType.Shield:
                SetUpShield(temp, cardValue);
                break;
            default:
                break;
        }
    }

    public void SetUpDamage(GameObject temp, int damageAmount)
    {
        Damage damage = temp.AddComponent<Damage>();
        damage.damage = damageAmount;

        damage.targetPlayer = targetPlayer;

        damage.useDamageRange = damageRange;
        damage.minDamage = damageAmount - 10;
        damage.maxDamage = damageAmount + 10;
    }

    public void SetUpCombo(GameObject temp, int damageAmount)
    {
        ComboDamage combo = temp.AddComponent<ComboDamage>();
        combo.damage = damageAmount;

        combo.targetPlayer = targetPlayer;

        combo.useDamageRange = damageRange;
        combo.minDamage = damageAmount - 10;
        combo.maxDamage = damageAmount + 10;
    }

    public void SetUpHeal(GameObject temp, int healAmount)
    {
        Heal heal = temp.AddComponent<Heal>();
        heal.healValue = healAmount;

        heal.targetPlayer = targetPlayer;
    }

    public void SetUpPersHeal(GameObject temp, int persHealAmount)
    {
        PersistentHeal persHeal = temp.AddComponent<PersistentHeal>();
        persHeal.healPerTurnValue = persHealAmount;

        persHeal.statusPrefab = healstatusPrefab;

        persHeal.targetPlayer = targetPlayer;
    }

    public void SetUpBuff(GameObject temp, int buffAmount)
    {
        MultBuff buff = temp.AddComponent<MultBuff>();
        buff.multIncrease = IntPercentage(buffAmount);
    }

    public void SetUpDamageOverTime(GameObject temp, int damageAmount)
    {
        DamageOverTime dot = temp.AddComponent<DamageOverTime>();
        dot.damagePerTurnValue = damageAmount;

        dot.statusPrefab = damagestatusPrefab;

        dot.targetPlayer = targetPlayer;
    }

    public void SetUpShield(GameObject temp, int shieldAmount)
    {
        ShieldEffect shield = temp.AddComponent<ShieldEffect>();

        shield.shieldMult = IntPercentage(shieldAmount);

        shield.targetPlayer = targetPlayer;
    }

    public void AddSprite(GameObject temp, Sprite sprite)
    {
        Image image = temp.GetComponent<Image>();
        image.sprite = sprite;

        SpriteRenderer sr = temp.GetComponent<SpriteRenderer>();
        sr.sprite = sprite;
    }

    float IntPercentage(int value)
    {
        float floatvalue = ((float)value) / 100; //make percentage
        return floatvalue;
    }

    public void SpawnComponentCards()
    {
        for (int i = 0; i < componentCardList.Count; i++)
        {
            temp = Instantiate(templateCard, deckZone.transform); //spawn gameObject
            temp.name = componentCardList[i]; //set name of gameObject

            cardSprite = componentSpriteList[i]; //set up card sprite
            cardImage = temp.GetComponent<Image>();
            if (cardImage == null) { cardImage = temp.AddComponent<Image>(); }
            spriteRender = temp.GetComponent<SpriteRenderer>();
            if (spriteRender == null) { spriteRender = temp.AddComponent<SpriteRenderer>(); }
            AddSprite(temp, cardSprite);

            tmp = temp.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>(); //get text object
            
            string original = temp.name; string updated = original;
            updated = updated.Replace("Head", "Head ");
            updated = updated.Replace("Arms", "Arms ");
            updated = updated.Replace("Legs", "Legs ");

            tmpText = updated;
            tmp.text = tmpText;

            temp = null; //reset info before next card in for loop
            cardSprite = null;
            cardImage = null;
            spriteRender = null;
        }

        doneGen = true;
    }
}
