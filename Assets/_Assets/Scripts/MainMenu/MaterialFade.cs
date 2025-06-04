using System.Collections;
using UnityEngine;

public class MaterialFade : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Renderer objRenderer;

    /// <summary>
    /// Cambia la opacidad del material del objeto a un valor deseado.
    /// </summary>
    /// <param name="targetAlpha">El valor de opacidad al que se quiere llegar (0.0f completamente transparente, 1.0f completamente opaco)</param>
    public void Fade(float targetAlpha)
    {
        StartCoroutine(FadeRoutine(targetAlpha));
    }

    [ContextMenu("FadeIn")]
    public void FadeIn()
    {
        StartCoroutine(FadeRoutine(1));
    }

    [ContextMenu("FadeOut")]
    public void FadeOut()
    {
        StartCoroutine(FadeRoutine(0));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        Material mat = objRenderer.material;

        Color startColor = mat.color; // Color inicial del material
        float startAlpha = startColor.a; // Opacidad inicial
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float t = timer / fadeDuration;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            Color currentColor = startColor;
            currentColor.a = currentAlpha;

            mat.color = currentColor;

            yield return null;
        }

        Color finalColor = mat.color;
        finalColor.a = targetAlpha;
        mat.color = finalColor;
    }
}
