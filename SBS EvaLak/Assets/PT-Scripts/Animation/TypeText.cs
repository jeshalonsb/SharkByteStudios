using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using TMPro.Examples;

public class TypeText : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private float delay; //delay between each character typed
    [SerializeField] private string fullText; //this is the full text that will be typed
    private string currentText; //this is what the current text is displayed
    private string remainText; //this is the remaining text that isnt typed yet
    public bool Finish = false; //for the warp text effect, checks if it is done typing before applying any effects

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI TextBox; // text mesh pro for the .text we are changing
    //if we wanted a sound to play when something is typed, link the sound stuff here

    [Header("Warp Text")]
    [SerializeField] bool HasWarp;
    private WarpTextExample WarpText;

    private void Start()
    {
        Init();
        StartCoroutine(Typewrite());
    }

    void Init()
    {
        if(TextBox == null) { TextBox = GetComponent<TextMeshProUGUI>(); }
        if(fullText == "") { fullText = TextBox.text; } //if text is not in the inspector for the code, it checks the text mesh pro to see if you typed the desired text there and gets it
        currentText = ""; //reset current text to be blank for the typing
        remainText = fullText;
    }

    public IEnumerator Typewrite()
    {
        int charLength = 0;
        int charLeft = fullText.Length;
        while(!Finish) //if the final text doesnt match the current text, ie you have more to type
        {
            currentText = fullText.Substring(0, charLength); //update text
            charLeft = fullText.Length - charLength; //measure how many characters left to type
            if(charLeft > 0) { currentText = currentText.Insert(currentText.Length, new string(' ', charLeft)); } //fill space with empty spaces for warp
            TextBox.text = currentText; //update textbox

            charLength++;
            if (currentText == fullText) { Finish = true; }
            yield return new WaitForSeconds(delay); //delay before next char typed
        }
    }

}
