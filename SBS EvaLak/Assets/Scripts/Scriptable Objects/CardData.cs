using UnityEngine;
using UnityEngine.UI;


public enum CardStyle {Strike, Combo, Buff, Nerf, Heal, PersHeal, Flamethrower, Shield}

//SCRAPPED SECTIONS
public enum StanceStyle {Offensive, Defensive, Neutral}

public enum DamageType { Kinetic, Thermal, Cryo, Warp, Psychic, Biotic, Radiation }


[CreateAssetMenu(fileName = "NewCardData", menuName = "ScriptableObjects")]
public class CardData : ScriptableObject
{
    [Header("Enums")]
    public CardStyle cardStyle;
    //public StanceStyle stanceStyle;
    //public DamageType damageType; //scrapped

    [Header("Ints")] 
    public int cardNumValue; //how much damage the card does, how much you can heal, percent buffs etc.
    public int cardNumCost; //how much energy does the card cost
    public int min;
    public int max;

    [Header("Strings")] 
    public string cardName;
    public string cardDescription;

    [Header("Object References")] 
    public Image cardImage;
    public Sprite cardSprite;
    public GameObject selectionBorder;
    public AudioClip playSound;
    public AudioClip selectSound;

}
