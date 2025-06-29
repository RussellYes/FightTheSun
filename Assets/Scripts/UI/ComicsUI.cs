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

    [Header("Greeting")]
    [SerializeField] private GameObject greetingHolder;
    [SerializeField] private Button closeGreetingButton;
    [SerializeField] private Button openGreetingButton;
    [SerializeField] private TextMeshProUGUI greetingText;
    private int greetingTextIndex = 0;

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
        else
        {
            // Otherwise, just show the comic menu
            comicHolder.SetActive(true);
        }
        GreetingsWindow();
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

        // Unlock comics based on progress
        UnlockComicsBasedOnProgress();
    }

    private void UnlockComicsBasedOnProgress()
    {
        var gameData = DataPersister.Instance.CurrentGameData;

        Debug.Log($"ComicsUI UnlockComicsBasedOnProgress BeforeCheck - Mission 1: {gameData.isMission1Complete}, Comic 0 (Mission 1): {IsComicUnlocked(0)} hasLost: {gameData.hasLost}");
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
        Debug.Log($"ComicsUI UnlockComicsBasedOnProgress AfterCheck - Mission 1: {gameData.isMission1Complete}, Comic 0 (Mission 1): {IsComicUnlocked(0)} hasLost: {gameData.hasLost}");

        // Save after unlocking
        DataPersister.Instance.SaveCurrentGame();
    }

    private void UnlockComic(float comicNumber)
    {
        if (DataPersister.Instance == null || DataPersister.Instance.CurrentGameData == null)
        {
            Debug.LogError("DataPersister not available to unlock comic");
            return;
        }

        var gameData = DataPersister.Instance.CurrentGameData;

        if (!gameData.comicData.ContainsKey(comicNumber))
        {
            Debug.Log($"Creating new comic entry for {comicNumber}");
            gameData.comicData[comicNumber] = new ComicData(comicNumber, true);
        }
        else if (!gameData.comicData[comicNumber].isUnlocked)
        {
            Debug.Log($"Unlocking comic {comicNumber}");
            gameData.comicData[comicNumber].isUnlocked = true;
        }
        else
        {
            Debug.Log($"Comic {comicNumber} was already unlocked");
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
        bool isMissionUnlocked = false;

        // Check unlocked status if DataPersister is available
        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            var gameData = DataPersister.Instance.CurrentGameData;

            // Check if unlocked in comicData
            isUnlocked = gameData.comicData.ContainsKey(currentComicNumber) && gameData.comicData[currentComicNumber].isUnlocked;


            // Check if this comic is unlocked by progress (and shouldn't be purchasable)
            isMissionUnlocked = (currentComicNumber == 0 && gameData.isMission1Complete) ||
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
                                (currentComicNumber >= 44 && currentComicNumber <= 47 && gameData.isMission10Complete); // Win comics

            if (!isUnlocked && !isMissionUnlocked)
            {
                unlockMemoryCost = Mathf.RoundToInt(memoryCostMultiplier * currentComicNumber);
                unlockMoneyCost = Mathf.RoundToInt(moneyCostMultiplier * currentComicNumber);
                unlockComicMemoryCostText.text = $"{unlockMemoryCost}";
                unlockComicMoneyCostText.text = $"{unlockMoneyCost}";
            }
        }
        // Update UI elements
        lockImage.SetActive(!isUnlocked);
        unlockComicHolder.SetActive(!isUnlocked);
        DebugComicUnlocks();

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
            Debug.Log($"Comic {currentComicNumber} is unlocked by mission progress and cannot be purchased");
            return;
        }

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
            Debug.Log("Not enough money to unlock this comic");
            // Play failure SFX
            if (sFXManager != null)
            {
                sFXManager.PlaySFX(buyFailSFX);
            }
        }
    }


    [ContextMenu("Debug Comic Unlocks")]
    public void DebugComicUnlocks()
    {
        if (DataPersister.Instance == null || DataPersister.Instance.CurrentGameData == null)
        {
            Debug.LogError("DataPersister not initialized!");
            return;
        }

        var gameData = DataPersister.Instance.CurrentGameData;
        StringBuilder debugOutput = new StringBuilder();
        debugOutput.AppendLine("===== MISSION COMPLETION STATUS =====");
        debugOutput.AppendLine($"Mission 1: {gameData.isMission1Complete}");
        debugOutput.AppendLine($"Mission 2: {gameData.isMission2Complete}");
        debugOutput.AppendLine($"Mission 3: {gameData.isMission3Complete}");
        debugOutput.AppendLine($"Mission 4: {gameData.isMission4Complete}");
        debugOutput.AppendLine($"Mission 5: {gameData.isMission5Complete}");
        debugOutput.AppendLine($"Mission 6: {gameData.isMission6Complete}");
        debugOutput.AppendLine($"Mission 7: {gameData.isMission7Complete}");
        debugOutput.AppendLine($"Mission 8: {gameData.isMission8Complete}");
        debugOutput.AppendLine($"Mission 9: {gameData.isMission9Complete}");
        debugOutput.AppendLine($"Mission 10: {gameData.isMission10Complete}");
        debugOutput.AppendLine($"Has Lost: {gameData.hasLost}");
        debugOutput.AppendLine("");

        debugOutput.AppendLine("===== COMIC UNLOCK STATUS =====");
        debugOutput.AppendLine($"Comic 0 (Mission 1): {IsComicUnlocked(0)}");
        debugOutput.AppendLine($"Comic 4 (Mission 2): {IsComicUnlocked(4)}");
        debugOutput.AppendLine($"Comic 8 (Mission 3): {IsComicUnlocked(8)}");
        debugOutput.AppendLine($"Comic 12 (Mission 4): {IsComicUnlocked(12)}");
        debugOutput.AppendLine($"Comic 16 (Mission 5): {IsComicUnlocked(16)}");
        debugOutput.AppendLine($"Comic 20 (Mission 6): {IsComicUnlocked(20)}");
        debugOutput.AppendLine($"Comic 24 (Mission 7): {IsComicUnlocked(24)}");
        debugOutput.AppendLine($"Comic 28 (Mission 8): {IsComicUnlocked(28)}");
        debugOutput.AppendLine($"Comic 32 (Mission 9): {IsComicUnlocked(32)}");
        debugOutput.AppendLine($"Comic 36 (Mission 10): {IsComicUnlocked(36)}");
        debugOutput.AppendLine($"Comic 40 (Lose 1): {IsComicUnlocked(40)}");
        debugOutput.AppendLine($"Comic 41 (Lose 2): {IsComicUnlocked(41)}");
        debugOutput.AppendLine($"Comic 42 (Lose 3): {IsComicUnlocked(42)}");
        debugOutput.AppendLine($"Comic 43 (Lose 4): {IsComicUnlocked(43)}");
        debugOutput.AppendLine($"Comic 44 (Win 1): {IsComicUnlocked(44)}");
        debugOutput.AppendLine($"Comic 45 (Win 2): {IsComicUnlocked(45)}");
        debugOutput.AppendLine($"Comic 46 (Win 3): {IsComicUnlocked(46)}");
        debugOutput.AppendLine($"Comic 47 (Win 4): {IsComicUnlocked(47)}");

        Debug.Log(debugOutput.ToString());
    }

    private bool IsComicUnlocked(float comicNumber)
    {
        if (DataPersister.Instance.CurrentGameData.comicData.TryGetValue(comicNumber, out ComicData data))
        {
            return data.isUnlocked;
        }
        return false;
    }

}

