using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class GameModes : MonoBehaviour
{
    public Material BlurMaterial;
    public Animator Anim;
    public RoomController RoomController;
    private IEnumerator LerpMaterialSize()
    {
        float duration = 1f; // Змініть це значення, щоб контролювати швидкість анімації
        float elapsedTime = 0f;
        float startSize = 0f;
        float endSize = 10f;

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
        float startSize = 10f;
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
        gameObject.SetActive(false);
    }
    public void ClassicMode()
    {
        PhotonNetwork.CurrentRoom.CustomProperties["GameMode"] = "Classic";
        PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        Anim.SetTrigger("back");
        Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties["GameMode"].ToString());
    }

    public void RaceMode()
    {
        PhotonNetwork.CurrentRoom.CustomProperties["GameMode"] = "Race";
        PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        Anim.SetTrigger("back");
        Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties["GameMode"].ToString());
    }

    public void ObstaclesMode()
    {
        PhotonNetwork.CurrentRoom.CustomProperties["GameMode"] = "Obstacles";
        PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        Anim.SetTrigger("back");
        Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties["GameMode"].ToString());
    }

    public void cat_mutantMode()
    {
        PhotonNetwork.CurrentRoom.CustomProperties["GameMode"] = "cat-mutant";
        PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        Anim.SetTrigger("back");
        Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties["GameMode"].ToString());
    }
}
