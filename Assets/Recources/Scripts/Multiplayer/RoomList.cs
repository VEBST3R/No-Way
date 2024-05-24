using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;

public class RoomList : MonoBehaviourPunCallbacks
{
    public GameObject lobbyButtonPrefab;
    public Transform scrollViewContent;
    public List<RoomInfo> roomList;
    public CreateRoom createroom;
    public Sprite[] passwordSprites;
    public GameObject passwordPanel;
    public Button joinPasswordButton;
    public TMP_InputField passwordInputField;
    public TMP_Text passwordErrorText;
    public TMP_Text roomNameText;
    public Animator PassMenuAnim;

    public override void OnJoinedLobby()
    {
        // Отримуємо список доступних кімнат при приєднанні до лобі
        PhotonNetwork.GetCustomRoomList(ConnectToServerScript.SqlLobby, "C0 > 999");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        this.roomList = roomList;
        UpdateRoomButtons();
    }

    public void UpdateRoomButtons()
    {
        // Видаляємо старі кнопки
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        // Створюємо нову кнопку для кожної кімнати
        foreach (RoomInfo roomInfo in roomList)
        {
            GameObject roomButtonObject = Instantiate(lobbyButtonPrefab, scrollViewContent);
            TMP_Text roomIDText = roomButtonObject.transform.GetChild(0).GetComponent<TMP_Text>();
            TMP_Text roomNameText = roomButtonObject.transform.GetChild(1).GetComponent<TMP_Text>();
            Image roomPassword = roomButtonObject.transform.GetChild(2).GetComponent<Image>();
            if (roomInfo.CustomProperties.ContainsKey("password"))
            {
                roomPassword.sprite = passwordSprites[0];
            }
            else
            {
                roomPassword.sprite = passwordSprites[1];
            }

            roomIDText.text = roomInfo.CustomProperties["C0"].ToString();
            roomNameText.text = roomInfo.Name;

            // Додаємо обробник подій до кнопки
            Button roomButton = roomButtonObject.GetComponent<Button>();
            roomButton.onClick.AddListener(() => JoinRoom(roomInfo, roomInfo.Name));
        }
    }

    private void JoinRoom(RoomInfo roomInfo, string roomName)
    {
        if (roomInfo.CustomProperties.ContainsKey("password"))
        {
            passwordPanel.SetActive(true);
            joinPasswordButton.onClick.RemoveAllListeners();
            joinPasswordButton.onClick.AddListener(() => JoinRoomWithPassword(roomInfo));
            passwordInputField.text = "";
            passwordErrorText.text = "";
            roomNameText.text = "приєднатися до \"" + roomName + "\"";

        }
        else
        {
            createroom.FromConnectToRoom();
            PhotonNetwork.JoinRoom(roomName);
        }

    }

    public void OnRefreshButtonClicked()
    {
        // Оновлюємо список кімнат при натисканні кнопки "Оновити"
        PhotonNetwork.GetCustomRoomList(ConnectToServerScript.SqlLobby, "C0 > 999");
    }
    public void JoinRoomWithPassword(RoomInfo roomInfo)
    {
        if (passwordInputField.text != roomInfo.CustomProperties["password"].ToString())
        {
            passwordErrorText.text = "Невірний пароль";
            passwordInputField.text = "";
        }
        else
        {
            passwordErrorText.text = "";
            createroom.FromConnectToRoom();
            PhotonNetwork.JoinRoom(roomInfo.Name);
            PassMenuAnim.SetTrigger("Back");
        }

    }
    public void clearErrorLog()
    {
        passwordErrorText.text = "";
    }
    public void passBack()
    {
        PassMenuAnim.SetTrigger("Back");
    }

}