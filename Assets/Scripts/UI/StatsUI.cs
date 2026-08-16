using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    /*
    time
    money
    metal
    rareMetal
    memory
    obstacle
    */

    ScoreManager scoreManager;

    [Header("Time UI")]
    [SerializeField] private GameObject timeHolder;
    [SerializeField] private Image timeIcon;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Button timeButton;

    [Header("Money UI")]
    [SerializeField] private GameObject moneyHolder;
    [SerializeField] private Image moneyIcon;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Button moneyButton;

    [Header("Metal UI")]
    [SerializeField] private GameObject metalHolder;
    [SerializeField] private Image metalIcon;
    [SerializeField] private TextMeshProUGUI metalText;
    [SerializeField] private Button metalButton;

    [Header("Rare Metal UI")]
    [SerializeField] private GameObject rareMetalHolder;
    [SerializeField] private Image rareMetalIcon;
    [SerializeField] private TextMeshProUGUI rareMetalText;
    [SerializeField] private Button rareMetalButton;

    [Header("Memory UI")]
    [SerializeField] private GameObject memoryHolder;
    [SerializeField] private Image memoryIcon;
    [SerializeField] private TextMeshProUGUI memoryText;
    [SerializeField] private Button memoryButton;

    [Header("Obstacle UI")]
    [SerializeField] private GameObject obstacleHolder;
    [SerializeField] private Image obstacleIcon;
    [SerializeField] private TextMeshProUGUI obstacleText;
    [SerializeField] private Button obstacleButton;

    [Header("Info Window")]
    [SerializeField] private GameObject infoWindowHolder;
    [SerializeField] private Button closeInfoWindowButton;
    [SerializeField] private Image statIcon;
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private TextMeshProUGUI statInfoText;


    private void Initialize()
    {
        scoreManager = FindFirstObjectByType<ScoreManager>();
        infoWindowHolder.SetActive(false);
        UpdateUI();
        ShowUI();
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        UpdateUI();
    }

    private void OnEnable()
    {
        DataPersister.InitializationComplete += Initialize;
        SceneManager.sceneLoaded += OnSceneLoad;
        timeButton.onClick.AddListener(() => { DisplayButtonInfo(timeButton); });
        moneyButton.onClick.AddListener(() => { DisplayButtonInfo(moneyButton); });
        metalButton.onClick.AddListener(() => { DisplayButtonInfo(metalButton); });
        rareMetalButton.onClick.AddListener(() => { DisplayButtonInfo(rareMetalButton); });
        memoryButton.onClick.AddListener(() => { DisplayButtonInfo(memoryButton); });
        obstacleButton.onClick.AddListener(() => { DisplayButtonInfo(obstacleButton); });
        closeInfoWindowButton.onClick.AddListener(() => { CloseInfoWindowButton(); });

        // Update UI
        ComicsUI.UpdateStatsUIEvent += UpdateUI;
        ShipUpgradesUI.UpdateStatsUIEvent += UpdateUI;
        CurrencyExchangeUI.UpdateStatsUIEvent += UpdateUI;
        MissileSellerUI.UpdateStatsUIEvent += UpdateUI;
    }

    private void OnDisable()
    {
        DataPersister.InitializationComplete -= Initialize;
        SceneManager.sceneLoaded -= OnSceneLoad;
        timeButton.onClick.RemoveAllListeners();
        moneyButton.onClick.RemoveAllListeners();
        metalButton.onClick.RemoveAllListeners();
        rareMetalButton.onClick.RemoveAllListeners();
        memoryButton.onClick.RemoveAllListeners();
        obstacleButton.onClick.RemoveAllListeners();
        closeInfoWindowButton.onClick.RemoveAllListeners();

        // Update UI
        ComicsUI.UpdateStatsUIEvent -= UpdateUI;
        ShipUpgradesUI.UpdateStatsUIEvent -= UpdateUI;
        CurrencyExchangeUI.UpdateStatsUIEvent -= UpdateUI;
        MissileSellerUI.UpdateStatsUIEvent -= UpdateUI;
    }


    public void ShowUI()
    {
        timeHolder.SetActive(true);
        moneyHolder.SetActive(true);
        metalHolder.SetActive(true);
        rareMetalHolder.SetActive(true);
        memoryHolder.SetActive(true);
        obstacleHolder.SetActive(true);
    }

    public void HideUI()
    {
        timeHolder.SetActive(false);
        moneyHolder.SetActive(false);
        metalHolder.SetActive(false);
        rareMetalHolder.SetActive(false);
        memoryHolder.SetActive(false);
        obstacleHolder.SetActive(false);
        infoWindowHolder.SetActive(false);
    }

    private void UpdateUI()
    {
        if (!scoreManager) 
        {
            Debug.Log($"StatsUI UpdateUI - scoreManager {scoreManager}.");
            return; 
        }

        int minutes = Mathf.FloorToInt(DataPersister.Instance.CurrentGameData.totalTime / 60);
        int seconds = Mathf.FloorToInt(DataPersister.Instance.CurrentGameData.totalTime % 60);
        timeText.text = $"{minutes:00}:{seconds:00}";
        moneyText.text = FormatNumber(scoreManager.GetLevelMoney());
        metalText.text = FormatNumber(scoreManager.GetTotalMetal());
        rareMetalText.text = FormatNumber(scoreManager.GetTotalRareMetal());
        memoryText.text = FormatNumber(DataPersister.Instance.CurrentGameData.memory);
        obstacleText.text = ($"{scoreManager.GetTotalObstaclesDestroyed()}");
    }

    private string FormatNumber(float value)
    {
        if (value < 1000)
        {
            return value.ToString("F1");
        }
        else if (value < 1000000)
        {
            return (value / 1000f).ToString("F1") + "K";
        }
        else if (value < 1000000000)
        {
            return (value / 1000000f).ToString("F1") + "M";
        }
        else if (value < 1000000000000)
        {
            return (value / 1000000000f).ToString("F1") + "B";
        }
        else
        {
            return (value / 1000000000000f).ToString("F1") + "T";
        }
    }

    private void DisplayButtonInfo(Button button)
    {
        Debug.Log("StatsUI DisplayButtonInfo");

        if (button == null) { return; }

        infoWindowHolder.SetActive(true);

        if (button == timeButton)
        {
            statIcon.sprite = timeIcon.sprite;
            statNameText.text = "Time";
            statInfoText.text = "The total TIME of your trip to the Sun. You have 60 minutes total to stop the Sun's collapse.";
        }
        else if (button == moneyButton)
        {
            statIcon.sprite = moneyIcon.sprite;
            statNameText.text = "Money";
            statInfoText.text = "MONEY can be spent at shops, and is used to unlock comics.";
        }
        else if (button == metalButton)
        {
            statIcon.sprite = metalIcon.sprite;
            statNameText.text = "Metal";
            statInfoText.text = "METAL is mined from asteroids. METAL can be spent at shops, upgrades the ship, and is used to unlock planets.";
        }
        else if (button == rareMetalButton)
        {
            statIcon.sprite = rareMetalIcon.sprite;
            statNameText.text = "Rare Metal";
            statInfoText.text = "RARE METAL is mined from asteroids. RARE METAL can be spent at shops, upgrades the ship, and is used to unlock planets.";
        }
        else if (button == memoryButton)
        {
            statIcon.sprite = memoryIcon.sprite;
            statNameText.text = "Memory";
            statInfoText.text = "MEMORY is the currency you gain when time traveling, and is used to unlocks comics.";
        }
        else if (button == obstacleButton)
        {
            statIcon.sprite = obstacleIcon.sprite;
            statNameText.text = "Obstacle";
            statInfoText.text = "A total of all destroyed OBSTACLEs";
        }
    }

    private void CloseInfoWindowButton()
    {
        infoWindowHolder.SetActive(false);
    }

}
