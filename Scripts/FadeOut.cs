using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeOut : MonoBehaviour
{
    public float duration = 2f; // Duraci�n de la animaci�n
    private Image panelImage;

    void Start()
    {
        panelImage = GetComponent<Image>();
        if (panelImage != null)
        {
            StartCoroutine(FadeOutPanel());
        }
        else
        {
            Debug.LogError("No Image component found on this GameObject.");
        }
    }

    private IEnumerator FadeOutPanel()
    {
        float elapsedTime = 0f;
        Color panelColor = panelImage.color;

        // Aseg�rate de que la opacidad inicial sea 100%
        panelColor.a = 1f;
        panelImage.color = panelColor;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - (elapsedTime / duration));
            panelColor.a = alpha;
            panelImage.color = panelColor;
            yield return null;
        }

        // Aseg�rate de que la opacidad final sea 0%
        panelColor.a = 0f;
        panelImage.color = panelColor;
    }
}

