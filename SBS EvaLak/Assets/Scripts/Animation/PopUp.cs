using System.Collections;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    [SerializeField] private bool isOpen = false;
    [SerializeField] private float duration = 0.25f;

    [SerializeField] private RectTransform rectTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        StartCoroutine(OpenPopUp());
    }

    private IEnumerator OpenPopUp()
    {
        Vector3 end = rectTransform.localScale;
        Vector3 start = Vector3.zero;

        rectTransform.localScale = start;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            rectTransform.localScale = Vector3.Lerp(start, end, smoothT);

            yield return null;
        }

        rectTransform.localScale = end;
        isOpen = true;
    }
}
