using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Data.Common;
using Unity.VisualScripting;

public class ClassicMobeButtonPlaceScript : MonoBehaviour
{
    private GameObject fog;
    [SerializeField] private ClassicMobeButtonPlaceScript[] allButtons;
    [SerializeField] private GameObject dog;
    [SerializeField] private GameObject cat;
    [SerializeField] public bool isDog;
    [SerializeField] public bool isCat;
    [SerializeField] private bool isCatFinish = false;
    [SerializeField] private Sprite[] worldItems;
    [SerializeField] private Sprite pawSprite;
    public List<Button> neigbourButtons = new List<Button>();
    [SerializeField] private ClassicModeController classicModeController;
    private PhotonView photonView;
    [SerializeField] private AudioSource bones;
    [SerializeField] private AudioSource Win;
    [SerializeField] private AudioSource Lose;
    [SerializeField] private AudioSource Main;
    [SerializeField] private AudioSource OpenBut;
    [SerializeField] private AudioSource OpenDog;

    public int moveCount = 0;
    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        isCat = false;
        isDog = false;
        PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        fog = gameObject.transform.GetChild(0).gameObject;
        if (PhotonNetwork.LocalPlayer.CustomProperties["Side"].ToString() == "cat")
        {
            fog.SetActive(false);
        }
        if (PhotonNetwork.LocalPlayer.CustomProperties["Side"].ToString() == "dog")
        {
            fog.SetActive(false);
        }
        gameObject.GetComponent<Button>().onClick.AddListener(onClick);
        gameObject.GetComponent<Button>().onClick.AddListener(Checker);
        fillMap();
    }
    private void onClick()
    {
        neigbourButtons.Clear();

        foreach (ClassicMobeButtonPlaceScript button in allButtons)
        {
            if (button.neigbourButtons.Count > 1)
            {
                // neigbourButtons.AddRange(button.neigbourButtons);
                button.neigbourButtons.Clear();
            }
            button.GetComponent<Button>().interactable = false;
        }
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(1f, 1f), 0f);
        foreach (Collider2D collider in colliders)
        {
            ClassicMobeButtonPlaceScript button = collider.GetComponent<ClassicMobeButtonPlaceScript>();
            if (button != null && button != this)
            {
                // Check if the button is strictly on the left, right, top, or bottom
                Vector3 direction = button.transform.position - transform.position;
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    // The button is on the left or right
                    if (Mathf.Abs(direction.y) < 0.1f)
                    {
                        // Make neighbour buttons interactive
                        button.GetComponent<Button>().interactable = true;
                        neigbourButtons.Add(button.GetComponent<Button>());
                    }
                }
                else
                {
                    // The button is on the top or bottom
                    if (Mathf.Abs(direction.x) < 0.1f)
                    {
                        // Make neighbour buttons interactive
                        button.GetComponent<Button>().interactable = true;
                        neigbourButtons.Add(button.GetComponent<Button>());
                    }
                }
            }
        }

        if (PhotonNetwork.LocalPlayer.CustomProperties["Side"].ToString() == "dog")
        {
            foreach (ClassicMobeButtonPlaceScript button in allButtons)
            {
                photonView.RPC("SetDogState", RpcTarget.All, button.photonView.ViewID, false);
            }
            photonView.RPC("SetDogState", RpcTarget.All, photonView.ViewID, true);
            photonView.RPC("ExecuteOnAllDevices", RpcTarget.All);
            if (!(isDog && isCat))
            {
                StartCoroutine(MoveDogToPosition(gameObject.transform.position, 2f));
                OpenBut.Play();
            }
            else
            {
                photonView.RPC("Check", RpcTarget.All);
                DisableNeighbourButtons();
                return;
            }
        }
        if (PhotonNetwork.LocalPlayer.CustomProperties["Side"].ToString() == "cat")
        {
            foreach (ClassicMobeButtonPlaceScript button in allButtons)
            {
                photonView.RPC("SetCatState", RpcTarget.All, button.photonView.ViewID, false);
            }
            photonView.RPC("SetCatState", RpcTarget.All, photonView.ViewID, true);
            photonView.RPC("ExecuteOnAllDevices", RpcTarget.All);
            if (!(isDog && isCat))
            {
                StartCoroutine(MoveCatToPosition(gameObject.transform.position, 2f));
                OpenBut.Play();
            }
            else
            {
                photonView.RPC("Check", RpcTarget.All);
                DisableNeighbourButtons();
                return;
            }
        }
    }
    [PunRPC]
    void SetDogState(int viewID, bool isDog)
    {
        PhotonView target = PhotonView.Find(viewID);
        ClassicMobeButtonPlaceScript button = target.GetComponent<ClassicMobeButtonPlaceScript>();
        button.isDog = isDog;
        // Change the color of the button if isDog is true
    }

    [PunRPC]
    void SetCatState(int viewID, bool isCat)
    {
        PhotonView target = PhotonView.Find(viewID);
        ClassicMobeButtonPlaceScript button = target.GetComponent<ClassicMobeButtonPlaceScript>();
        button.isCat = isCat;
        if (isCat)
        {
            System.Random random = new System.Random();
            List<Button> neighbourButtons = button.GetComponent<ClassicMobeButtonPlaceScript>().neigbourButtons;

            if (neighbourButtons.Count > 0)
            {
                Button randomButton = neighbourButtons[random.Next(neighbourButtons.Count)];
                Image imageComponent = null;

                if (randomButton.gameObject.transform.Find("Items") != null)
                {
                    imageComponent = randomButton.gameObject.transform.Find("Items").GetComponent<Image>();
                }

                if (imageComponent != null && imageComponent.sprite == null)
                {
                    // Sync the sprite first
                    imageComponent.GetComponent<PhotonView>().RPC("SyncSpriteRPC", RpcTarget.Others, "Item-1", moveCount);

                    Sprite sprite = Resources.Load<Sprite>("Item-1");
                    Debug.Log("Loaded sprite: " + sprite);
                    imageComponent.sprite = sprite;
                    imageComponent.color = new Color(1f, 1f, 1f, 1f);
                    moveCount++;

                    // If the move count is 2, remove the sprite
                    if (moveCount == 2)
                    {
                        imageComponent.sprite = null;
                        imageComponent.color = new Color(1f, 1f, 1f, 0f);
                        moveCount = 0; // Reset the move count
                    }
                }
            }
        }
        else
        {
            button.gameObject.transform.GetChild(1).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.53f);
        }
    }

    IEnumerator MoveCatToPosition(Vector3 targetPosition, float moveSpeed)
    {
        // Включаємо анімацію руху
        cat.GetComponent<Animator>().SetBool("isMoving", true);

        // Обчислюємо напрямок до цільової позиції
        Vector3 direction = targetPosition - cat.transform.position;
        direction.Normalize();

        // Повертаємо собаку в напрямку цільової позиції
        cat.transform.up = direction;
        cat.transform.eulerAngles = new Vector3(cat.transform.eulerAngles.x, 0, cat.transform.eulerAngles.z);

        while ((cat.transform.position - targetPosition).sqrMagnitude > 0.01f)
        {
            cat.transform.position = Vector3.MoveTowards(cat.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Вимикаємо анімацію руху
        cat.GetComponent<Animator>().SetBool("isMoving", false);

    }
    IEnumerator MoveDogToPosition(Vector3 targetPosition, float moveSpeed)
    {
        // Включаємо анімацію руху
        dog.GetComponent<Animator>().SetBool("isMoving", true);

        // Обчислюємо напрямок до цільової позиції
        Vector3 direction = targetPosition - dog.transform.position;
        direction.Normalize();

        // Повертаємо собаку в напрямку цільової позиції
        dog.transform.up = direction;
        dog.transform.eulerAngles = new Vector3(dog.transform.eulerAngles.x, 0, dog.transform.eulerAngles.z);

        while ((dog.transform.position - targetPosition).sqrMagnitude > 0.01f)
        {
            dog.transform.position = Vector3.MoveTowards(dog.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Вимикаємо анімацію руху
        dog.GetComponent<Animator>().SetBool("isMoving", false);
    }
    [PunRPC]
    void ExecuteOnAllDevices()
    {
        classicModeController.NeighboursManager();
    }
    [PunRPC]
    void FillMapRPC(int itemIndex)
    {
        Transform imageTransform = gameObject.transform.Find("Items");
        if (imageTransform != null)
        {
            Image imageComponent = imageTransform.GetComponent<Image>();
            if (imageComponent != null && imageComponent.sprite == null)
            {
                imageComponent.sprite = worldItems[itemIndex];
                if (itemIndex == 0)
                {
                    imageComponent.color = new Color(1f, 1f, 1f, 0f);
                }
                else
                {
                    imageComponent.color = new Color(1f, 1f, 1f, 1f);
                }
            }
        }
    }

    private void fillMap()
    {
        int itemIndex = itemrandomizer();
        photonView.RPC("FillMapRPC", RpcTarget.All, itemIndex);
    }
    private int itemrandomizer()
    {
        System.Random random = new System.Random();
        int randomNumber = random.Next(0, 101); // Generate a number between 0 and 100

        int itemIndex = 0; // -1 means no item

        // 35% chance to have an item
        if (randomNumber < 12)
        {
            itemIndex = random.Next(0, 5);
            return itemIndex;
        }
        else
        {
            return 0;
        }

    }
    public void Checker()
    {
        ClassicMobeButtonPlaceScript info = gameObject.GetComponent<ClassicMobeButtonPlaceScript>();
        if (info.isCat && info.isCatFinish)
        {
            photonView.RPC("CheckWinOrLose", RpcTarget.All);
        }
    }
    [PunRPC]
    void CheckWinOrLose()
    {
        Debug.LogWarning("Перевірка перемоги або поразки");
        bool isLocalPlayerDog = PhotonNetwork.LocalPlayer.CustomProperties["Side"].ToString() == "dog";
        bool isLocalPlayerCat = PhotonNetwork.LocalPlayer.CustomProperties["Side"].ToString() == "cat";

        if (isDog && isCat)
        {
            DisableNeighbourButtons();
            if (isLocalPlayerDog)
            {
                Win.Play();
                Main.Stop();
                Lose.Stop();
                OpenDog.Play();
                classicModeController.OnDogWin();
            }
            else if (isLocalPlayerCat)
            {
                Win.Stop();
                Main.Stop();
                Lose.Play();
                OpenDog.Play();
                classicModeController.OnCatLose();
            }
        }
        else if (isCat && isCatFinish)
        {
            DisableNeighbourButtons();
            if (isLocalPlayerDog)
            {
                Win.Stop();
                Main.Stop();
                Lose.Play();
                classicModeController.OnDogLose();
            }
            else if (isLocalPlayerCat)
            {
                Win.Play();
                Main.Stop();
                Lose.Stop();
                classicModeController.OnCatWin();
            }
        }
    }

    void DisableNeighbourButtons()
    {
        foreach (ClassicMobeButtonPlaceScript button in allButtons)
        {
            if (button.isDog || button.isCat)
            {
                foreach (Button neighbourButton in button.neigbourButtons)
                {
                    neighbourButton.interactable = false;
                }
            }
        }
    }
    IEnumerator MoveDogToCat(float duration)
    {
        float elapsedTime = 0;
        Vector3 dogStartPosition;
        Vector3 catPosition;
        dog.SetActive(true);
        cat.SetActive(true);
        if (PhotonNetwork.LocalPlayer.CustomProperties["Side"].ToString() == "dog")
        {
            RectTransform rectTransform = dog.GetComponent<RectTransform>();
            rectTransform.pivot = new Vector2(0.5f, 0.8f);
            dogStartPosition = dog.transform.position;
            catPosition = gameObject.transform.position;
        }
        else
        {
            RectTransform rectTransform = dog.GetComponent<RectTransform>();
            rectTransform.pivot = new Vector2(0.5f, 0.8f);
            dogStartPosition = gameObject.transform.position;
            catPosition = cat.transform.position;
        }
        if (PhotonNetwork.LocalPlayer.CustomProperties["Side"].ToString() == "dog")
        {
            // Set cat's position to the button where isCat = true
            foreach (ClassicMobeButtonPlaceScript button in allButtons)
            {
                if (button.isCat)
                {
                    cat.transform.position = button.transform.position;
                    break;
                }
            }
        }

        while (elapsedTime < duration)
        {
            dog.transform.position = Vector3.Lerp(dogStartPosition, catPosition, elapsedTime / duration);

            // Змусити собаку дивитися на кота
            Vector2 direction = catPosition - dog.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            dog.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        dog.transform.position = catPosition;
    }
    IEnumerator HandleDogAndCatInteraction()
    {
        yield return StartCoroutine(MoveDogToCat(0.5f));
        bones.Play();
        yield return new WaitForSeconds(1f);
        photonView.RPC("CheckWinOrLose", RpcTarget.All);
    }
    [PunRPC]
    void Check()
    {
        StartCoroutine(HandleDogAndCatInteraction());
    }
}
