using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using ExitGames.Client.Photon;
using JetBrains.Annotations;

public class RoomController : MonoBehaviourPunCallbacks
{
    public GameObject playerInfoPrefab;
    public Transform playerListContent;
    public TMP_InputField PlayerNameInputField;
    public GameObject GameModeMenu;
    public TMP_Text GameModeText;
    public Button GameModeButton;
    public Button ReadyButton;
    public TMP_Text ReadyButtonText;
    public TMP_Text PlayerStatusText;
    public Animator CircleAnim;

    private List<GameObject> playerListEntries = new List<GameObject>();

    public override void OnJoinedRoom()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddPlayerInfo(player);
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("GameMode") && PhotonNetwork.CurrentRoom.CustomProperties["GameMode"].ToString() != "None")
        {
            ReadyButton.interactable = true;
        }
        else
        {
            ReadyButton.interactable = false;
        }


        ReadyButtonText.text = "не готовий";
        ReadyButton.onClick.AddListener(SetReady);
        // Перевіряємо, чи є локальний гравець майстром клієнта
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            // Якщо гравець є майстром клієнта, він може вибирати режими гри
            GameModeButton.interactable = true;
        }
        else
        {
            // Якщо гравець не є майстром клієнта, він не може вибирати режими гри
            GameModeButton.interactable = false;
        }
        UpdateRoomInfo();
        UpdatePlayersInfo();

    }
    public override void OnLeftRoom()
    {
        foreach (GameObject playerInfoObject in playerListEntries)
        {
            Destroy(playerInfoObject);
        }
        // Очищуємо список об'єктів гравця
        playerListEntries.Clear();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerInfo(newPlayer);
        UpdateRoomInfo();

        if (PhotonNetwork.PlayerList.Length > 1)
        {
            if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["Ready"] == true)
            {
                ReadyButtonText.text = "готовий";
            }
            else
            {
                ReadyButtonText.text = "не готовий";
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateRoomInfo();
        RemovePlayerInfo(otherPlayer);
        PhotonNetwork.LocalPlayer.CustomProperties["Ready"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        SetNotReady();
        if (otherPlayer.ActorNumber == PhotonNetwork.CurrentRoom.MasterClientId)
        {
            // Якщо гравець, який покинув кімнату, був лідером, встановлюємо нового лідера
            Player newLeader = PhotonNetwork.CurrentRoom.GetPlayer(PhotonNetwork.CurrentRoom.MasterClientId);
            PhotonNetwork.CurrentRoom.SetMasterClient(newLeader);
            PhotonNetwork.CurrentRoom.CustomProperties["Room_Leader"] = PhotonNetwork.CurrentRoom.MasterClientId;
            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        }
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            // Якщо гравець є майстром клієнта, він може вибирати режими гри
            GameModeButton.interactable = true;
        }
        else
        {
            // Якщо гравець не є майстром клієнта, він не може вибирати режими гри
            GameModeButton.interactable = false;
        }
    }

    private void UpdateRoomInfo()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("GameMode"))
        {
            string gameMode = PhotonNetwork.CurrentRoom.CustomProperties["GameMode"].ToString();

            switch (gameMode)
            {
                case "Classic":
                    GameModeText.text = "режим: Класичний";
                    ReadyButton.interactable = true;
                    break;
                case "Race":
                    GameModeText.text = "режим: Гонка";
                    ReadyButton.interactable = true;
                    break;
                case "Obstacles":
                    GameModeText.text = "режим: Перешкоди";
                    ReadyButton.interactable = true;
                    break;
                case "cat-mutant":
                    GameModeText.text = "режим: Кіт-мутант";
                    ReadyButton.interactable = true;
                    break;
                default:
                    GameModeText.text = "режим: не вибрано";
                    ReadyButton.interactable = false;
                    break;
            }
        }
        else
        {
            GameModeText.text = "режим: не вибрано";
            ReadyButton.interactable = false;
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        UpdateRoomInfo();
        if (propertiesThatChanged.ContainsKey("GameMode").ToString() != "None")
        {
            ReadyButton.interactable = true;
        }
        else
        {
            ReadyButton.interactable = false;
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Ready"))
        {
            UpdatePlayersInfo();
            if (PhotonNetwork.PlayerList.Length >= 2)
            {
                bool allPlayersReady = true;
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if (!(bool)player.CustomProperties["Ready"])
                    {
                        Debug.Log("Not all players are ready.");
                        StopAllCoroutines();
                        if (player == PhotonNetwork.LocalPlayer)
                        {
                            ReadyButtonText.text = "не готовий";
                        }
                        allPlayersReady = false;
                    }
                    else if (player == PhotonNetwork.LocalPlayer)
                    {
                        ReadyButtonText.text = "готовий";
                    }
                }
                if (!allPlayersReady)
                {
                    return;
                }
                Debug.Log("All players are ready.");
                if (PhotonNetwork.IsMasterClient)
                {
                    string firstPlayerSide = UnityEngine.Random.Range(0, 2) == 0 ? "dog" : "cat";
                    string secondPlayerSide = firstPlayerSide == "dog" ? "cat" : "dog";

                    PhotonNetwork.CurrentRoom.CustomProperties["Player_1_Side"] = firstPlayerSide;
                    PhotonNetwork.CurrentRoom.CustomProperties["Player_2_Side"] = secondPlayerSide;
                    PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
                }
                StartCoroutine(StartCountdown());
            }
            else
            {
                ReadyButtonText.text = "поклич друга";
            }
        }
    }
    private Dictionary<string, bool> playerReadyStates = new Dictionary<string, bool>();
    private Dictionary<string, GameObject> playerInfoObjects = new Dictionary<string, GameObject>();

    private void UpdatePlayersInfo()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            bool isReady = (bool)player.CustomProperties["Ready"];

            // Оновлюємо текст тільки тоді, коли стан готовності гравця змінюється
            if (!playerReadyStates.ContainsKey(player.NickName) || playerReadyStates[player.NickName] != isReady)
            {
                string readyText = isReady ? "готовий" : "не готовий";

                // Перевіряємо, чи існує playerInfoObject для цього гравця
                if (playerInfoObjects.ContainsKey(player.NickName))
                {
                    GameObject playerInfoObject = playerInfoObjects[player.NickName];

                    TMP_Text playerReadyText = playerInfoObject.transform.GetChild(2).GetComponent<TMP_Text>();
                    playerReadyText.text = readyText;

                    playerReadyStates[player.NickName] = isReady;
                }
            }
        }
        Debug.Log("Змінено стан готовності гравця: " + PhotonNetwork.LocalPlayer.NickName + " " + (bool)PhotonNetwork.LocalPlayer.CustomProperties["Ready"] + " " + PhotonNetwork.CurrentRoom.PlayerCount + " " + PhotonNetwork.CurrentRoom.MaxPlayers);
    }

    private void AddPlayerInfo(Player player)
    {
        GameObject playerInfoObject = Instantiate(playerInfoPrefab, playerListContent);
        playerInfoObject.name = player.NickName;
        TMP_Text playerNameText = playerInfoObject.transform.GetChild(1).GetComponent<TMP_Text>();
        PlayerStatusText = playerInfoObject.transform.GetChild(2).GetComponent<TMP_Text>();
        playerNameText.text = player.NickName;

        playerListEntries.Add(playerInfoObject);

        // Додаємо playerInfoObject до playerInfoObjects
        playerInfoObjects[player.NickName] = playerInfoObject;

        // Додаємо стан готовності гравця до playerReadyStates
        bool isReady = (bool)player.CustomProperties["Ready"];
        playerReadyStates[player.NickName] = isReady;
        PlayerStatusText.text = isReady ? "готовий" : "не готовий";
    }
    public void SetPlayerName()
    {
        PhotonNetwork.NickName = PlayerNameInputField.text;
    }

    private void RemovePlayerInfo(Player player)
    {
        foreach (GameObject playerInfoObject in playerListEntries)
        {
            TMP_Text playerNameText = playerInfoObject.transform.GetChild(1).GetComponent<TMP_Text>();
            if (playerNameText.text == player.NickName)
            {
                Destroy(playerInfoObject);
                playerListEntries.Remove(playerInfoObject);
                break;
            }
        }
    }
    public void TakeGameMode()
    {
        GameModeMenu.SetActive(true);
    }
    public void SetReady()
    {
        PhotonNetwork.LocalPlayer.CustomProperties["Ready"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        ReadyButton.GetComponent<Image>().color = new Color(0.79f, 1f, 0.72f, 1f);
        ReadyButtonText.text = "готовий";
        ReadyButton.onClick.RemoveAllListeners();
        ReadyButton.onClick.AddListener(SetNotReady);
    }

    public void SetNotReady()
    {
        StopAllCoroutines();
        PhotonNetwork.LocalPlayer.CustomProperties["Ready"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        ReadyButton.onClick.RemoveAllListeners();
        ReadyButton.onClick.AddListener(SetReady);
        ReadyButton.GetComponent<Image>().color = new Color(1f, 0.72f, 0.72f, 1f);
        ReadyButtonText.text = "не готовий";
    }
    private IEnumerator StartCountdown()
    {

        float countdownTime = 5f;
        while (countdownTime > 0)
        {
            // Оновлюємо текст ReadyButtonText
            ReadyButtonText.text = countdownTime.ToString("0.00");

            // Зменшуємо час
            countdownTime -= Time.deltaTime;

            // Чекаємо до наступного кадру
            yield return null;
        }

        ReadyButtonText.text = "Старт";
        if (PhotonNetwork.CurrentRoom.CustomProperties["GameMode"].ToString() == "Classic")
        {
            StartClassic();
        }
    }
    public void StartClassic()
    {
        CircleAnim.SetTrigger("changeMP");
    }



}