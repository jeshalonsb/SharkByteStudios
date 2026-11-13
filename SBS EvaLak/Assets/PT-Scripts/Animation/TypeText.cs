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
        while(currentText != fullText) //if the final text doesnt match the current text, ie you have more to type
        {
            charLength = currentText.Length; //find how many char are typed
            remainText = fullText.Substring(charLength); //remaining text is calculated by removing the amount of char already typed
            currentText = currentText + remainText[0]; //current text adds the first char from the remaining text
            TextBox.text = currentText; //update textbox
            yield return new WaitForSeconds(delay); //delay before next char typed
        }
        Finish = true; //if current text matches full text, then it is finished typing
        if(HasWarp) { WarpText = GetComponent<WarpTextExample>(); WarpText.WarpFunction(); }
    }

}
