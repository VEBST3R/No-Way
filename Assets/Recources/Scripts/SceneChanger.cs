using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using Photon.Pun;
using Photon.Realtime;

public class SceneChanger : MonoBehaviour
{
    public Animator Anim;
    public TMP_Text scoreText;

    public void ChangeScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
    public void disableCircle()
    {
        gameObject.SetActive(false);
    }
    public void enableCircle()
    {
        gameObject.SetActive(true);
    }
    public void MP_ChangeScene(int sceneIndex)
    {
        PhotonNetwork.LoadLevel(sceneIndex);
    }
    public void MP_Dissconnect(int sceneIndex)
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(sceneIndex);
    }
}
