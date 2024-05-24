using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using System.Linq;
public class ButtonPlaceScript : MonoBehaviour
{
    public float detectionRadius = 1.0f; // Радіус для виявлення сусідніх кнопок
    public LayerMask buttonLayer;
    public LayerMask ItemsLayer;
    public bool dogHere = false;
    public CanvasGroup mainMines;
    public GameObject PauseMenu;
    public GameObject WinMenu;
    public GameObject LoseMenu;
    public GameObject Circle;
    public bool _neighbor_ = false;
    public Button[] neighbors;

    private static List<ButtonPlaceScript> visitedButtons = new List<ButtonPlaceScript>();
    private List<Coroutine> _blinkCoroutines = new List<Coroutine>();
    private static List<Button> activeButtons = new List<Button>();
    [SerializeField] private Image ItemPlace;
    [SerializeField] private Sprite[] dogNeighboursItems;
    [SerializeField] private Sprite[] worldItems;
    [SerializeField] public GameObject fogObject;
    [SerializeField] public TMP_Text textComponent;
    [SerializeField] public SpriteRenderer[] Fogers;
    public GameObject cat; // Об'єкт кота
    public GameObject _dog; // Об'єкт собаки
    public float moveSpeed = 0.5f; // Швидкість руху кота
    public AudioSource openBut;
    public AudioSource dog;
    public AudioSource Win;
    public AudioSource Lose;
    public AudioSource main;
    public AudioSource bones;
    public Animator PauseAnim;
    [SerializeField] private bool SafeZone = false;

    void Start()
    {

        textComponent.text = GameManager.score.ToString();
        if (dogRandomizer() == 1 && !SafeZone)
        {
            bool allNeighborsEmpty = true;
            foreach (Button neighbor in neighbors)
            {
                ButtonPlaceScript neighborScript = neighbor.GetComponent<ButtonPlaceScript>();
                if (neighborScript != null && neighborScript.dogHere)
                {
                    allNeighborsEmpty = false;
                    break;
                }

            }
            if (allNeighborsEmpty)
            {
                dogHere = true;
                gameObject.transform.Find("Items").gameObject.SetActive(false);
                FindNeighbors();
            }

        }
        fillMap();
        gameObject.GetComponent<Button>().onClick.AddListener(Click);

        Button[] allButtons = FindObjectsOfType<Button>();

        foreach (Button button in allButtons)
        {
            GameObject butty = button.gameObject;
            SpriteRenderer childSpriteRenderer = FindChildSpriteRenderer(butty, "Fog");
            if (childSpriteRenderer != null)
            {
                switch (fogRandomizer())
                {
                    case 1:
                        childSpriteRenderer.flipX = false;
                        childSpriteRenderer.flipY = false;
                        break;
                    case 2:
                        childSpriteRenderer.flipX = true;
                        childSpriteRenderer.flipY = false;
                        break;
                    case 3:
                        childSpriteRenderer.flipX = false;
                        childSpriteRenderer.flipY = true;
                        break;
                    case 4:
                        childSpriteRenderer.flipX = true;
                        childSpriteRenderer.flipY = true;
                        break;
                }
            }
        }

    }

    private void fillMap()
    {
        if (!dogHere)
        {
            Transform imageTransform = gameObject.transform.Find("Items");
            if (imageTransform != null)
            {
                Image imageComponent = imageTransform.GetComponent<Image>();
                if (imageComponent != null && imageComponent.sprite == null)
                {
                    float randomRotation = Random.Range(0f, 360f); // Генеруємо випадковий кут повороту
                    imageComponent.transform.Rotate(0, 0, randomRotation);
                    switch (itemrandomizer())
                    {
                        case 1:
                            imageComponent.sprite = worldItems[0];
                            Debug.Log("Перший предмет");
                            imageComponent.color = new Color(1f, 1f, 1f, 1f);
                            break;
                        case 2:
                            imageComponent.sprite = worldItems[1];
                            Debug.Log("Другий предмет");
                            imageComponent.color = new Color(1f, 1f, 1f, 1f);
                            break;
                        case 3:
                            imageComponent.sprite = worldItems[2];
                            Debug.Log("Третій предмет");
                            imageComponent.color = new Color(1f, 1f, 1f, 1f);
                            break;
                        case 4:
                            imageComponent.sprite = worldItems[3];
                            Debug.Log("Четвертий предмет");
                            imageComponent.color = new Color(1f, 1f, 1f, 1f);
                            break;
                        case 5:
                            imageComponent.sprite = null;
                            Debug.Log("Пустий предмет");
                            break;
                        case 6:
                            imageComponent.sprite = null;
                            Debug.Log("Пустий предмет");
                            break;
                        case 7:
                            imageComponent.sprite = null;
                            Debug.Log("Пустий предмет");
                            break;
                        case 8:
                            imageComponent.sprite = null;
                            Debug.Log("Пустий предмет");
                            break;
                        case 9:
                            imageComponent.sprite = null;
                            Debug.Log("Пустий предмет");
                            break;
                        case 10:
                            imageComponent.sprite = null;
                            Debug.Log("Пустий предмет");
                            break;
                        case 11:
                            imageComponent.sprite = null;
                            Debug.Log("Пустий предмет");
                            break;
                        case 12:
                            imageComponent.sprite = null;
                            Debug.Log("Пустий предмет");
                            break;
                        case 13:
                            imageComponent.sprite = null;
                            Debug.Log("Пустий предмет");
                            break;
                    }
                }

            }
        }

    }
    private void FindNeighbors()
    {
        foreach (Button neighbor in neighbors)
        {
            if (neighbor.GetComponent<ButtonPlaceScript>().dogHere == false)
            {
                neighbor.transform.GetChild(2).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                neighbor.transform.GetChild(2).GetComponent<Image>().sprite = dogNeighboursItems[Random.Range(0, dogNeighboursItems.Length)];
            }

        }
    }
    private int dogRandomizer()
    {
        System.Random random = new System.Random();
        int randomNumber = random.Next(1, 9); // Генерує випадкове число від 1 до 11
        return randomNumber; // Повертає випадкове число
    }
    private int fogRandomizer()
    {
        System.Random random = new System.Random();
        int randomNumber = random.Next(1, 4); // Генерує випадкове число від 1 до 11
        return randomNumber; // Повертає випадкове число
    }
    private int itemrandomizer()
    {
        System.Random random = new System.Random();
        int randomNumber = random.Next(1, 10); // Генерує випадкове число від 1 до 11
        return randomNumber; // Повертає випадкове число
    }



    public void Click()
    {
        textComponent.text = GameManager.score.ToString();
        if (!visitedButtons.Contains(this))
        {
            visitedButtons.Add(this);
        }

        ButtonPlaceScript[] allButtons = FindObjectsOfType<ButtonPlaceScript>();

        foreach (ButtonPlaceScript button in allButtons)
        {
            button.DisableCorutines();
            button.GetComponent<Button>().interactable = false;
        }
        if (fogObject != null)
        {
            StartCoroutine(FadeOutFog(fogObject, 0.5f));
        }

        // Змінити колір попередніх активних кнопок назад
        foreach (Button button in activeButtons)
        {
            if (button != null && button.gameObject.activeInHierarchy)
            {
                button.interactable = false;
                button.transform.Find("Fog").gameObject.transform.Find("Green").gameObject.SetActive(false);
                button.StopCoroutine(Blink(button.transform.Find("Fog").gameObject.transform.Find("Green").GetComponent<SpriteRenderer>(), 1.0f));

                //Видалити кнопку зі списку відвіданих кнопок
                ButtonPlaceScript buttonScript = button.GetComponent<ButtonPlaceScript>();
                if (buttonScript != null)
                {
                    visitedButtons.Remove(buttonScript);
                }
            }
            else
            {
                if (button != null)
                {
                    button.StopCoroutine(Blink(button.transform.Find("Fog").gameObject.transform.Find("Green").GetComponent<SpriteRenderer>(), 1.0f));
                }
            }
        }

        if (dogHere)
        {

            Debug.Log("Кінець гри!");

        }

        // Очистити список активних кнопок
        activeButtons.Clear();

        Vector2 boxSize = new Vector2(1.0f, 1.0f); // Розмір коробки
        Collider2D[] neighbors = Physics2D.OverlapBoxAll(transform.position, boxSize, 0, buttonLayer);
        foreach (Collider2D neighbor in neighbors)
        {
            Vector2 direction = neighbor.transform.position - transform.position;
            if (Mathf.Abs(direction.x) > 0.5f && Mathf.Abs(direction.y) < 0.5f ||
                direction.y > 0.5f && Mathf.Abs(direction.x) < 0.5f ||
                direction.y < -0.5f && Mathf.Abs(direction.x) < 0.5f)
            {
                Button neighborButtonComponent = neighbor.GetComponent<Button>();
                ButtonPlaceScript neighborButton = neighborButtonComponent.GetComponent<ButtonPlaceScript>();

                // Перемістити туман подалі від вибраної кнопки та її сусідів
                Vector2 fogMoveDirection = (neighbor.transform.position - transform.position).normalized;
                Vector3 fogTargetPosition = neighbor.transform.position + (Vector3)fogMoveDirection * 1f; // 1f - це відстань, на яку переміщується туман
                GameObject fogNeighbor = neighbor.GetComponent<ButtonPlaceScript>().fogObject;

                // Додати кнопку до списку активних кнопок
                activeButtons.Add(neighborButtonComponent);
            }
            OnButtonClicked();
        }

        // Вимкнути поточну кнопку і змінити її колір
        gameObject.GetComponent<Button>().interactable = false;
        if (dogHere)
        {
            _dog.transform.position = gameObject.transform.position;
            _dog.gameObject.SetActive(true);
            StartCoroutine(HandleDogAndCatInteraction());
            return;
        }
        else
        {
            openBut.Play();
            gameObject.transform.Find("Image").GetComponent<Image>().color = new Color(0f, 0f, 1f, 0.2f); // синій колірs
        }


    }
    IEnumerator Blink(SpriteRenderer spriteRenderer, float speed)
    {
        float alpha = 0f;
        spriteRenderer.color = new Color(0f, 1f, 0f, alpha);
        float startTime = Time.time;
        while (true)
        {
            if (spriteRenderer != null)
            {
                alpha = Mathf.PingPong((Time.time - startTime) * speed, 1f) * 0.5f;
                spriteRenderer.color = new Color(0f, 1f, 0f, alpha);
            }
            yield return null;
        }
    }
    IEnumerator Blink(Image imageComponent, float speed)
    {
        float alpha = 0f;
        float startTime = Time.time;

        while (true)
        {
            alpha = Mathf.PingPong((Time.time - startTime) * speed, 1f) * 0.5f;
            imageComponent.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
    }
    public SpriteRenderer FindChildSpriteRenderer(GameObject parent, string childName)
    {
        Transform childTransform = parent.transform.Find(childName);
        if (childTransform != null)
        {
            return childTransform.GetComponent<SpriteRenderer>();
        }
        else
        {
            return null;
        }
    }
    public IEnumerator FadeOutFog(GameObject fog, float duration)
    {
        SpriteRenderer fogRenderer = fog.GetComponent<SpriteRenderer>();
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            // Перевіряємо, чи об'єкт fog все ще існує
            if (fog == null)
            {
                yield break; // Зупиняємо корутину, якщо об'єкт fog було видалено
            }

            float normalizedTime = t / duration;
            // зменшуємо прозорість спрайту туману
            fogRenderer.color = new Color(fogRenderer.color.r, fogRenderer.color.g, fogRenderer.color.b, 1f - normalizedTime);
            yield return null;
        }
    }
    public void OnButtonClicked()
    {
        // Починаємо корутину для плавного переміщення кота
        ButtonPlaceScript buttonScript = GetComponent<ButtonPlaceScript>();
        if (!buttonScript.dogHere)
        {
            StartCoroutine(MoveCatToPosition(transform.position));
        }
    }

    // Корутина для плавного переміщення кота
    IEnumerator MoveCatToPosition(Vector3 targetPosition)
    {
        // Включаємо анімацію руху
        foreach (GameObject neighborsButton in neighborsButtons())
        {

            neighborsButton.SetActive(false);
            StopCoroutine(Blink(neighborsButton.transform.GetComponent<SpriteRenderer>(), 1.0f));
        }

        // Включаємо анімацію руху
        cat.GetComponent<Animator>().SetBool("isMoving", true);

        // Обчислюємо напрямок до цільової позиції
        Vector3 direction = targetPosition - cat.transform.position;
        direction.Normalize();

        // Повертаємо кота в напрямку цільової позиції
        cat.transform.up = direction;
        cat.transform.eulerAngles = new Vector3(cat.transform.eulerAngles.x, 0, cat.transform.eulerAngles.z);

        while ((cat.transform.position - targetPosition).sqrMagnitude > 0.01f)
        {
            cat.transform.position = Vector3.MoveTowards(cat.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Вимикаємо анімацію руху
        cat.GetComponent<Animator>().SetBool("isMoving", false);
        OnCatStop();
    }
    public void OnCatStop()
    {
        fogObject.SetActive(false);
        Vector2 boxSize = new Vector2(1.0f, 1.0f); // Розмір коробки
        Collider2D[] neighbors = Physics2D.OverlapBoxAll(transform.position, boxSize, 0, buttonLayer);
        foreach (Collider2D neighbor in neighbors)
        {

            Vector2 direction = neighbor.transform.position - transform.position;
            if (Mathf.Abs(direction.x) > 0.5f && Mathf.Abs(direction.y) < 0.5f ||
                direction.y > 0.5f && Mathf.Abs(direction.x) < 0.5f ||
                direction.y < -0.5f && Mathf.Abs(direction.x) < 0.5f)
            {
                Button neighborButtonComponent = neighbor.GetComponent<Button>();
                ButtonPlaceScript neighborButton = neighborButtonComponent.GetComponent<ButtonPlaceScript>();
                Transform imageTransform = neighborButton.transform.Find("Image");
                neighborButtonComponent.interactable = true;
                if (imageTransform != null)
                {
                    Image imageComponent = imageTransform.GetComponent<Image>();
                    if (imageComponent != null)
                    {

                        _blinkCoroutines.Add(StartCoroutine(Blink(imageComponent, 1f)));

                    }
                }
                if (!visitedButtons.Contains(neighborButton))
                {
                    GameObject neighborFog = neighbor.transform.Find("Fog").gameObject;
                    GameObject neighborFogGreen = neighborFog.transform.Find("Green").gameObject; // зелений колір
                    neighborFogGreen.SetActive(true);
                    neighborFogGreen.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f, 0f);
                    StartCoroutine(Blink(neighborFogGreen.GetComponent<SpriteRenderer>(), 1.0f));
                }
            }

        }
        EnableNeighbors();
    }
    public void EnableNeighbors()
    {
        // foreach (Button neighbor in _neighbors)
        // {
        //     neighbor.interactable = true;
        //     Image imageComponent = neighbor.transform.Find("Image").GetComponent<Image>();
        //     if (imageComponent != null && imageComponent.enabled)
        //     {

        //         StartCoroutine(Blink(imageComponent, 1.0f));
        //     }
        //     _neighbor_ = true;
        // }
    }
    public List<GameObject> neighborsButtons()
    {
        List<GameObject> _neighborsButtons = new List<GameObject>();
        Vector2 boxSize = new Vector2(1.0f, 1.0f); // Розмір коробки
        Collider2D[] neighbors = Physics2D.OverlapBoxAll(transform.position, boxSize, 0, buttonLayer);
        foreach (Collider2D neighbor in neighbors)
        {
            Vector2 direction = neighbor.transform.position - transform.position;
            if (Mathf.Abs(direction.x) > 0.5f && Mathf.Abs(direction.y) < 0.5f ||
                direction.y > 0.5f && Mathf.Abs(direction.x) < 0.5f ||
                direction.y < -0.5f && Mathf.Abs(direction.x) < 0.5f)
            {
                Button neighborButtonComponent = neighbor.GetComponent<Button>();
                ButtonPlaceScript neighborButton = neighborButtonComponent.GetComponent<ButtonPlaceScript>();

                if (!visitedButtons.Contains(neighborButton))
                {

                    GameObject neighborFog = neighbor.transform.Find("Fog").gameObject;
                    GameObject neighborFogGreen = neighborFog.transform.Find("Green").gameObject; // зелений колір
                    _neighborsButtons.Add(neighborFogGreen);
                }

            }
        }
        return _neighborsButtons;
    }

    IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        // Плавно змінюємо прозорість CanvasGroup
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            // Обчислюємо нове значення прозорості
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t / duration);

            // Встановлюємо нове значення прозорості
            canvasGroup.alpha = alpha;

            yield return null;
        }

        // Встановлюємо кінцеве значення прозорості
        canvasGroup.alpha = endAlpha;
    }
    public void Countine()
    {
        PauseAnim.SetTrigger("Exit");
    }
    public void Pause()
    {
        PauseMenu.SetActive(true);
    }
    public void Restart()
    {
        Circle.GetComponent<Animator>().SetTrigger("Restart");

    }
    public void clearScore()
    {
        GameManager.score = 0;
    }
    public void OnWin()
    {
        GameManager.score++;

        ILeaderboard leaderboard = PlayGamesPlatform.Instance.CreateLeaderboard();
        leaderboard.id = GIC.leaderboard_singleplayer_leaderboard;
        leaderboard.LoadScores(ok =>
        {
            if (ok)
            {
                IScore currentUserScore = leaderboard.scores.FirstOrDefault(s => s.userID == Social.localUser.id);
                if (currentUserScore != null)
                {
                    int highScore = (int)currentUserScore.value;
                    if (GameManager.score > highScore)
                    {
                        Social.ReportScore(GameManager.score, GIC.leaderboard_singleplayer_leaderboard, success => { });
                    }
                }
                else
                {
                    Social.ReportScore(GameManager.score, GIC.leaderboard_singleplayer_leaderboard, success => { });
                }
            }
        });

        main.Stop();
        dog.Stop();
        Win.Play();
        WinMenu.SetActive(true);
    }
    public void ToMenu()
    {
        Circle.GetComponent<Animator>().SetTrigger("Exit");
    }
    IEnumerator MoveDogToCat(GameObject dog, GameObject cat, float duration)
    {
        float elapsedTime = 0;

        Vector3 dogStartPosition = dog.transform.position;
        Vector3 catPosition = cat.transform.position;

        while (elapsedTime < duration)
        {
            dog.transform.position = Vector3.Lerp(dogStartPosition, catPosition, elapsedTime / duration);

            // Змусити собаку дивитися на кота
            Vector2 direction = catPosition - dog.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            dog.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        dog.transform.position = catPosition;
    }
    IEnumerator HandleDogAndCatInteraction()
    {
        StartCoroutine(ScaleUp(_dog, 0.5f));
        yield return StartCoroutine(MoveDogToCat(_dog, cat, 0.5f));
        bones.Play();
        yield return new WaitForSeconds(0.5f);

        LoseMenu.SetActive(true);
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        int secondHighScore = PlayerPrefs.GetInt("SecondHighScore", 0);
        int thirdHighScore = PlayerPrefs.GetInt("ThirdHighScore", 0);

        if (GameManager.score > highScore)
        {
            // Зсуваємо поточні HighScore та SecondHighScore
            PlayerPrefs.SetInt("ThirdHighScore", secondHighScore);
            PlayerPrefs.SetInt("SecondHighScore", highScore);
            PlayerPrefs.SetInt("HighScore", GameManager.score);
        }
        else if (GameManager.score > secondHighScore)
        {
            // Зсуваємо поточний SecondHighScore
            PlayerPrefs.SetInt("ThirdHighScore", secondHighScore);
            PlayerPrefs.SetInt("SecondHighScore", GameManager.score);
        }
        else if (GameManager.score > thirdHighScore)
        {
            PlayerPrefs.SetInt("ThirdHighScore", GameManager.score);
        }

        GameManager.score = 0;
        dog.Play();
        Win.Stop();
        main.Stop();
        Lose.Play();
    }
    IEnumerator ScaleUp(GameObject dog, float duration)
    {
        Vector3 originalScale = dog.transform.localScale;
        Vector3 targetScale = new Vector3(10f, 8.55f, 0f); // Змініть це значення, щоб контролювати, наскільки великою стане собака

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            dog.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        dog.transform.localScale = targetScale;
    }

    private void DisableCorutines()
    {
        StopAllCoroutines();
    }

}
