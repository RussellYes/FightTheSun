using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class ComicsUI : MonoBehaviour
{
    private SFXManager sFXManager;

    [Header("Menu Controls")]
    [SerializeField] private GameObject comicMenuHolder;
    [SerializeField] private GameObject comicHolder;
    [SerializeField] private Button openComicMenu;
    [SerializeField] private Button closeComicMenu;
    [SerializeField] private float uIOpenCloseLerpTime = 1f;
    [SerializeField] private AudioClip[] comicMenuOpenCloseSFX;
    [SerializeField] private AudioClip[] buttonSFX;
    [SerializeField] private AudioClip[] buttonFailSFX;

    [Header("Comic Display")]
    [SerializeField] private Image comicImage;
    [SerializeField] private GameObject lockImage;
    [SerializeField] private Button unlockComicButton;
    [SerializeField] private GameObject unlockComicHolder;
    [SerializeField] private TextMeshProUGUI unlockComicMemoryCostText;
    [SerializeField] private TextMeshProUGUI unlockComicMoneyCostText;
    [SerializeField] private TextMeshProUGUI panelNumberText;
    [SerializeField] private AudioClip buySucessSFX;
    [SerializeField] private AudioClip buyFailSFX;
    private float unlockMemoryCost;
    private float unlockMoneyCost;
    private float memoryCostMultiplier = 1003f;
    private float moneyCostMultiplier = 107f;


    [Header("Navigation")]
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button backButton;

    [Header("Comic Data")]
    [SerializeField] private Sprite[] comicSprites;
    [SerializeField] private float[] comicNumbers;
    private float currentComicNumber;

    private int currentPanelIndex = 0;

    private void OnEnable()
    {
        DataPersister.InitializationComplete += Initalize;
        openComicMenu.onClick.AddListener(() => StartCoroutine(OpenComicMenu()));
        closeComicMenu.onClick.AddListener(() => StartCoroutine(CloseComicMenu()));
        forwardButton.onClick.AddListener(ShowNextPanel);
        backButton.onClick.AddListener(ShowPreviousPanel);
        unlockComicButton.onClick.AddListener(UnlockComicPanel);
    }

    private void OnDisable()
    {
        DataPersister.InitializationComplete -= Initalize;
        openComicMenu.onClick.RemoveListener(() => StartCoroutine(OpenComicMenu()));
        closeComicMenu.onClick.RemoveListener(() => StartCoroutine(CloseComicMenu()));
        forwardButton.onClick.RemoveListener(ShowNextPanel);
        backButton.onClick.RemoveListener(ShowPreviousPanel);
        unlockComicButton.onClick.RemoveListener(UnlockComicPanel);
    }

    private void Initalize()
    {
        InitializeComicData();
        ShowCurrentPanel();
        sFXManager = FindAnyObjectByType<SFXManager>();
    }
    IEnumerator OpenComicMenu()
    {
        comicMenuHolder.SetActive(true);

        //Play SFX
        if (sFXManager != null && comicMenuOpenCloseSFX.Length > 0)
        {
            sFXManager.PlaySFX(comicMenuOpenCloseSFX[UnityEngine.Random.Range(0, comicMenuOpenCloseSFX.Length)]);
        }

        // without delay, move comicHolder up 2000 on the y axis.
        RectTransform rectTransform = comicHolder.GetComponent<RectTransform>();
        Vector3 originalPosition = rectTransform.localPosition;
        Vector3 startPosition = originalPosition + new Vector3(0, 2000, 0);
        rectTransform.localPosition = startPosition;

        // lerp comicHolder's position from its +2000 y axis position to its original position over UIOpenCloseLerpTime seconds.
        float elapsedTime = 0f;
        while (elapsedTime < uIOpenCloseLerpTime)
        {
            rectTransform.localPosition = Vector3.Lerp(startPosition, originalPosition, elapsedTime / uIOpenCloseLerpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rectTransform.localPosition = originalPosition;
    }

    IEnumerator CloseComicMenu()
    {
        //Play SFX
        if (sFXManager != null && comicMenuOpenCloseSFX.Length > 0)
        {
            sFXManager.PlaySFX(comicMenuOpenCloseSFX[UnityEngine.Random.Range(0, comicMenuOpenCloseSFX.Length)]);
        }

        // lerp comicHolder's position from its original position to +2000 y over UIOpenCloseLerpTime seconds.
        RectTransform rectTransform = comicHolder.GetComponent<RectTransform>();
        Vector3 originalPosition = rectTransform.localPosition;
        Vector3 endPosition = originalPosition + new Vector3(0, 2000, 0);

        float elapsedTime = 0f;
        while (elapsedTime < uIOpenCloseLerpTime)
        {
            rectTransform.localPosition = Vector3.Lerp(originalPosition, endPosition, elapsedTime / uIOpenCloseLerpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rectTransform.localPosition = endPosition;

        // without delay, move comicHolder down 2000 on the y axis back to its original position.
        rectTransform.localPosition = originalPosition;

        comicMenuHolder.SetActive(false);
    }

    private void InitializeComicData()
    {
        // Verify array setup
        if (comicSprites == null || comicNumbers == null || comicSprites.Length == 0 || comicNumbers.Length == 0)
        {
            Debug.LogError("Comic arrays are not properly initialized in the inspector!");
            return;
        }

        if (comicSprites.Length != comicNumbers.Length)
        {
            Debug.LogError("ComicSprites and ComicNumbers arrays must be the same length!");
            return;
        }

        // Initialize data if available, otherwise use default locked state
        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            foreach (float number in comicNumbers)
            {
                if (!DataPersister.Instance.CurrentGameData.comicData.ContainsKey(number))
                {
                    DataPersister.Instance.CurrentGameData.comicData[number] = new ComicData(number, false);
                }
            }
        }
    }

    private void ShowCurrentPanel()
    {
        

        if (comicSprites == null || comicNumbers == null ||
            comicSprites.Length == 0 || comicNumbers.Length == 0 ||
            comicSprites.Length != comicNumbers.Length)
        {
            Debug.LogError("Comic arrays are not properly initialized!");
            return;
        }

        if (currentPanelIndex < 0 || currentPanelIndex >= comicSprites.Length)
        {
            Debug.LogError($"Invalid panel index: {currentPanelIndex}");
            return;
        }

        // Show current comic
        comicImage.sprite = comicSprites[currentPanelIndex];
        panelNumberText.text = $"Comic {comicNumbers[currentPanelIndex]}";

        // Show costs if locked
        currentComicNumber = comicNumbers[currentPanelIndex];
        bool isUnlocked = false;

        // Check unlocked status if DataPersister is available
        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            isUnlocked = DataPersister.Instance.CurrentGameData.comicData.ContainsKey(currentComicNumber) &&
                        DataPersister.Instance.CurrentGameData.comicData[currentComicNumber].isUnlocked;

            if (!isUnlocked)
            {
                unlockMemoryCost = Mathf.RoundToInt(memoryCostMultiplier * currentComicNumber);
                unlockMoneyCost = Mathf.RoundToInt(moneyCostMultiplier * currentComicNumber);
                unlockComicMemoryCostText.text = $"{unlockMemoryCost}";
                unlockComicMoneyCostText.text = $"{unlockMoneyCost}";
            }
        }
        // Update UI elements
        lockImage.SetActive(!isUnlocked);

        UpdateNavigationButtons();
    }

    private void UpdateNavigationButtons()
    {
        backButton.interactable = currentPanelIndex > 0;
        forwardButton.interactable = currentPanelIndex < comicSprites.Length - 1;
    }

    private void ShowNextPanel()
    {
        if (currentPanelIndex < comicSprites.Length - 1)
        {
            currentPanelIndex++;
            ShowCurrentPanel();
            sFXManager.PlaySFX(buttonSFX[UnityEngine.Random.Range(0, buttonSFX.Length)]);
        }
        else
        {
            sFXManager.PlaySFX(buttonFailSFX[UnityEngine.Random.Range(0, buttonFailSFX.Length)]);
        }
    }

    private void ShowPreviousPanel()
    {
        if (currentPanelIndex > 0)
        {
            currentPanelIndex--;
            ShowCurrentPanel();
            sFXManager.PlaySFX(buttonSFX[UnityEngine.Random.Range(0, buttonSFX.Length)]);
        }
        else
        {
            sFXManager.PlaySFX(buttonFailSFX[UnityEngine.Random.Range(0, buttonFailSFX.Length)]);
        }
    }

    private void UnlockComicPanel()
    {
        currentComicNumber = comicNumbers[currentPanelIndex];
        unlockMemoryCost = Mathf.RoundToInt(memoryCostMultiplier * currentComicNumber);
        unlockMoneyCost = Mathf.RoundToInt(moneyCostMultiplier * currentComicNumber);

        // Check if already unlocked
        if (DataPersister.Instance.CurrentGameData.comicData.ContainsKey(currentComicNumber) &&
            DataPersister.Instance.CurrentGameData.comicData[currentComicNumber].isUnlocked)
        {
            Debug.Log($"Comic {currentComicNumber} is already unlocked");
            return;
        }

        // Check if player has enough currency
        if (DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore >= unlockMemoryCost && DataPersister.Instance.CurrentGameData.totalMoney >= unlockMoneyCost)
        {
            // Deduct costs
            DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore -= unlockMemoryCost;
            DataPersister.Instance.CurrentGameData.totalMoney -= unlockMoneyCost;

            // Unlock comic
            if (!DataPersister.Instance.CurrentGameData.comicData.ContainsKey(currentComicNumber))
            {
                DataPersister.Instance.CurrentGameData.comicData[currentComicNumber] = new ComicData(currentComicNumber, true);
            }
            else
            {
                DataPersister.Instance.CurrentGameData.comicData[currentComicNumber].isUnlocked = true;
            }

            // Save changes
            DataPersister.Instance.SaveCurrentGame();

            // Update UI
            ShowCurrentPanel();

            // Play success SFX
            if (sFXManager != null)
            {
                sFXManager.PlaySFX(buySucessSFX);
            }
        }
        else
        {
            Debug.Log("Not enough money to unlock this comic");
            // Play failure SFX
            if (sFXManager != null)
            {
                sFXManager.PlaySFX(buyFailSFX);
            }
        }
    }

}