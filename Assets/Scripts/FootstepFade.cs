using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FootstepFade : MonoBehaviour
{
    public float fadeTime = 1f;
    public Image sr;

    void Start()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float t = 0;
        Color c = sr.color;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            sr.color = new Color(c.r, c.g, c.b, 1 - t / fadeTime);
            yield return null;
        }
        Destroy(gameObject);
    }
}
