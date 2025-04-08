using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// This script displays the end screen and gives the player a choice to save their score.

public class EndConditionsUI : MonoBehaviour
{
    private ScoreManager scoreManager;
    private GameManager gameManager;
    private SFXManager sFXManager;
    private PlayerStatsManager playerStatsManager;

    public static Action EndConditionUIScoreChoiceEvent;
    public static Action reviveEvent;

    [Header("Winning UI")]
    [SerializeField] private bool isWin;
    [SerializeField] private Image winBackground;
    [SerializeField] private Sprite winSprite;
    [SerializeField] private TextMeshProUGUI endText;

    [Header("New Score UI")]
    [SerializeField] private Button newScoreSaveButtonFront;
    [SerializeField] private Button newScoreSaveButtonBack;
    [SerializeField] private TextMeshProUGUI newMoneyText;
    [SerializeField] private GameObject moneyTextShineBar;
    [SerializeField] private TextMeshProUGUI newObstaclesDestroyedText;
    [SerializeField] private GameObject obstaclesDestroyedShineBar;
    [SerializeField] private TextMeshProUGUI newTimeText;
    [SerializeField] private GameObject timeShineBar;
    [SerializeField] private AudioClip[] textMovementSfx;
    [SerializeField] private AudioClip buttonAppearSFX;
    [SerializeField] private float textAppearDelay;
    [SerializeField] private float sFXDelay;

    [Header("Old Score UI")]
    [SerializeField] private Button oldScoreSaveButtonFront;
    [SerializeField] private Button oldScoreSaveButtonBack;
    [SerializeField] private TextMeshProUGUI oldMoneyText;
    [SerializeField] private TextMeshProUGUI oldObstaclesDestroyedText;
    [SerializeField] private TextMeshProUGUI oldTimeText;

    [Header("Lose UI")]
    [SerializeField] private Sprite loseSprite;
    [SerializeField] private Color loseColor;
    [SerializeField] private Color memoryUpgradeColor;
    [SerializeField] private TextMeshProUGUI loseText;
    [SerializeField] private TextMeshProUGUI totalGameTimeText;
    [SerializeField] private TextMeshProUGUI totalObstaclesDestroyedText;
    [SerializeField] private TextMeshProUGUI totalMoneyText;
    [SerializeField] private TextMeshProUGUI lineText;

    private float memoryScore;

    [Header("Upgrade Buttons")]
    [SerializeField] private GameObject upgradeButtonHolder;
    [SerializeField] private TextMeshProUGUI memoryScoreText;
    public Button engineeringButton;
    public Button pilotingButton;
    public Button mechanicsButton;
    public Button miningButton;
    public Button roboticsButton;
    public Button combatButton;
    [SerializeField] private TextMeshProUGUI engineeringText;
    [SerializeField] private TextMeshProUGUI pilotingText;
    [SerializeField] private TextMeshProUGUI mechanicsText;
    [SerializeField] private TextMeshProUGUI miningText;
    [SerializeField] private TextMeshProUGUI roboticsText;
    [SerializeField] private TextMeshProUGUI combatText;
    [SerializeField] private TextMeshProUGUI engineeringCostText;
    [SerializeField] private TextMeshProUGUI pilotingCostText;
    [SerializeField] private TextMeshProUGUI mechanicsCostText;
    [SerializeField] private TextMeshProUGUI miningCostText;
    [SerializeField] private TextMeshProUGUI roboticsCostText;
    [SerializeField] private TextMeshProUGUI combatCostText;
    private float engineeringCost;
    private float pilotingCost;
    private float mechanicsCost;
    private float miningCost;
    private float roboticsCost;
    private float combatCost;

    [SerializeField] private GameObject salesGameObject;
    [SerializeField] private TextMeshProUGUI salesText;
    public Button saveButtonFront;
    [SerializeField] private GameObject saveButtonHolder;


    private void Awake()
    {
        scoreManager = FindAnyObjectByType<ScoreManager>();
        gameManager = FindAnyObjectByType<GameManager>();
        sFXManager = FindAnyObjectByType<SFXManager>();
        playerStatsManager = FindAnyObjectByType<PlayerStatsManager>();

        HideUI();
    }

    private void Start()
    {
        engineeringButton.onClick.AddListener(() => { BuyEngineering(); });
        pilotingButton.onClick.AddListener(() => { BuyPiloting(); });
        mechanicsButton.onClick.AddListener(() => { BuyMechanics(); });
        miningButton.onClick.AddListener(() => { BuyMining(); });
        roboticsButton.onClick.AddListener(() => { BuyRobotics(); });
        combatButton.onClick.AddListener(() => { BuyCombat(); });
        saveButtonFront.onClick.AddListener(() => { Revive(); });
    }

    private void OnEnable()
    {
        Debug.Log("EndConditionsUI subscribed to EndGameEvent");
        GameManager.EndGameEvent += EndGame;
        ScoreManager.SavedTotalEvent += LoadMainMenuScene;
    }

    private void OnDisable()
    {
        Debug.Log("EndConditionsUI unsubscribed from EndGameEvent");
        GameManager.EndGameEvent -= EndGame;
        ScoreManager.SavedTotalEvent -= LoadMainMenuScene;
    }

    private void HideUI()
    {
        // Initially hide all text elements and their shine bars
        endText.gameObject.SetActive(false);
        newMoneyText.gameObject.SetActive(false);
        if (moneyTextShineBar != null) moneyTextShineBar.SetActive(false);
        newObstaclesDestroyedText.gameObject.SetActive(false);
        if (obstaclesDestroyedShineBar != null) obstaclesDestroyedShineBar.SetActive(false);
        newTimeText.gameObject.SetActive(false);
        if (timeShineBar != null) timeShineBar.SetActive(false);
        oldMoneyText.gameObject.SetActive(false);
        oldObstaclesDestroyedText.gameObject.SetActive(false);
        oldTimeText.gameObject.SetActive(false);

        loseText.gameObject.SetActive(false);
        totalGameTimeText.gameObject.SetActive(false);
        totalObstaclesDestroyedText.gameObject.SetActive(false);
        totalMoneyText.gameObject.SetActive(false);
        lineText.gameObject.SetActive(false);
        memoryScoreText.gameObject.SetActive(false);

        upgradeButtonHolder.gameObject.SetActive(false);
        saveButtonFront.gameObject.SetActive(false);
        saveButtonHolder.gameObject.SetActive(false);

        newScoreSaveButtonFront.gameObject.SetActive(false);
        newScoreSaveButtonBack.gameObject.SetActive(false);
        oldScoreSaveButtonFront.gameObject.SetActive(false);
        oldScoreSaveButtonBack.gameObject.SetActive(false);
    }

    private void EndGame(bool isWin)
    {
        if (scoreManager != null && gameManager != null)
        {
            Debug.Log($"EndGame called with isWin = {isWin}");

            // Update the win/lose UI
            if (isWin)
            {
                winBackground.sprite = winSprite;
                endText.text = "You win";

                // Update the money text
                newMoneyText.text = $"Money: {scoreManager.GetLevelMoney()}";

                // Update the obstacles destroyed text
                newObstaclesDestroyedText.text = $"Destroyed: {scoreManager.GetLevelObstaclesDestroyed()}";

                // Update the time text
                int minutes = Mathf.FloorToInt(gameManager.GameTime / 60);
                int seconds = Mathf.FloorToInt(gameManager.GameTime % 60);
                newTimeText.text = $"Time: {minutes:00}:{seconds:00}";

                StartCoroutine(ShowWinTextsWithDelay());
            }
            else
            {
                winBackground.color = loseColor;
                endText.text = "The sun implodes";
                StartCoroutine(ShowLoseTextsWithDelay());
            }
        }
        else
        {
            Debug.Log("EndConditionsUI can't find ScoreManager or GameManager");
        }
    }

    private IEnumerator ShowWinTextsWithDelay()
    {
        // Show win text
        endText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(textAppearDelay);

        // Show time text and shine bar
        newTimeText.gameObject.SetActive(true);
        if (timeShineBar != null) timeShineBar.SetActive(true);
        PlayRandomTextSfx();
        yield return new WaitForSecondsRealtime(textAppearDelay);

        // Show obstacles destroyed text and shine bar
        newObstaclesDestroyedText.gameObject.SetActive(true);
        if (obstaclesDestroyedShineBar != null) obstaclesDestroyedShineBar.SetActive(true);
        PlayRandomTextSfx();
        yield return new WaitForSecondsRealtime(textAppearDelay);

        // Show score text and shine bar
        newMoneyText.gameObject.SetActive(true);
        if (moneyTextShineBar != null) moneyTextShineBar.SetActive(true);
        PlayRandomTextSfx();
        yield return new WaitForSecondsRealtime(2);

        newScoreSaveButtonFront.gameObject.SetActive(true);
        newScoreSaveButtonBack.gameObject.SetActive(true);
        oldScoreSaveButtonFront.gameObject.SetActive(true);
        oldScoreSaveButtonBack.gameObject.SetActive(true);

        oldMoneyText.gameObject.SetActive(true);
        oldObstaclesDestroyedText.gameObject.SetActive(true);
        oldTimeText.gameObject.SetActive(true);

        // Set initial alpha to 0
        var newBack = newScoreSaveButtonBack.GetComponent<CanvasGroup>();
        var newFront = newScoreSaveButtonFront.GetComponent<CanvasGroup>();
        var oldBack = oldScoreSaveButtonBack.GetComponent<CanvasGroup>();
        var oldFront = oldScoreSaveButtonFront.GetComponent<CanvasGroup>();
        var oldMoney = oldMoneyText.GetComponent<CanvasGroup>();
        var oldObstacles = oldObstaclesDestroyedText.GetComponent<CanvasGroup>();
        var oldTime = oldTimeText.GetComponent<CanvasGroup>();
        newBack.alpha = 0f;
        newFront.alpha = 0f;
        oldBack.alpha = 0f;
        newBack.alpha = 0f;
        oldMoney.alpha = 0f;
        oldObstacles.alpha = 0f;
        oldTime.alpha = 0f;

        endText.text = "Save One Score";

        // Get current level index
        int levelNumber = SceneManager.GetActiveScene().buildIndex - 1;
        // Load level data using ScoreManager's save format
        float money = PlayerPrefs.GetFloat($"Level_{levelNumber}_Money", 0);
        oldMoneyText.text = money > 0 ? $"Money: {money}" : "Money: 0";

        int obstaclesDestroyed = PlayerPrefs.GetInt($"Level_{levelNumber}_ObstaclesDestroyed", 0);
        oldObstaclesDestroyedText.text = obstaclesDestroyed > 0 ? $"Obstacles: {obstaclesDestroyed}" : "Obstacles: 0";

        float timeInSeconds = PlayerPrefs.GetFloat($"Level_{levelNumber}_Time", 0);
        oldTimeText.text = timeInSeconds > 0 ? $"Best Time: {Mathf.FloorToInt(timeInSeconds / 60):00}:{Mathf.FloorToInt(timeInSeconds % 60):00}" : "Time: --:--";

        // Fade in over time duration
        float fadeDuration = 1f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);

            newBack.alpha = alpha;
            newFront.alpha = alpha;
            oldBack.alpha = alpha;
            oldFront.alpha = alpha;
            oldMoney.alpha = alpha;
            oldObstacles.alpha = alpha;
            oldTime.alpha = alpha;

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // Ensure full visibility at the end
        newBack.alpha = 1f;
        newFront.alpha = 1f;
        oldBack.alpha = 1f;
        oldFront.alpha = 1f;
        oldMoney.alpha = 1f;
        oldObstacles.alpha = 1f;
        oldTime.alpha = 1f;

        // This button keeps the existing level score.
        oldScoreSaveButtonFront.onClick.AddListener(() =>
        {
            LoadMainMenuScene();
        });

        // This button saves a new score over the existing level score.
        newScoreSaveButtonFront.onClick.AddListener(() =>
        {
            EndConditionUIScoreChoiceEvent?.Invoke();
        });
    }

    IEnumerator ShowLoseTextsWithDelay()
    {
        // Show lose text
        /*endText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(textAppearDelay);

        // Countdown timer
        float countdownDuration = 3.1f;
        float countdownElapsed = 0f;

        while (countdownElapsed < countdownDuration)
        {
            float remaining = countdownDuration - countdownElapsed;

            // Calculate seconds and milliseconds (2 digits)
            int seconds = Mathf.FloorToInt(remaining);
            int milliseconds = Mathf.FloorToInt((remaining % 1) * 100); // Extract 2-digit ms

            // Format as "SS:MS" (e.g., "03:09", "00:99")
            endText.text = $"{seconds:00}:{milliseconds:00}";

            countdownElapsed += Time.unscaledDeltaTime;
            yield return null;
        }


        endText.text = "Blackhole";
        winBackground.color = Color.black;
        yield return new WaitForSecondsRealtime(textAppearDelay);
        endText.gameObject.SetActive(false);*/
        loseText.gameObject.SetActive(true);
        loseText.text = "What will you remember?";
        winBackground.color = memoryUpgradeColor;

        float loseTime = scoreManager.GetTotalTime() + gameManager.GameTime;
        int loseObstacles = scoreManager.GetTotalObstaclesDestroyed() + scoreManager.GetLevelObstaclesDestroyed();
        float loseMoney = scoreManager.GetTotalMoney() + scoreManager.GetLevelMoney();

        // Update the time text
        int minutes2 = Mathf.FloorToInt(loseTime / 60);
        int seconds2 = Mathf.FloorToInt(loseTime % 60);
        totalGameTimeText.text = $"Time: {minutes2:00}:{seconds2:00}";

        // Update the obstacles destroyed text
        totalObstaclesDestroyedText.text = $"Destroyed: " + loseObstacles;

        // Update the money text
        totalMoneyText.text = $"Money: " + loseMoney;

        endText.gameObject.SetActive(false);

        loseText.gameObject.SetActive(true);
        totalGameTimeText.gameObject.SetActive(true);
        totalObstaclesDestroyedText.gameObject.SetActive(true);
        totalMoneyText.gameObject.SetActive(true);
        PlayRandomTextSfx();
        yield return new WaitForSecondsRealtime(textAppearDelay);

        memoryScore = PlayerPrefs.GetFloat("memoryScore", 0);

        salesGameObject.SetActive(false);
        lineText.gameObject.SetActive(true);
        memoryScoreText.gameObject.SetActive(true);
        memoryScoreText.text = memoryScore.ToString("0") + " memories";
        UpdateMemoryAndSkillsText();
        PlayRandomTextSfx();
        yield return new WaitForSecondsRealtime(textAppearDelay);
        
        // Calculate final memory score
        float finalMemoryScore = memoryScore + (loseObstacles * loseMoney / loseTime);
        memoryScoreText.text = finalMemoryScore.ToString("0") + " memories";
        // Lerp all values simultaneously over 3 seconds
        float lerpDuration = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            float progress = elapsedTime / lerpDuration;

            // Lerp memory score up
            float currentMemoryValue = Mathf.Lerp(0, finalMemoryScore, progress);
            memoryScoreText.text = Mathf.RoundToInt(currentMemoryValue).ToString("0") + " memories";

            // Lerp other values down
            float currentTime = Mathf.Lerp(loseTime, 0, progress);
            int minutes3 = Mathf.FloorToInt(currentTime / 60);
            int seconds3 = Mathf.FloorToInt(currentTime % 60);
            totalGameTimeText.text = $"Time: {minutes3:00}:{seconds3:00}";

            float currentObstacles = Mathf.Lerp(loseObstacles, 0, progress);
            totalObstaclesDestroyedText.text = $"Destroyed: {Mathf.RoundToInt(currentObstacles)}";

            float currentMoney = Mathf.Lerp(loseMoney, 0, progress);
            totalMoneyText.text = $"Money: {Mathf.RoundToInt(currentMoney)}";

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        memoryScore = finalMemoryScore;

        // Ensure final values are exact
        memoryScoreText.text = Mathf.RoundToInt(memoryScore).ToString("0") + " memories";
        totalGameTimeText.text = "Time: 00:00";
        totalObstaclesDestroyedText.text = "Destroyed: 0";
        totalMoneyText.text = "Money: 0";
        
        // Show upgrade and Done buttons
        upgradeButtonHolder.gameObject.SetActive(true);
        saveButtonHolder.gameObject.SetActive(true);
        saveButtonFront.gameObject.SetActive(true);
        
    }

    public void UpdateMemoryAndSkillsText()
    {
        if (playerStatsManager == null)
        {
            playerStatsManager = FindAnyObjectByType<PlayerStatsManager>();
        }

        memoryScoreText.text = memoryScore.ToString("0") + " memories";

        engineeringText.text = "Engineering " + playerStatsManager.EngineeringSkill.ToString("0.00");
        pilotingText.text = "Piloting " + playerStatsManager.PilotingSkill.ToString("0.00");
        mechanicsText.text = "Mechanics " + playerStatsManager.MechanicsSkill.ToString("0.00");
        miningText.text = "Mining " + playerStatsManager.MiningSkill.ToString("0.00");
        roboticsText.text = "Robotics " + playerStatsManager.RoboticsSkill.ToString("0.00");
        combatText.text = "Combat " + playerStatsManager.CombatSkill.ToString("0.00");

        engineeringCost = playerStatsManager.EngineeringSkill * playerStatsManager.EngineeringSkill;
        engineeringCostText.text = "Cost " + engineeringCost.ToString("0.00");

        pilotingCost = playerStatsManager.PilotingSkill * playerStatsManager.PilotingSkill;
        pilotingCostText.text = "Cost " + pilotingCost.ToString("0.00");

        mechanicsCost = playerStatsManager.MechanicsSkill * playerStatsManager.MechanicsSkill;
        mechanicsCostText.text = "Cost " + mechanicsCost.ToString("0.00");

        miningCost = playerStatsManager.MiningSkill * playerStatsManager.MiningSkill;
        miningCostText.text = "Cost " + miningCost.ToString("0.00");

        roboticsCost = playerStatsManager.RoboticsSkill * playerStatsManager.RoboticsSkill;
        roboticsCostText.text = "Cost " + roboticsCost.ToString("0.00");

        combatCost = playerStatsManager.CombatSkill * playerStatsManager.CombatSkill;
        combatCostText.text = "Cost " + combatCost.ToString("0.00");

        Debug.Log($"Robotics Display - Value: {playerStatsManager.RoboticsSkill}, Cost: {roboticsCost}");
    }

    private void PlayRandomTextSfx()
    {
        if (textMovementSfx != null && textMovementSfx.Length > 0 && sFXManager != null)
        {
            int randomIndex = UnityEngine.Random.Range(0, textMovementSfx.Length);
            sFXManager.PlaySFX(textMovementSfx[randomIndex]);
        }
    }

    private void LoadMainMenuScene()
    {
        StartCoroutine(SavingScreenBeforeChangingScene());
    }

    IEnumerator SavingScreenBeforeChangingScene()
    {
        HideUI();

        winBackground.color = Color.black;
        endText.gameObject.SetActive(true);
        endText.text = "Saving...";

        yield return new WaitForSecondsRealtime(1);

        Loader.Load(Loader.Scene.MainMenuScene);
        Debug.Log("Loading Scene");
    }

    private void BuyEngineering()
    {
        if (memoryScore >= engineeringCost)
        {
            memoryScore -= engineeringCost;
            playerStatsManager.MultiplyEngineeringSkill();
            UpdateMemoryAndSkillsText();
        }
    }
    private void BuyPiloting()
    {
        if (memoryScore >= pilotingCost)
        {
            memoryScore -= pilotingCost;
            UpdateMemoryAndSkillsText();
            playerStatsManager.MultiplyPilotingSkill();
        }
    }
    private void BuyMechanics()
    {
        if (memoryScore >= mechanicsCost)
        {
            memoryScore -= mechanicsCost;
            UpdateMemoryAndSkillsText();
            playerStatsManager.MultiplyMechanicsSkill();
        }
    }
    private void BuyMining()
    {
        if (memoryScore >= miningCost)
        {
            memoryScore -= miningCost;
            UpdateMemoryAndSkillsText();
            playerStatsManager.MultiplyMiningSkill();
        }
    }
    private void BuyRobotics()
    {
        if (memoryScore >= roboticsCost)
        {
            memoryScore -= roboticsCost;
            UpdateMemoryAndSkillsText();
            playerStatsManager.MultiplyRoboticsSkill();
        }
    }
    private void BuyCombat()
    {
        if (memoryScore >= combatCost)
        {
            memoryScore -= combatCost;
            UpdateMemoryAndSkillsText();
            playerStatsManager.MultiplyCombatSkill();
        }
    }

    private void Revive()
    {
        reviveEvent?.Invoke();



        LoadMainMenuScene();
    }
}