using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseBlurController : MonoBehaviour
{
    // Start is called before the first frame update[SerializeField] private Material BlurMaterial;
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
    public void EndBlur(float targetBlur)
    {
        BlurMaterial.SetFloat("_Size", 8.9f);
        StartCoroutine(IncreaseBlur(targetBlur, 1f));
    }
    public void disablegameObject()
    {
        gameObject.SetActive(false);
    }
}
