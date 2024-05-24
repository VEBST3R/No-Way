using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.UI;
using System.Linq;
public class InGameRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject PlayerDisconnectWaitMenu;
    [SerializeField] private TMP_Text LeftPlayerName;
    [SerializeField] private TMP_Text TimerLeftText;
    [SerializeField] private Animator Circle_MP;
    [SerializeField] private Button ContinueButton_FromWin;
    [SerializeField] private Button ContinueButton_FromLose;
    [SerializeField] private Animator PauseBloactorAnim;
    private TMP_Text ContinueButtonText;
    private Button Curbut;

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PauseBloactorAnim.SetTrigger("FastExit");
        PlayerDisconnectWaitMenu.SetActive(true);
        LeftPlayerName.text = "гравець " + otherPlayer.NickName + " від'єднався";
        StartCoroutine(StartCountdown());
    }
    private IEnumerator StartCountdown()
    {

        float countdownTime = 5f;
        while (countdownTime > 0)
        {
            // Оновлюємо текст лічильника
            TimerLeftText.text = countdownTime.ToString("0");

            // Зменшуємо час
            countdownTime -= Time.deltaTime;

            // Чекаємо до наступного кадру
            yield return null;
        }
        PhotonNetwork.Disconnect();
        Circle_MP.SetTrigger("ToRoom");
    }
    public void WinOnContinueButtonClicked_Ready()
    {
        StopAllCoroutines();
        Curbut = ContinueButton_FromWin;
        ContinueButtonText = ContinueButton_FromWin.GetComponentInChildren<TMP_Text>();
        PhotonNetwork.LocalPlayer.CustomProperties["Ready"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        ContinueButton_FromWin.GetComponent<Image>().color = new Color(0.79f, 1f, 0.72f, 1f);
        ContinueButtonText.text = "готовий";
        ContinueButton_FromWin.onClick.RemoveAllListeners();
        ContinueButton_FromWin.onClick.AddListener(WinOnContinueButtonClicked_Unready);
    }
    public void WinOnContinueButtonClicked_Unready()
    {
        StopAllCoroutines();
        Curbut = ContinueButton_FromWin;
        ContinueButtonText = ContinueButton_FromWin.GetComponentInChildren<TMP_Text>();
        PhotonNetwork.LocalPlayer.CustomProperties["Ready"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        ContinueButton_FromWin.GetComponent<Image>().color = new Color(1f, 0.72f, 0.72f, 1f);
        ContinueButtonText.text = "не готовий";
        ContinueButton_FromWin.onClick.RemoveAllListeners();
        ContinueButton_FromWin.onClick.AddListener(WinOnContinueButtonClicked_Ready);
    }
    public void LoseOnContinueButtonClicked_Ready()
    {
        StopAllCoroutines();
        Curbut = ContinueButton_FromLose;
        ContinueButtonText = ContinueButton_FromLose.GetComponentInChildren<TMP_Text>();
        PhotonNetwork.LocalPlayer.CustomProperties["Ready"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        ContinueButton_FromLose.GetComponent<Image>().color = new Color(0.79f, 1f, 0.72f, 1f);
        ContinueButtonText.text = "готовий";
        ContinueButton_FromLose.onClick.RemoveAllListeners();
        ContinueButton_FromLose.onClick.AddListener(LoseOnContinueButtonClicked_Unready);
    }
    public void LoseOnContinueButtonClicked_Unready()
    {
        StopAllCoroutines();
        Curbut = ContinueButton_FromLose;
        ContinueButtonText = ContinueButton_FromLose.GetComponentInChildren<TMP_Text>();
        PhotonNetwork.LocalPlayer.CustomProperties["Ready"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        ContinueButton_FromLose.GetComponent<Image>().color = new Color(1f, 0.72f, 0.72f, 1f);
        ContinueButtonText.text = "не готовий";
        ContinueButton_FromLose.onClick.RemoveAllListeners();
        ContinueButton_FromLose.onClick.AddListener(LoseOnContinueButtonClicked_Ready);
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (PhotonNetwork.PlayerList.All(p => (bool)p.CustomProperties["Ready"]))
        {
            // Запускаємо корутину з таймером
            StartCoroutine(StartGameCountdown());
        }
        else
        {
            StopAllCoroutines();
            if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Ready") && PhotonNetwork.LocalPlayer.CustomProperties["Ready"].Equals(true))
            {
                ContinueButtonText.text = "Готовий";
            }
            else
            {
                if (ContinueButtonText != null)
                {
                    ContinueButtonText.text = "Не готовий";
                }
                else
                {
                    Debug.Log("Не готовий");
                }
            }
        }
    }
    private IEnumerator StartGameCountdown()
    {
        // Встановлюємо тривалість таймера
        float countdownTime = 5.0f;

        while (countdownTime > 0)
        {
            // Виводимо час, що залишився
            ContinueButtonText.text = countdownTime.ToString("0.00");

            // Зменшуємо час
            countdownTime -= Time.deltaTime;

            // Чекаємо на наступний кадр
            yield return null;
        }

        // Запускаємо гру
        StartGame();
    }

    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            string firstPlayerSide = UnityEngine.Random.Range(0, 2) == 0 ? "dog" : "cat";
            string secondPlayerSide = firstPlayerSide == "dog" ? "cat" : "dog";

            PhotonNetwork.CurrentRoom.CustomProperties["Player_1_Side"] = firstPlayerSide;
            PhotonNetwork.CurrentRoom.CustomProperties["Player_2_Side"] = secondPlayerSide;
            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        }
        Circle_MP.SetTrigger("Restart");
    }
    public void Disconect()
    {
        Circle_MP.SetTrigger("Disconnect");
    }
}
