using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseMenuBlurController : MonoBehaviour
{
    [SerializeField] private Material BlurMaterial;

    public IEnumerator IncreaseBlur(float targetBlur, float duration)
    {
        float startBlur = BlurMaterial.GetFloat("_Size");
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float newBlur = Mathf.Lerp(startBlur, targetBlur, elapsedTime / duration);
            BlurMaterial.SetFloat("_Size", newBlur);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        BlurMaterial.SetFloat("_Size", targetBlur);
    }
    public void StartBlur(float targetBlur)
    {
        BlurMaterial.SetFloat("_Size", 0f);
        StartCoroutine(IncreaseBlur(targetBlur, 1f));
    }
    public IEnumerator ChangeBlurColor(Color targetColor, float duration)
{
    Color startColor = BlurMaterial.GetColor("_AdditiveColor");
    float elapsedTime = 0;

    while (elapsedTime < duration)
    {
        Color newColor = Color.Lerp(startColor, targetColor, elapsedTime / duration);
        BlurMaterial.SetColor("_AdditiveColor", newColor);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    BlurMaterial.SetColor("_AdditiveColor", targetColor);
}

public void StartChangeBlurColor()
{
    BlurMaterial.SetColor("_AdditiveColor", new Color(0f, 0f, 0f, 0f) );
    StartCoroutine(ChangeBlurColor(new Color(0.0849f, 0f, 0f, 0f), 1f));
}
}