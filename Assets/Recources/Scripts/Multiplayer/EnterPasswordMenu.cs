using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterPasswordMenu : MonoBehaviour
{
    [SerializeField] private Material BlurMaterial;

    private IEnumerator LerpMaterialSize()
    {
        float duration = 1f; // Змініть це значення, щоб контролювати швидкість анімації
        float elapsedTime = 0f;
        float startSize = 0f;
        float endSize = 8.9f;

        while (elapsedTime < duration)
        {
            float newSize = Mathf.Lerp(startSize, endSize, elapsedTime / duration);
            BlurMaterial.SetFloat("_Size", newSize);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Забезпечуємо, що _Size точно дорівнює endSize в кінці анімації
        BlurMaterial.SetFloat("_Size", endSize);
    }

    private void OnEnable()
    {
        StartCoroutine(LerpMaterialSize());
    }
    private void Disable()
    {
        StartCoroutine(LerpMaterialSizeBackwards());
    }
    private IEnumerator LerpMaterialSizeBackwards()
    {
        float duration = 1f; // Змініть це значення, щоб контролювати швидкість анімації
        float elapsedTime = 0f;
        float startSize = 8.9f;
        float endSize = 0f;

        while (elapsedTime < duration)
        {
            float newSize = Mathf.Lerp(startSize, endSize, elapsedTime / duration);
            BlurMaterial.SetFloat("_Size", newSize);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Забезпечуємо, що _Size точно дорівнює endSize в кінці анімації
        BlurMaterial.SetFloat("_Size", endSize);
    }
    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

}
