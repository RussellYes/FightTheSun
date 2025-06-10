using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipUpgradesUI : MonoBehaviour
{
    private PlayerStatsManager playerStatsManager;

    [SerializeField] private GameObject shipUpgradeHolder;
    [SerializeField] private Button shipUpgradeOpenButton;
    [SerializeField] private Button shipUpgradeCloseButton;

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

    private void OnEnable()
    {
        shipUpgradeOpenButton.onClick.AddListener(OpenShipUpgradeMenu);
        shipUpgradeCloseButton.onClick.AddListener(CloseShipUpgradeMenu);
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
        shipUpgradeOpenButton.onClick.RemoveListener(OpenShipUpgradeMenu);
        shipUpgradeCloseButton.onClick.RemoveListener(CloseShipUpgradeMenu);
        // Upgrade button listeners
        engineeringButton.onClick.RemoveListener(() => { BuyEngineering(); });
        pilotingButton.onClick.RemoveListener(() => { BuyPiloting(); });
        mechanicsButton.onClick.RemoveListener(() => { BuyMechanics(); });
        miningButton.onClick.RemoveListener(() => { BuyMining(); });
        roboticsButton.onClick.RemoveListener(() => { BuyRobotics(); });
        combatButton.onClick.RemoveListener(() => { BuyCombat(); });
    }

    private void OpenShipUpgradeMenu()
    {
        shipUpgradeHolder.SetActive(true);
    }

    private void CloseShipUpgradeMenu()
    {
        shipUpgradeHolder.SetActive(false);
    }

    public void UpdateMemoryText()
    {
        if (playerStatsManager == null)
        {
            playerStatsManager = FindAnyObjectByType<PlayerStatsManager>();
        }

        memoryScoreText.text = memoryScore.ToString("0") + " memories";
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
    }

    private void BuyEngineering()
    {
        if (memoryScore >= engineeringCost)
        {
            memoryScore -= engineeringCost;
            playerStatsManager.MultiplyEngineeringSkill();
            UpdateMemoryText();
        }
    }
    private void BuyPiloting()
    {
        if (memoryScore >= pilotingCost)
        {
            memoryScore -= pilotingCost;
            UpdateMemoryText();
            playerStatsManager.MultiplyPilotingSkill();
        }
    }
    private void BuyMechanics()
    {
        if (memoryScore >= mechanicsCost)
        {
            memoryScore -= mechanicsCost;
            UpdateMemoryText();
            playerStatsManager.MultiplyMechanicsSkill();
        }
    }
    private void BuyMining()
    {
        if (memoryScore >= miningCost)
        {
            memoryScore -= miningCost;
            UpdateMemoryText();
            playerStatsManager.MultiplyMiningSkill();
        }
    }
    private void BuyRobotics()
    {
        if (memoryScore >= roboticsCost)
        {
            memoryScore -= roboticsCost;
            UpdateMemoryText();
            playerStatsManager.MultiplyRoboticsSkill();
        }
    }
    private void BuyCombat()
    {
        if (memoryScore >= combatCost)
        {
            memoryScore -= combatCost;
            UpdateMemoryText();
            playerStatsManager.MultiplyCombatSkill();
        }
    }



}
