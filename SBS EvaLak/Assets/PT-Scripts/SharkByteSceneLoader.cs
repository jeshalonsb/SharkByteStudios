using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SharkByteSceneLoader : MonoBehaviour
{
    public void Start()
    {
        LoadSceneAfterWait("BootStrap");
    }

    public void LoadSceneAfterWait(string sceneName)
    {
        StartCoroutine(WaitAndLoad(sceneName));
    }

    private IEnumerator WaitAndLoad(string sceneName)
    {
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(sceneName);
    }
}
