using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class CreateRoom : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomNameInputField;
    public TMP_InputField playerNameInputField;
    public TMP_InputField passwordInputField;
    public Button createRoomButton;
    public Animator Anim;

    private void Start()
    {
        roomNameInputField.characterLimit = 6;
        createRoomButton.onClick.AddListener(CreateNewRoom);

        // Додаємо обробник подій до події onValueChanged для обох полів вводу
        roomNameInputField.onValueChanged.AddListener(UpdateCreateRoomButtonState);

        // Встановлюємо початковий стан кнопки
        UpdateCreateRoomButtonState("");

    }

    private void UpdateCreateRoomButtonState(string _)
    {
        // Кнопка "Створити кімнату" активна, тільки якщо обидва поля вводу не порожні
        createRoomButton.interactable = !string.IsNullOrEmpty(roomNameInputField.text);
    }

    private void CreateNewRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text) || string.IsNullOrEmpty(playerNameInputField.text))
        {
            Debug.LogError("Room name and player name should not be empty.");
            return;
        }

        PhotonNetwork.NickName = playerNameInputField.text;

        RoomOptions roomOptions = new RoomOptions();
        // Встановлюємо властивості кімнати
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "C0", Random.Range(1000, 10000) } };
        roomOptions.CustomRoomProperties.Add("Status", 0);
        roomOptions.CustomRoomProperties.Add("GameMode", "None");
        roomOptions.CustomRoomProperties.Add("Room_Leader", 0);
        roomOptions.CustomRoomProperties.Add("Player_1_Side", "none");
        roomOptions.CustomRoomProperties.Add("Player_2_Side", "none");
        roomOptions.CustomRoomProperties.Add("turn", "cat");
        roomOptions.CustomRoomProperties.Add("onPause", "none");
        if (Anim.GetBool("Pass") == true)
        {
            roomOptions.CustomRoomProperties.Add("password", passwordInputField.text);
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "C0", "password", "Status", "GameMode", "Room_Leader" };
        }
        else
        {
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "C0", "Status", "GameMode", "Room_Leader" };
        }

        // Вказуємо, які властивості кімнати повинні бути видимі і доступні для інших гравців
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = 2;

        PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions, ConnectToServerScript.SqlLobby);
        FromCreateToRoom();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created successfully.");
        PhotonNetwork.CurrentRoom.CustomProperties["Room_Leader"] = PhotonNetwork.CurrentRoom.MasterClientId;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Room creation failed: " + message);
    }
    public void FromCreateToRoom()
    {
        Anim.SetBool("InRoom", true);
    }
    public void FromConnectToRoom()
    {
        Anim.SetBool("InRoomFromConnect", true);
    }
    public void FromRoomToMenu()
    {
        Anim.SetBool("Create", false);
        Anim.SetBool("Pass", false);
        Anim.SetBool("InRoom", false);
        Anim.SetBool("InRoomFromConnect", false);
        Anim.SetBool("JoinToLobby", false);
        PhotonNetwork.LeaveRoom();
    }

}
