using System.Collections;
using System.Linq;
using System.Text;
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
    [SerializeField] private GameObject unlockProgressComicHolder;
    [SerializeField] private TextMeshProUGUI unlockProgressComicText;
    [SerializeField] private TextMeshProUGUI unlockComicMemoryCostText;
    [SerializeField] private TextMeshProUGUI unlockComicMoneyCostText;
    [SerializeField] private TextMeshProUGUI memoryPlayerText;
    [SerializeField] private TextMeshProUGUI moneyPlayerText;
    [SerializeField] private TextMeshProUGUI panelNumberText;
    [SerializeField] private AudioClip buySucessSFX;
    [SerializeField] private AudioClip buyFailSFX;
    private float unlockMemoryCost;
    private float unlockMoneyCost;
    private float memoryCostMultiplier = 107f;
    private float moneyCostMultiplier = 21f;


    [Header("Navigation")]
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button backButton;

    [Header("Comic Data")]
    [SerializeField] private Sprite[] comicSprites;
    [SerializeField] private float[] comicNumbers;
    public float[] ComicNumbers => comicNumbers; // Expose comic numbers for other scripts
    private float currentComicNumber;

    private int currentPanelIndex = 0;

    [Header("Greeting")]
    [SerializeField] private GameObject greetingHolder;
    [SerializeField] private Button closeGreetingButton;
    [SerializeField] private Button openGreetingButton;
    [SerializeField] private TextMeshProUGUI greetingText;
    private int greetingTextIndex = 0;

    [Header("Time Capsule")]
    [SerializeField] private GameObject timeCapsuleHolder;
    [SerializeField] private TextMeshProUGUI timeCapsuleText;
    [SerializeField] private TextMeshProUGUI jermaTimeCapsuleText;
    [SerializeField] private Button timeCapsuleCloseButton;

    private void OnEnable()
    {
        DataPersister.InitializationComplete += Initalize;
        openComicMenu.onClick.AddListener(() => StartCoroutine(OpenComicMenu()));
        closeComicMenu.onClick.AddListener(() => StartCoroutine(CloseComicMenu()));
        forwardButton.onClick.AddListener(ShowNextPanel);
        backButton.onClick.AddListener(ShowPreviousPanel);
        unlockComicButton.onClick.AddListener(UnlockComicPanel);
        openGreetingButton.onClick.AddListener(() => GreetingsWindow());
        closeGreetingButton.onClick.AddListener(() => GreetingsDialogue());
        timeCapsuleCloseButton.onClick.AddListener(() => timeCapsuleHolder.SetActive(false));
    }

    private void OnDisable()
    {
        DataPersister.InitializationComplete -= Initalize;
        openComicMenu.onClick.RemoveListener(() => StartCoroutine(OpenComicMenu()));
        closeComicMenu.onClick.RemoveListener(() => StartCoroutine(CloseComicMenu()));
        forwardButton.onClick.RemoveListener(ShowNextPanel);
        backButton.onClick.RemoveListener(ShowPreviousPanel);
        unlockComicButton.onClick.RemoveListener(UnlockComicPanel);
        openGreetingButton.onClick.RemoveListener(() => GreetingsWindow());
        closeGreetingButton.onClick.RemoveListener(() => GreetingsDialogue());
        timeCapsuleCloseButton.onClick.RemoveListener(() => timeCapsuleHolder.SetActive(false));
    }

    private void Initalize()
    {
        Debug.Log("ComicsUI Initalize");
        InitializeComicData();
        ShowCurrentPanel();
        sFXManager = FindAnyObjectByType<SFXManager>();
    }
    IEnumerator OpenComicMenu()
    {
        UnlockComicsBasedOnProgress();

        comicMenuHolder.SetActive(true);
        DisplayTimeCapsule();

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

        if (!DataPersister.Instance.CurrentGameData.hasOpenedComics)
        {
            // If this is the first time opening comics, show the greeting window
            GreetingsWindow();
        }
    }

    private void GreetingsWindow()
    {
        greetingHolder.SetActive(true);
        DataPersister.Instance.CurrentGameData.hasOpenedComics = true;
        GreetingsDialogue();
    }

    private void GreetingsDialogue()
    {
        // Play SFX
        if (sFXManager != null && buttonSFX.Length > 0)
        {
            sFXManager.PlaySFX(buttonSFX[UnityEngine.Random.Range(0, buttonSFX.Length)]);
        }

        if (greetingTextIndex == 0)
        {
            greetingText.text = "Hi, I'm Jerma.";
            greetingTextIndex = 1;
            return;
        }
        if (greetingTextIndex == 1)
        {
            greetingText.text = "Time travel messes with the mind. I can help you remember.";
            greetingTextIndex = 2;
            return;
        }
        if (greetingTextIndex == 2)
        {
            greetingText.text = "Unlock comics to get bigger time capsules.";
            greetingTextIndex = 3;
            return;
        }
        if (greetingTextIndex == 3)
        {
            greetingText.text = "Time capsules save some of your items through time.";
            greetingTextIndex = 4;
            return;
        }
        if (greetingTextIndex == 4)
        {
            int unlockedComics = DataPersister.Instance.CurrentGameData.comicData.Count(kvp => kvp.Value.isUnlocked);
            int totalComics = comicNumbers.Length;
            float comicUnlockPercent = (float)unlockedComics / totalComics * 100f;
            greetingText.text = $"Comics unlocked {unlockedComics} of {totalComics} {comicUnlockPercent:F0}%)";
            greetingTextIndex = 5;
            return;
        }
        if (greetingTextIndex >= 5)
        {
            greetingHolder.SetActive(false);
            greetingTextIndex = 0; // Reset for next time
        }
    }

    private void DisplayTimeCapsule()
    {
        timeCapsuleHolder.SetActive(true);
        int unlockedComics = DataPersister.Instance.CurrentGameData.comicData.Count(kvp => kvp.Value.isUnlocked);
        int totalComics = comicNumbers.Length;
        float comicUnlockPercent = (float)unlockedComics / totalComics * 100f;
        jermaTimeCapsuleText.text = $"Hey! {unlockedComics} of {totalComics} comics. Keep unlocking comics to upgrade the time capsule.";
        timeCapsuleText.text = $"{comicUnlockPercent:F0}%";
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
        Debug.Log("ComicsUI InitializeComicData");
        // Verify array setup
        if (comicSprites == null || comicNumbers == null || comicSprites.Length == 0 || comicNumbers.Length == 0)
        {
            Debug.LogError("ComicsUI InitializeComicData - Comic arrays are not properly initialized in the inspector!");
            return;
        }

        if (comicSprites.Length != comicNumbers.Length)
        {
            Debug.LogError("ComicsUI InitializeComicData - ComicSprites and ComicNumbers arrays must be the same length!");
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

        // Unlock comics based on progress
        UnlockComicsBasedOnProgress();
    }
    private void UnlockComicsBasedOnProgress()
    {
        // Save the total comics count to GameData
        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            DataPersister.Instance.CurrentGameData.comicNumbersLength = comicNumbers.Length;
            Debug.Log($"Saved total comics count: {DataPersister.Instance.CurrentGameData.comicNumbersLength}");
        }

        var gameData = DataPersister.Instance.CurrentGameData;

        // Mission unlocks
        if (gameData.isMission1Complete) UnlockComic(0);
        if (gameData.isMission2Complete) UnlockComic(4);
        if (gameData.isMission3Complete) UnlockComic(8);
        if (gameData.isMission4Complete) UnlockComic(12);
        if (gameData.isMission5Complete) UnlockComic(16);
        if (gameData.isMission6Complete) UnlockComic(20);
        if (gameData.isMission7Complete) UnlockComic(24);
        if (gameData.isMission8Complete) UnlockComic(28);
        if (gameData.isMission9Complete) UnlockComic(32);


        // Mission 10 unlocks
        if (gameData.isMission10Complete)
        {
            UnlockComic(36);
            UnlockComic(44);
            UnlockComic(45);
            UnlockComic(46);
            UnlockComic(47);
        }

        // Lose comics
        if (gameData.hasLost)
        {
            for (int i = 40; i <= 43; i++)
            {
                UnlockComic(i);
            }
        }

        DebugUnlockStatus("UnlockComicsBasedOnProgress");

        // Save after unlocking
        DataPersister.Instance.SaveCurrentGame();

        // Refresh UI if showing comics
        if (comicMenuHolder.activeSelf)
        {
            ShowCurrentPanel();
        }
    }

    private void UnlockComic(float comicNumber)
    {
        Debug.Log("ComicsUI UnlockedComic");
        if (DataPersister.Instance == null || DataPersister.Instance.CurrentGameData == null)
        {
            Debug.LogError("ComicsUI UnlockComic - DataPersister not available to unlock comic");
            return;
        }

        var gameData = DataPersister.Instance.CurrentGameData;

        gameData.comicData[comicNumber] = new ComicData(comicNumber, true);
        Debug.Log($"ComicsUI UnlockComic - Unlocked comic {comicNumber}. isUnlocked = {IsComicUnlocked(comicNumber)}");

        DataPersister.Instance.SaveCurrentGame();
    }


    private void ShowCurrentPanel()
    {     
        Debug.Log("ComicsUI ShowCurrentPanel");
        if (comicSprites == null || comicNumbers == null ||
            comicSprites.Length == 0 || comicNumbers.Length == 0 ||
            comicSprites.Length != comicNumbers.Length)
        {
            Debug.LogError("Comic arrays are not properly initialized!");
            return;
        }

        if (currentPanelIndex < 0 || currentPanelIndex >= comicSprites.Length)
        {
            Debug.LogError($"ComicsUI ShowCurrentPanel - Invalid panel index: {currentPanelIndex}");
            return;
        }

        // Show current comic
        comicImage.sprite = comicSprites[currentPanelIndex];
        panelNumberText.text = $"Comic {comicNumbers[currentPanelIndex]}";

        // Show costs if locked
        if (IsComicUnlocked(currentPanelIndex))
        {
            unlockProgressComicHolder.SetActive(false);
            unlockComicHolder.SetActive(false);
        }
        else if (new[] { 0, 4, 8, 12, 16, 20, 24, 28, 32, 36, 40, 41, 42, 43, 44, 45, 46, 47 }.Contains(currentPanelIndex))
        {
            unlockProgressComicHolder.SetActive(true);
            unlockComicHolder.SetActive(false);
            
           string progressGoal = "?";
            if (currentPanelIndex == 0)
            {
                progressGoal = "level 1";
            }
            if (currentPanelIndex == 4)
            {
                progressGoal = "level 2";
            }
            else if (currentPanelIndex == 8)
            {
                progressGoal = "level 3";
            }
            else if (currentPanelIndex == 12)
            {
                progressGoal = "level 4";
            }
            else if (currentPanelIndex == 16)
            {
                progressGoal = "level 5";
            }
            else if (currentPanelIndex == 20)
            {
                progressGoal = "level 6";
            }
            else if (currentPanelIndex == 24)
            {
                progressGoal = "level 7";
            }
            else if (currentPanelIndex == 28)
            {
                progressGoal = "level 8";
            }
            else if (currentPanelIndex == 32)
            {
                progressGoal = "level 9";
            }
            else if (currentPanelIndex == 36)
            {
                progressGoal = "level 10";
            }
            else if (currentPanelIndex >= 40 && currentPanelIndex <= 43)
            {
                progressGoal = "losing";
            }
            else if (currentPanelIndex >= 44 && currentPanelIndex <= 47)
            {
                progressGoal = "winning";
            }

            unlockProgressComicText.text = $"Unlocked by completing {progressGoal}";
        }
        else
        {
            unlockProgressComicHolder.SetActive(false);
            unlockComicHolder.SetActive(true);
            unlockMemoryCost = Mathf.RoundToInt(memoryCostMultiplier * currentPanelIndex);
            unlockMoneyCost = Mathf.RoundToInt(moneyCostMultiplier * currentPanelIndex);
            unlockComicMemoryCostText.text = $"{unlockMemoryCost}";
            unlockComicMoneyCostText.text = $"{unlockMoneyCost}";
            UpdatePlayerCurrencyUI();
        }

        bool hasLost = DataPersister.Instance.CurrentGameData.hasLost;

        // Lock should be INACTIVE if comic is unlocked. Unlocked state is determined in InitializeComicData

        DebugUnlockStatus("ShowCurrentPanel");

        lockImage.SetActive(!IsComicUnlocked(currentPanelIndex));

    }

    private void ShowNextPanel()
    {
        sFXManager.PlaySFX(buttonSFX[UnityEngine.Random.Range(0, buttonSFX.Length)]);

        if (currentPanelIndex < comicSprites.Length - 1)
        {
            currentPanelIndex++;
        }
        else
        {
            currentPanelIndex = 0;
        }

        ShowCurrentPanel();
    }

    private void ShowPreviousPanel()
    {
        sFXManager.PlaySFX(buttonSFX[UnityEngine.Random.Range(0, buttonSFX.Length)]);

        if (currentPanelIndex > 0)
        {
            currentPanelIndex--;
        }
        else
        {
            currentPanelIndex = comicSprites.Length - 1;
        }

        ShowCurrentPanel();
    }

    private void UnlockComicPanel()
    {
        currentComicNumber = currentPanelIndex;

        // Check if this comic is unlocked by missions
        var gameData = DataPersister.Instance.CurrentGameData;
        bool isMissionUnlocked = (currentComicNumber == 0 && gameData.isMission1Complete) ||
                                (currentComicNumber == 4 && gameData.isMission2Complete) ||
                                (currentComicNumber == 8 && gameData.isMission3Complete) ||
                                (currentComicNumber == 12 && gameData.isMission4Complete) ||
                                (currentComicNumber == 16 && gameData.isMission5Complete) ||
                                (currentComicNumber == 20 && gameData.isMission6Complete) ||
                                (currentComicNumber == 24 && gameData.isMission7Complete) ||
                                (currentComicNumber == 28 && gameData.isMission8Complete) ||
                                (currentComicNumber == 32 && gameData.isMission9Complete) ||
                                (currentComicNumber == 36 && gameData.isMission10Complete) ||
                                (currentComicNumber >= 40 && currentComicNumber <= 43 && gameData.hasLost) || // Lose comics
                                (currentComicNumber >= 44 && currentComicNumber <= 47 && gameData.isMission10Complete);

        if (isMissionUnlocked)
        {
            Debug.Log($"ComicsUI UnlockComicPanel - Comic {currentComicNumber} is unlocked by mission progress and cannot be purchased");
            return;
        }

        unlockMemoryCost = Mathf.RoundToInt(memoryCostMultiplier * currentComicNumber);
        unlockMoneyCost = Mathf.RoundToInt(moneyCostMultiplier * currentComicNumber);

        // Check if already unlocked
        if (DataPersister.Instance.CurrentGameData.comicData.ContainsKey(currentComicNumber) &&
            DataPersister.Instance.CurrentGameData.comicData[currentComicNumber].isUnlocked)
        {
            Debug.Log($"ComicsUI UnlockComicPanel - Comic {currentComicNumber} is already unlocked");
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
            InitializeComicData();
            ShowCurrentPanel();

            // Play success SFX
            if (sFXManager != null)
            {
                sFXManager.PlaySFX(buySucessSFX);
            }
        }
        else
        {
            Debug.Log("ComicsUI UnlockComicPanel - Not enough money to unlock this comic");
            // Play failure SFX
            if (sFXManager != null)
            {
                sFXManager.PlaySFX(buyFailSFX);
            }
        }
    }
    private bool IsComicUnlocked(float comicNumber)
    {
        if (DataPersister.Instance.CurrentGameData.comicData.TryGetValue(comicNumber, out ComicData data))
        {
            return data.isUnlocked;
        }
        return false;
    }

    private void UpdatePlayerCurrencyUI()
    {
        if (DataPersister.Instance == null || DataPersister.Instance.CurrentGameData == null)
        {
            Debug.LogError("ComicsUI UpdatePlayerCurrencyUI - DataPersister or CurrentGameData is not initialized.");
            return;
        }
        var gameData = DataPersister.Instance.CurrentGameData;
        memoryPlayerText.text = $"{gameData.playerData[0].playerMemoryScore:F0}";
        moneyPlayerText.text = $"{gameData.totalMoney:F0}";
    }
    private void DebugUnlockStatus(string method)
    {
        var gameData = DataPersister.Instance.CurrentGameData;

        Debug.Log($"# ComicsUI {method} - Mission 1: {gameData.isMission1Complete}, Comic 0 (Mission 1): {IsComicUnlocked(0)}");
        Debug.Log($"# ComicsUI {method} - Mission 2: {gameData.isMission2Complete}, Comic 4 (Mission 2): {IsComicUnlocked(4)}");
        Debug.Log($"# ComicsUI {method} - Mission 3: {gameData.isMission3Complete}, Comic 8 (Mission 3): {IsComicUnlocked(8)}");
        Debug.Log($"# ComicsUI {method} - Mission 4: {gameData.isMission4Complete}, Comic 12 (Mission 4): {IsComicUnlocked(12)}");
        Debug.Log($"# ComicsUI {method} - Mission 5: {gameData.isMission5Complete}, Comic 16 (Mission 5): {IsComicUnlocked(16)}");
        Debug.Log($"# ComicsUI {method} - Mission 6: {gameData.isMission6Complete}, Comic 20 (Mission 6): {IsComicUnlocked(20)}");
        Debug.Log($"# ComicsUI {method} - Mission 7: {gameData.isMission7Complete}, Comic 24 (Mission 7): {IsComicUnlocked(24)}");
        Debug.Log($"# ComicsUI {method} - Mission 8: {gameData.isMission8Complete}, Comic 28 (Mission 8): {IsComicUnlocked(28)}");
        Debug.Log($"# ComicsUI {method} - Mission 9: {gameData.isMission9Complete}, Comic 32 (Mission 9): {IsComicUnlocked(32)}");
        Debug.Log($"# ComicsUI {method} - Mission 10: {gameData.isMission10Complete}, Comic 36 (Mission 10): {IsComicUnlocked(36)}");
        Debug.Log($"# ComicsUI {method} - Mission 10: {gameData.isMission10Complete}, Comic 36 (Mission 10): {IsComicUnlocked(36)} Comic 44 {IsComicUnlocked(44)} Comic 45 {IsComicUnlocked(45)} Comic 46 {IsComicUnlocked(46)} Comic 47 {IsComicUnlocked(47)}");
        Debug.Log($"# ComicsUI {method} - hasLost: {gameData.hasLost}, Comic 40: {IsComicUnlocked(40)} Comic 41 {IsComicUnlocked(41)} Comic 42 {IsComicUnlocked(42)} Comic 43 {IsComicUnlocked(43)}");
    }

}

