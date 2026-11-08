using System.Collections.Generic;
using UnityEngine;

public class RewardCards : MonoBehaviour
{
    public List<string> unlockedDeckList; //bool list of unlocked action cards
    public List<string> fullDeckList; //full list of action cards
    public List<string> duplicateList; //list of duplicate cards
    public Inventory inventory; 
    public GameObject DDOL;

    public List<string> mechDeckList; //list of unlocked component cards
    public List<string> fullmechdeckList; //full list of component cards
    //currently no duplicate list for component cards, no earning dupes for it as we cant scrap component cards yet
    //wait till shop rework to implement that
    [SerializeField] private int chance = 100; //this is the chance of getting component cards, 50 = 50%; 100 = 100%
    private string CardRewardType; //string, either action or component depending on what is being rewarded

    public string cardName;
    public int cardID;
    public bool newreward = false;

    public string cardReward1;
    public string cardReward2;
    public string cardReward3;
    public int rewardNumber = 3;

    public GameObject cardRewardCenter;
    public GameObject cardRewardRight;
    public GameObject cardRewardLeft;

    public GameObject new1; //ui to show if card reward is brand new
    public GameObject new2;
    public GameObject new3; 

    public List<GameObject> cardGameObjectList;
    public GameObject temp;
    public GameObject DeckZone;
    public float SizeUp = 70f;

    private Animator ConfettiAnimator;
    private GameObject confetti;

    void Start()
    {
        Initialize(); //initialize the variables
        PickCard();
        DisplayCards();
    }

    public void Initialize()
    {
        DDOL = GameObject.Find("DDOL");
        inventory = DDOL.GetComponent<Inventory>();
        unlockedDeckList = inventory.unlockedDeckList;
        fullDeckList = inventory.fullDeckList;
        cardGameObjectList = inventory.cardGameObjectList;
        duplicateList = inventory.duplicateList;

        mechDeckList = inventory.mechDeckList;
        fullmechdeckList = inventory.fullmechDeckList;
        //if we want chance to be stored from enemies, chance can be saved in inventory after fighting enemy and then initialized here later

        cardRewardLeft = GameObject.Find("CardReward-Left");
        cardRewardCenter = GameObject.Find("CardReward-Center");
        cardRewardRight = GameObject.Find("CardReward-Right");

        DeckZone = GameObject.Find("DeckZone");

        new1 = GameObject.Find("new1");
        new2 = GameObject.Find("new2");
        new3 = GameObject.Find("new3");
        new1.SetActive(false);
        new2.SetActive(false);
        new3.SetActive(false);

        confetti = GameObject.Find("Confetti");
        ConfettiAnimator = confetti.GetComponent<Animator>();
        confetti.SetActive(false);
    }

    public void PickCard()
    {
        //if (fullDeckList.Count - unlockedDeckList.Count < 3) { rewardNumber = fullDeckList.Count - unlockedDeckList.Count; } //if less then 3 rewards possible, only reward however many is left
        rewardNumber = 3;
        CardRewardType = ""; //reset card reward type

        for (int i = 0; i < rewardNumber; i++) //for loop to make sure it happens 3 times
        {
            if(mechDeckList.Count == fullmechdeckList.Count) { chance = 0; } //if you unlocked all component cards, set chance to earn them to zero since we cant earn dupes of it yet

            int roll = Random.Range(0, 100);
            if(roll > chance) { CardRewardType = "action"; }
            else if(roll <= chance) { CardRewardType = "component"; }
            if(chance == 0) { CardRewardType = "action"; }


            switch(CardRewardType)
            {
                case "action":
                    {
                        cardID = Random.Range(0, fullDeckList.Count); //card index 
                        cardName = fullDeckList[cardID];

                        if (unlockedDeckList.Contains(cardName)) { duplicateList.Add(cardName); }
                        else if (!unlockedDeckList.Contains(cardName))
                        {
                            unlockedDeckList.Add(cardName);
                            newreward = true;
                        }

                        if (i == 0) { cardReward1 = cardName; if (newreward) { new2.SetActive(true); } }
                        else if (i == 1) { cardReward2 = cardName; if (newreward) { new1.SetActive(true); } }
                        else if (i == 2) { cardReward3 = cardName; if (newreward) { new3.SetActive(true); } }
                        else { break; } //if somehow you do more than 3 iterations, break out
                        newreward = false;
                        break;
                    }
                case "component":
                    {
                        cardID = Random.Range(0, fullmechdeckList.Count); //card index 
                        cardName = fullmechdeckList[cardID];

                        if (mechDeckList.Contains(cardName)) 
                        {
                            //place for if there is dupe mech component cards, for now this will just pass
                            i = i - 1; 
                            break;
                        }
                        else if (!unlockedDeckList.Contains(cardName))
                        {
                            mechDeckList.Add(cardName);
                            newreward = true;
                        }

                        if (i == 0) { cardReward1 = cardName; if (newreward) { new2.SetActive(true); } }
                        else if (i == 1) { cardReward2 = cardName; if (newreward) { new1.SetActive(true); } }
                        else if (i == 2) { cardReward3 = cardName; if (newreward) { new3.SetActive(true); } }
                        else { break; } //if somehow you do more than 3 iterations, break out
                        newreward = false;
                        break;
                    }
                default: //if CardRewardType breaks
                    Debug.Log(CardRewardType);
                    break;
            }
            
        }

        inventory.UpdateLockedCards();
    }

    public void DisplayCards()
    {
        foreach (var card in fullDeckList) //action card rewards
        {
            temp = GameObject.Find(card);
            if (card == cardReward1) { temp = Instantiate(temp, cardRewardCenter.transform); CardLocation(temp); }
            if (card == cardReward2) { temp = Instantiate(temp, cardRewardLeft.transform); CardLocation(temp); }
            if (card == cardReward3) { temp = Instantiate(temp, cardRewardRight.transform); CardLocation(temp); }
        }

        foreach (var card in fullmechdeckList) //component card rewards
        {
            temp = GameObject.Find(card);
            if (card == cardReward1) { temp = Instantiate(temp, cardRewardCenter.transform); CardLocation(temp); }
            if (card == cardReward2) { temp = Instantiate(temp, cardRewardLeft.transform); CardLocation(temp); }
            if (card == cardReward3) { temp = Instantiate(temp, cardRewardRight.transform); CardLocation(temp); }
        }

        if (confetti != null) { confetti.SetActive(true); ConfettiAnimator.SetTrigger("Confetti"); }
    }

    public void CardLocation(GameObject temp) 
    {
        temp.transform.localScale = new Vector3(temp.transform.localScale.x * SizeUp, temp.transform.localScale.y * SizeUp, temp.transform.localScale.z * SizeUp);
        temp.transform.localPosition = new Vector3(-3.51f, -2.05f, 7f);
    }
}
