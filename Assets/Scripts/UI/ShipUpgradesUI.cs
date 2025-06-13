using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipUpgradesUI : MonoBehaviour
{
    private PlayerStatsManager playerStatsManager;

    [SerializeField] private GameObject shipUpgradeHolder;
    [SerializeField] private GameObject upgradeButtonHolder;
    [SerializeField] private Button shipUpgradeOpenButton;
    [SerializeField] private Button shipUpgradeCloseButton;
    [SerializeField] private float uIOpenCloseLerpTime = 1f;

    [Header("Memory Score")]
    private float memoryScore;
    [SerializeField] private TextMeshProUGUI memoryScoreText;

    [Header("Upgrade Buttons")]
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

    private void Start()
    {
        playerStatsManager = FindAnyObjectByType<PlayerStatsManager>();
        shipUpgradeHolder.SetActive(false);
    }

    private void OnDataPersisterReady()
    {
        LoadMemoryAndSkills();
    }
    private void LoadMemoryAndSkills()
    {
        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            if (DataPersister.Instance.CurrentGameData.playerData.Count > 0)
                memoryScore = DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore;
            else
            {
                // Initialize if empty
                DataPersister.Instance.CurrentGameData.playerData.Add(new PlayerSaveData());
                memoryScore = 0;
            }

            Debug.Log($"Loaded memoryScore: {memoryScore}");
        }
        UpdateMemoryAndSkillsText();
    }
    private void OnEnable()
    {
        DataPersister.InitializationComplete += OnDataPersisterReady;
        shipUpgradeOpenButton.onClick.AddListener(() => StartCoroutine(OpenShipUpgradeMenu()));
        shipUpgradeCloseButton.onClick.AddListener(() => StartCoroutine(CloseShipUpgradeMenu()));
        // Upgrade button listeners
        engineeringButton.onClick.AddListener(() => { BuyEngineering(); });
        pilotingButton.onClick.AddListener(() => { BuyPiloting(); });
        mechanicsButton.onClick.AddListener(() => { BuyMechanics(); });
        miningButton.onClick.AddListener(() => { BuyMining(); });
        roboticsButton.onClick.AddListener(() => { BuyRobotics(); });
        combatButton.onClick.AddListener(() => { BuyCombat(); });
    }

    private void OnDisable()
    {
        DataPersister.InitializationComplete -= OnDataPersisterReady;
        shipUpgradeOpenButton.onClick.RemoveListener(() => StartCoroutine(OpenShipUpgradeMenu()));
        shipUpgradeCloseButton.onClick.RemoveListener(() => StartCoroutine(CloseShipUpgradeMenu()));
        // Upgrade button listeners
        engineeringButton.onClick.RemoveListener(() => { BuyEngineering(); });
        pilotingButton.onClick.RemoveListener(() => { BuyPiloting(); });
        mechanicsButton.onClick.RemoveListener(() => { BuyMechanics(); });
        miningButton.onClick.RemoveListener(() => { BuyMining(); });
        roboticsButton.onClick.RemoveListener(() => { BuyRobotics(); });
        combatButton.onClick.RemoveListener(() => { BuyCombat(); });
    }

    IEnumerator OpenShipUpgradeMenu()
    {
        shipUpgradeHolder.SetActive(true);
        // without delay, move upgradeButtonHolder up 2000 on the y axis.
        RectTransform rectTransform = upgradeButtonHolder.GetComponent<RectTransform>();
        Vector3 originalPosition = rectTransform.localPosition;
        Vector3 startPosition = originalPosition + new Vector3(0, 2000, 0);
        rectTransform.localPosition = startPosition;

        //  Update memory score and skills text.
        UpdateMemoryAndSkillsText();

        // lerp upgradeButtonHolder's position from its +2000 y axis position to its original position over UIOpenCloseLerpTime seconds.
        float elapsedTime = 0f;
        while (elapsedTime < uIOpenCloseLerpTime)
        {
            rectTransform.localPosition = Vector3.Lerp(startPosition, originalPosition, elapsedTime / uIOpenCloseLerpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rectTransform.localPosition = originalPosition;
    }

    IEnumerator CloseShipUpgradeMenu()
    {
        // lerp upgradeButtonHolder's position from its original position to +2000 y over UIOpenCloseLerpTime seconds.
        RectTransform rectTransform = upgradeButtonHolder.GetComponent<RectTransform>();
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

        // without delay, move upgradeButtonHolder down 2000 on the y axis back to its original position.
        rectTransform.localPosition = originalPosition;

        shipUpgradeHolder.SetActive(false);
    }

    public float GetMemoryScore()
    {
        return memoryScore;
    }
    public void UpdateMemoryText()
    {
        memoryScoreText.text = memoryScore.ToString("0") + " memories";
    }


    public void UpdateMemoryAndSkillsText()
    {
        UpdateMemoryText();

        if (playerStatsManager == null)
        {
            playerStatsManager = FindAnyObjectByType<PlayerStatsManager>();
        }

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
    }

    private void BuyEngineering()
    {
        if (memoryScore >= engineeringCost)
        {
            memoryScore -= engineeringCost;
            playerStatsManager.MultiplyEngineeringSkill();
            SaveMemoryAndSkills();
            UpdateMemoryAndSkillsText();
        }
    }
    private void BuyPiloting()
    {
        if (memoryScore >= pilotingCost)
        {
            memoryScore -= pilotingCost;
            playerStatsManager.MultiplyPilotingSkill();
            SaveMemoryAndSkills();
            UpdateMemoryAndSkillsText();
        }
    }
    private void BuyMechanics()
    {
        if (memoryScore >= mechanicsCost)
        {
            memoryScore -= mechanicsCost;
            playerStatsManager.MultiplyMechanicsSkill();
            SaveMemoryAndSkills();
            UpdateMemoryAndSkillsText();
        }
    }
    private void BuyMining()
    {
        if (memoryScore >= miningCost)
        {
            memoryScore -= miningCost;
            playerStatsManager.MultiplyMiningSkill();
            SaveMemoryAndSkills();
            UpdateMemoryAndSkillsText();
        }
    }
    private void BuyRobotics()
    {
        if (memoryScore >= roboticsCost)
        {
            memoryScore -= roboticsCost;
            playerStatsManager.MultiplyRoboticsSkill();
            SaveMemoryAndSkills();
            UpdateMemoryAndSkillsText();
        }
    }
    private void BuyCombat()
    {
        if (memoryScore >= combatCost)
        {
            memoryScore -= combatCost;
            playerStatsManager.MultiplyCombatSkill();
            SaveMemoryAndSkills();
            UpdateMemoryAndSkillsText();
        }
    }

    private void SaveMemoryAndSkills()
    {
        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            // Ensure player data exists
            if (DataPersister.Instance.CurrentGameData.playerData.Count == 0)
            {
                DataPersister.Instance.CurrentGameData.playerData.Add(new PlayerSaveData());
            }

            // Update memory score
            DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore = memoryScore;

            // Skills are saved by PlayerStatsManager when changed

            // Save the game
            DataPersister.Instance.SaveCurrentGame();
            Debug.Log("Saved memory score and skills");
        }
    }

}
