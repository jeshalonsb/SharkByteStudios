using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class DeckBuilderManager : MonoBehaviour
{
    [Header("Music")]
    public AudioSource audio;
    public AudioMixerGroup musicMixerGroup;

    [Header("Start Screen")]
    [SerializeField] private bool finishedTyping = false;
    [SerializeField] private GameObject bootText;
    [SerializeField] private TypeText typetext;
    bool startScreen = true; //turns false when unloaded, is start screen on

    [Header("DB Screen")]
    [SerializeField] private bool DBActive = false; //if full normal db is up (place where we do stuff)
    [SerializeField] private bool dbLoad = false; //if load screen
    [SerializeField] private GameObject loadDBScene;
    [SerializeField] private GameObject dbScreen;

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
        if (typetext != null) { finishedTyping = typetext.Finish; }
        if(finishedTyping) { Cutscene(); }
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


        if(loadDBScene == null) { loadDBScene = GameObject.Find("LoadDB"); }
        if (dbScreen == null) { dbScreen = GameObject.Find("OnScreen"); }
    }

    public void Cutscene()
    {
        if (startScreen)
        {
            //UnloadTextFunction(bootText);
            StartCoroutine(Wait(2f));
            bootText.SetActive(false);

            dbLoad = true;
        }

        if (dbLoad) 
        { 
            startScreen = false;

            loadDBScene.SetActive(true);
            //Animation to move from text to sprites
            loadDBScene.SetActive(false);

            DBActive = true; 
        }

        if (DBActive)
        {
            dbLoad = false;

            dbScreen.SetActive(true);
        }
    }

    public void UnloadTextFunction(GameObject o)
    {
        TextMeshProUGUI text = o.GetComponent<TextMeshProUGUI>();
        string currentText = text.text;
        int startLength = currentText.Length;

        StartCoroutine(UnloadText(currentText, startLength));
    }

    public IEnumerator UnloadText(string text, int length)
    {
        char[] newLineChars = { '\r', '\n' }; //indicators that line is entered
        int index = 0;

        bool unloaded = false;
        while(!unloaded)
        {
            index = text.IndexOfAny(newLineChars, index); //find index of line break
            if (index > 0 && text.Length > 0) { text = text.Substring(index, text.Length); } //new text is now without the area before first line break

            if (text.Length == 0) { unloaded = true; break; }
            yield return new WaitForSeconds(0.001f);
        }
    }

    public IEnumerator Wait(float delay)
    {
        yield return new WaitForSeconds(delay);
    }
}

