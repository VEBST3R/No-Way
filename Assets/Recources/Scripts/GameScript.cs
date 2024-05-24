
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
public class GameScript : MonoBehaviour
{
    public Animator Circle;
    public Animator Anim;
    [SerializeField] private Toggle _PassLobby;
    public TMP_Text first_place_scoreText;
    public TMP_Text second_place_scoreText;
    public TMP_Text third_place_scoreText;
    public CanvasGroup multiplayer;
    public TMP_InputField NickName;
    public AudioSource audioSource;
    public TMP_Text ConnectText;
    public TMP_Text ErrorMainName;
    public TMP_Text ErrorMainText;
    [SerializeField] private ConnectToServerScript _connectToServerScript;
    public void Start()
    {
        Anim.SetBool("Menu", true);
        ConnectText.text = "підключення до Google Play Games";
    }
    public void StartGame()
    {
        Circle.SetTrigger("change");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void myRecordON()
    {
        // Anim.SetBool("on", true);
        // if (!PlayerPrefs.HasKey("HighScore"))
        // {
        //     first_place_scoreText.text = "1. <b> - </b>";
        // }
        // else
        // {
        //     first_place_scoreText.text = "1. <b>" + PlayerPrefs.GetInt("HighScore").ToString() + "</b>";
        // }
        // if (!PlayerPrefs.HasKey("SecondHighScore"))
        // {
        //     second_place_scoreText.text = "2. <b> - </b>";
        // }
        // else
        // {
        //     second_place_scoreText.text = "2. <b>" + PlayerPrefs.GetInt("SecondHighScore").ToString() + "</b>";
        // }
        // if (!PlayerPrefs.HasKey("ThirdHighScore"))
        // {
        //     third_place_scoreText.text = "3. <b> - </b>";
        // }
        // else
        // {
        //     third_place_scoreText.text = "3. <b>" + PlayerPrefs.GetInt("ThirdHighScore").ToString() + "</b>";
        // }
        PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder().Build());
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    Social.ShowLeaderboardUI();
                }
                else
                {
                    Anim.SetTrigger("ErrorLad");
                }
            });

        }
        else
        {
            Social.ShowLeaderboardUI();
        }


    }
    public void myRecordOFF()
    {
        Anim.SetTrigger("ErrorLad");
        if (Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.SignOut();
        }
    }
    public void Click()
    {
        audioSource.Play();
    }

    private PlayGamesPlatform platform;
    public void MyltiplayerOn()
    {
        StopAllCoroutines();
        Anim.SetBool("MultiplayerOn", true);

        PlayGamesPlatform.InitializeInstance(new PlayGamesClientConfiguration.Builder().Build());
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {

                    Debug.Log("Авторизація пройшла успішно");
                    Anim.SetBool("Error", false);
                    _connectToServerScript.Connect();
                    ConnectText.text = "підключення до сервера";
                }
                else
                {
                    Anim.SetBool("Error", true);
                    ErrorMainName.text = "помилка авторизації";
                    ErrorMainText.text = "ви не авторизовані в Google Play Games";
                    Debug.Log("Error ви не авторизовані в Google Play Games");
                }
            });
        }
        else
        {
            Debug.Log("Авторизація пройшла успішно");
            Anim.SetBool("Error", false);
            _connectToServerScript.Connect();
            ConnectText.text = "підключення до сервера";
        }

    }
    public void OnReconnectClick()
    {
        Anim.SetBool("Menu", false);
        StartCoroutine(reconnect());
    }
    public IEnumerator reconnect()
    {
        Anim.SetBool("Error", false);

        yield return new WaitForSeconds(2);

        MyltiplayerOn();

        yield return null;
    }
    public void DisconnectToMenu()
    {
        StopAllCoroutines();
        Anim.SetBool("Menu", true);
        Anim.SetBool("MultiplayerOn", false);
        Anim.SetBool("Connect", false);
        Anim.SetBool("Error", false);
    }
    public void MyltiplayerOff()
    {
        Anim.SetBool("MultiplayerOn", false);
        Anim.SetBool("Connect", false);
    }
    public void CreateLobbyOn()
    {
        _PassLobby.isOn = false;
        Anim.SetBool("Create", true);
    }
    public void CreateLobbyOff()
    {
        Anim.SetBool("Create", false);
    }
    public void EnterPassword()
    {
        if (_PassLobby.isOn)
        {
            Anim.SetBool("Pass", true);
        }
        else
        {
            Anim.SetBool("Pass", false);
        }
    }
    public void ViewListOfGames()
    {
        Anim.SetBool("JoinToLobby", true);
    }
    public void CloseListOfGames()
    {
        Anim.SetBool("JoinToLobby", false);
    }
    public void SaveNickName()
    {
        PlayerPrefs.SetString("NickName", NickName.text);
        PhotonNetwork.NickName = NickName.text;
    }
}
