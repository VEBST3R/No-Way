using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using GooglePlayGames;
using UnityEngine.UI;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using TMPro;

public class ConnectToServerScript : MonoBehaviourPunCallbacks
{
    public Animator Anim;
    public Image PlayerImage;
    public TMP_Text ErrorMainName;
    public TMP_Text ErrorMainText;

    public void Connect()
    {
        Debug.Log("Connecting to server.");
        PhotonNetwork.ConnectUsingSettings();
    }
    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Anim.SetBool("Connect", false);
        Anim.SetBool("Error", true);
        ErrorMainName.text = "помилка підключення";
        ErrorMainText.text = "виникла помилка підключення до сервера";
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master server");
        Anim.SetBool("Connect", true);

#if UNITY_ANDROID

        Texture2D texture = Social.localUser.image;

        PlayerImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));


        PhotonNetwork.LocalPlayer.CustomProperties["Name"] = Social.localUser.userName;

        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
#endif

        PhotonNetwork.LocalPlayer.CustomProperties["Ready"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);

        PhotonNetwork.LocalPlayer.CustomProperties["Side"] = "None";
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);

        ConnectToServer();
    }
    public static TypedLobby SqlLobby;
    public static void ConnectToServer()
    {
        SqlLobby = new TypedLobby("developerLobby", LobbyType.SqlLobby);
        PhotonNetwork.JoinLobby(SqlLobby);
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Було підключено до лоббі: " + PhotonNetwork.CurrentLobby.Name);
    }
}
