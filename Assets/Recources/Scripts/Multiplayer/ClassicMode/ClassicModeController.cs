using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;

public class ClassicModeController : MonoBehaviour
{
    [SerializeField] private GameObject dogSideMenu;
    [SerializeField] private GameObject catSideMenu;
    [SerializeField] private Material BlurMaterial;
    [SerializeField] private GameObject MinePlace;
    [SerializeField] private GameObject InfoObjectForDog;
    [SerializeField] private GameObject InfoObjectForCat;
    [SerializeField] private GameObject dog;
    [SerializeField] private GameObject cat;
    [SerializeField] private TMP_Text CatSideTurnText;
    [SerializeField] private TMP_Text DogSideTurnText;
    [SerializeField] private Button[] AllButtons;
    [SerializeField] private Button[] StartDogActiveButtons;
    [SerializeField] private Button[] StartCatActiveButtons;
    [SerializeField] private Sprite DogWinSprite;
    [SerializeField] private Sprite CatWinSprite;
    [SerializeField] private Sprite DogLoseSprite;
    [SerializeField] private Sprite CatLoseSprite;
    [SerializeField] private GameObject WinMenu;
    [SerializeField] private Image WinMenuImage;
    [SerializeField] private GameObject LoseMenu;
    [SerializeField] private Image LoseMenuImage;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject PauseMenuBlocator;
    [SerializeField] private string turn;
    [SerializeField] private Animator PauseAnim;
    [SerializeField] private Animator PauseBloactorAnim;
    [SerializeField] private TMP_Text PlayerOnPasueText;
    [SerializeField] private PauseMPScript pauseMPScript;
    private PhotonView photonView;
    private bool waitCat = false;

    private void Awake()
    {
        PhotonNetwork.LocalPlayer.CustomProperties["Ready"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);

        photonView = GetComponent<PhotonView>();
        Player[] players = PhotonNetwork.PlayerList;
        if (PhotonNetwork.LocalPlayer.ActorNumber == players[0].ActorNumber)
        {
            PhotonNetwork.LocalPlayer.CustomProperties["Side"] = PhotonNetwork.CurrentRoom.CustomProperties["Player_1_Side"].ToString();
            PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        }
        if (PhotonNetwork.LocalPlayer.ActorNumber == players[1].ActorNumber)
        {
            PhotonNetwork.LocalPlayer.CustomProperties["Side"] = PhotonNetwork.CurrentRoom.CustomProperties["Player_2_Side"].ToString();
            PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.CustomProperties["turn"] = "cat";
            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        }

        foreach (Button button in AllButtons)
        {
            button.interactable = false;
        }
        if (PhotonNetwork.LocalPlayer.CustomProperties["Side"].ToString() == "dog")
        {
            dogSideMenu.SetActive(true);
            SettingBlurOnStart();
            MinePlaceForDog();
            dog.SetActive(true);
            cat.SetActive(false);
            catSideMenu.SetActive(false);
            foreach (Button button in StartDogActiveButtons)
            {
                button.interactable = true;
            }
            waitCat = false;
        }
        if (PhotonNetwork.LocalPlayer.CustomProperties["Side"].ToString() == "cat")
        {
            dogSideMenu.SetActive(false);
            SettingBlurOnStart();
            MinePlaceForCat();
            dog.SetActive(false);
            cat.SetActive(true);
            catSideMenu.SetActive(true);
            foreach (Button button in StartCatActiveButtons)
            {
                button.interactable = false;

            }
            waitCat = true;
        }
        DogSideTurnText.text = "Ваш хід";
        CatSideTurnText.text = "Хід пса";
        comeOutInfo();
    }
    private void comeOutInfo()
    {
        Debug.Log("Перший гравець грає за: " + PhotonNetwork.CurrentRoom.CustomProperties["Player_1_Side"].ToString());
        Debug.Log("Другий гравець грає за: " + PhotonNetwork.CurrentRoom.CustomProperties["Player_2_Side"].ToString());
        Debug.Log("Хід гравця: " + PhotonNetwork.CurrentRoom.CustomProperties["turn"].ToString());
        if (PhotonNetwork.LocalPlayer.CustomProperties["Side"].ToString() == "dog")
        {
            Debug.Log("Ви граєте за собаку");
        }
        if (PhotonNetwork.LocalPlayer.CustomProperties["Side"].ToString() == "cat")
        {
            Debug.Log("Ви граєте за кота");
        }
        if (waitCat)
        {
            Debug.Log("Кіт ще ні разу не ходив");
        }
    }
    public void CloseSideMenu()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties["Side"].ToString() == "dog")
        {
            dogSideMenu.transform.GetComponent<Animator>().SetTrigger("Close");
            SettingBlurOnEnd();
        }
        if (PhotonNetwork.LocalPlayer.CustomProperties["Side"].ToString() == "cat")
        {
            catSideMenu.transform.GetComponent<Animator>().SetTrigger("Close");
            SettingBlurOnEnd();
        }
    }
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
        if (targetBlur == 0f)
        {
            if (dogSideMenu.activeSelf)
            {
                dogSideMenu.SetActive(false);
            }
            if (catSideMenu.activeSelf)
            {
                catSideMenu.SetActive(false);
            }
        }
    }
    public IEnumerator ChangeBlurColor(Color targetColor, float duration)
    {
        Color startColor = BlurMaterial.GetColor("_MultiplyColor");
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            Color newColor = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            BlurMaterial.SetColor("_MultiplyColor", newColor);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        BlurMaterial.SetColor("_MultiplyColor", targetColor);
    }
    public void NeighboursManager()
    {
        Button dog_button = null;
        Button cat_button = null;

        foreach (Button button in AllButtons)
        {
            var buttonScript = button.GetComponent<ClassicMobeButtonPlaceScript>();
            if (buttonScript.isDog)
            {
                dog_button = button;
            }
            else if (buttonScript.isCat)
            {
                cat_button = button;
            }
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("turn"))
        {
            turn = PhotonNetwork.CurrentRoom.CustomProperties["turn"].ToString();
        }
        else
        {
            turn = "dog";
            PhotonNetwork.CurrentRoom.CustomProperties["turn"] = turn;
            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        }

        if (waitCat)
        {
            if (turn == "cat" && PhotonNetwork.LocalPlayer.CustomProperties["Side"].ToString() == "cat")
            {
                foreach (Button button in StartCatActiveButtons)
                {
                    button.interactable = true;
                }
                waitCat = false;
                SetButtonNeighboursInteractable(dog_button, false);
            }
        }

        if (turn == "cat")
        {
            SetButtonNeighboursInteractable(cat_button, true);
            SetButtonNeighboursInteractable(dog_button, false);

            PhotonNetwork.CurrentRoom.CustomProperties["turn"] = "dog";
            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);

            CatSideTurnText.text = "Ваш хід";
            DogSideTurnText.text = "Хід кота";

        }
        else if (turn == "dog")
        {
            SetButtonNeighboursInteractable(dog_button, true);
            SetButtonNeighboursInteractable(cat_button, false);

            PhotonNetwork.CurrentRoom.CustomProperties["turn"] = "cat";
            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);

            CatSideTurnText.text = "Хід пса";
            DogSideTurnText.text = "Ваш хід";
        }
        comeOutInfo();
        Debug.LogWarning("Зараз ходить: " + PhotonNetwork.CurrentRoom.CustomProperties["turn"].ToString());
    }

    private void SetButtonNeighboursInteractable(Button button, bool interactable)
    {
        if (button != null)
        {
            foreach (Button neighbourButton in button.GetComponent<ClassicMobeButtonPlaceScript>().neigbourButtons)
            {
                neighbourButton.interactable = interactable;
            }
        }
    }
    public void SettingBlurOnStart()
    {
        BlurMaterial.SetFloat("_Size", 0f);
        BlurMaterial.SetColor("_MultiplyColor", new Color(1f, 1f, 1f, 1f));
        StartCoroutine(ChangeBlurColor(new Color(0.98f, 1f, 0.70f, 1f), 1f));
        StartCoroutine(IncreaseBlur(23f, 2f));
    }
    public void SettingBlurOnEnd()
    {
        StartCoroutine(ChangeBlurColor(new Color(1f, 1f, 1f, 1f), 1f));
        StartCoroutine(IncreaseBlur(0f, 2f));
    }
    public void MinePlaceForCat()
    {
        MinePlace.transform.rotation = Quaternion.Euler(0, 0, 0);
        MinePlace.transform.localPosition = new Vector2(MinePlace.transform.position.x, -76.548f);
        InfoObjectForCat.SetActive(true);
        InfoObjectForDog.SetActive(false);
    }
    public void MinePlaceForDog()
    {
        MinePlace.transform.rotation = Quaternion.Euler(0, 0, 180);
        MinePlace.transform.localPosition = new Vector2(MinePlace.transform.position.x, 72f);
        InfoObjectForCat.SetActive(false);
        InfoObjectForDog.SetActive(true);
    }
    public void OnCatWin()
    {
        WinMenu.SetActive(true);
        WinMenuImage.sprite = CatWinSprite;
    }
    public void OnDogWin()
    {
        WinMenu.SetActive(true);
        WinMenuImage.sprite = DogWinSprite;
    }
    public void OnCatLose()
    {
        LoseMenu.SetActive(true);
        LoseMenuImage.sprite = CatLoseSprite;
    }
    public void OnDogLose()
    {
        LoseMenu.SetActive(true);
        LoseMenuImage.sprite = DogLoseSprite;
    }
    [PunRPC]
    void SetPauseMenuStateRPC()
    {
        PauseMenuBlocator.SetActive(true);
        PlayerOnPasueText.text = "гравець " + PhotonNetwork.CurrentRoom.CustomProperties["onPause"] + " взяв паузу";
        pauseMPScript.wait();

    }
    [PunRPC]
    void SetPauseMenuStateRPC_Continue()
    {
        PauseBloactorAnim.SetTrigger("Exit");
    }
    public void OnPause()
    {
        PhotonNetwork.CurrentRoom.CustomProperties["onPause"] = PhotonNetwork.LocalPlayer.NickName;
        PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        PauseMenu.SetActive(true);
        photonView.RPC("SetPauseMenuStateRPC", RpcTarget.Others);
    }
    public void OnContinue()
    {
        PhotonNetwork.CurrentRoom.CustomProperties["onPause"] = "none";
        PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        PauseAnim.SetTrigger("Exit");
        photonView.RPC("SetPauseMenuStateRPC_Continue", RpcTarget.All);
    }
}
