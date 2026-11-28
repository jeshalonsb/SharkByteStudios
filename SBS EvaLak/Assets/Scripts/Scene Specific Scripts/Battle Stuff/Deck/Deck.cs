using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Deck : MonoBehaviour
{
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private PlayModifiers mod;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private CardZone drawPile;
    [SerializeField] private CardZone hand;
    [SerializeField] private CardZone playArea;
    [SerializeField] private CardZone graveyard;
    [SerializeField] private TextMeshPro remainingCardsText;
    [SerializeField] private float playDelay = 1f;
    [SerializeField] private float cardPlayInterval = 0.5f;

    [SerializeField] private float checkCardsInterval = 0.25f;
    [SerializeField] private int numDiscards;

    public GameObject lastPlayedCard;
    public GameObject currentPlayedCard;

    public bool isPlaying;

    private void Start()
    {
        enemyManager = FindAnyObjectByType<EnemyManager>(); // find enemy after it is spawned
    }

    public void Update()
    {
        //if (!isPlaying)
        //{
        //    enemyManager.ResetAnimationSpeed();
        //}
    }

    public void DrawCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // If the draw pile is empty, refill it from the graveyard
            if (drawPile.Count == 0)
            {
                RefillDeck();

                // If there are still no cards, there is nothing left to draw
                if (drawPile.Count == 0)
                    break;
            }

            hand.Add(drawPile.RandomCard());
        }

        remainingCardsText.text = drawPile.Count.ToString();
    }

    public void Discard()
    {
        Debug.Log("Discard");

        if (numDiscards >= 1) //you cant just infinitely discard
        { return; }

        foreach (Card card in hand.GetSelected())
            graveyard.Add(card);

        RefillHand();

        numDiscards++;
    }


    public void Play()
    {
        if (!isPlaying) StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        mod.combo = 0;
        isPlaying = true;
        if (enemyManager == null)
        {
            Debug.Log("NotFound");
            enemyManager = FindAnyObjectByType<EnemyManager>();
        }

        // Selected cards move to the play area, keeping their left to right order
        List<Card> playedCards = hand.GetSelected();
        if (playedCards.Count == 0) yield break;

        List<Card> copyPlayedCards = new List<Card>(playedCards);
        int tmp = GetTotalAttacks(copyPlayedCards);
        enemyManager.SetAnimationSpeed(tmp);

        yield return new WaitForSeconds(checkCardsInterval); //if the add card runs too quickly, the GetTotalAttacks function will break and crash because of zones being moved

        foreach (Card card in playedCards)
            playArea.Add(card);

        // Wait for the cards to arrive in the play area
        yield return new WaitForSeconds(playDelay);

        // Play cards from left to right with delay in between each card, each card is sent to the graveyard after
        foreach (Card card in playedCards)
        {
            lastPlayedCard = currentPlayedCard;
            currentPlayedCard = card.gameObject;
            CheckResetMult();
            card.OnPlay();

            graveyard.Add(card);
            yield return new WaitForSeconds(cardPlayInterval);
        }
        enemyManager.AttackRoutine(); // Actually do the enemy attack turn after the player turn

        RefillHand();

        numDiscards = 0;

        isPlaying = false;

        mod.EndHand(); // tell mod script that the hand has stopped playing

        lastPlayedCard = null;

        enemyManager.ResetAnimationSpeed();
    }

    // Draw cards until the hand size is reached
    private void RefillHand()
    {
        DrawCards(deckManager.handSize - hand.Count);
    }

    private void RefillDeck()
    {
        //refills the deck from the graveyard, then hand size is drawn again
        foreach (Card card in graveyard.GetAllCards())
        {
            drawPile.Add(card);
        }
    }

    private int GetTotalAttacks(List<Card> playedCards)
    {
        int count = 0;

        for (int i = 0; i < playedCards.Count; i++)
        {
            if (playedCards[i] == null) { continue; }

            if (playedCards[i].TryGetComponent(out Damage damage) || playedCards[i].TryGetComponent(out ComboDamage combo))
            {
                count++;
            }
        }

        if (count > 3) { count = 3; }

        //Debug.Log(count);

        return count;
    }

    private void CheckResetMult()
    {
        //if (currentPlayedCard == null) { return; }

        //if (currentPlayedCard.name.Contains("Combo")) //if combo card played
        //{
        //    return;
        //}
        //else if (lastPlayedCard == null && currentPlayedCard.name.Contains("BuffMult")) //if first card played is buffmult
        //{
        //    return;
        //}
        //else if (!lastPlayedCard.name.Contains("BuffMult") && lastPlayedCard != null) //if last card played is not mult
        //{
        //    mod.damageMult = mod.basedamageMult;
        //}

        if (currentPlayedCard == null) { return; }

        if (currentPlayedCard.name.Contains("Combo")) //if combo card played
        {
            return;
        }
        else if (currentPlayedCard.name.Contains("BuffMult")) //if combo card played
        {
            return;
        }
        else if (!lastPlayedCard.name.Contains("BuffMult") && lastPlayedCard != null)
        {
            mod.damageMult = mod.basedamageMult;
        }

    }
}