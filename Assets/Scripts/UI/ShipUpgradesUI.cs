using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class ShipUpgradesUI : MonoBehaviour
{
    private PlayerStatsManager playerStatsManager;
    private SFXManager sFXManager;

    [SerializeField] private GameObject shipUpgradeHolder;
    [SerializeField] private GameObject upgradeButtonHolder;
    [SerializeField] private Button shipUpgradeOpenButton;
    [SerializeField] private Button shipUpgradeCloseButton;
    [SerializeField] private float uIOpenCloseLerpTime = 1f;
    [SerializeField] private AudioClip[] shipUpgradeMenuOpenCloseSFX;
    [SerializeField] private AudioClip[] buttonPositiveSFX;
    [SerializeField] private AudioClip buttonNegitiveSFX;

    [Header("Memory Score")]
    [SerializeField] private TextMeshProUGUI memoryScoreText;
    [SerializeField] private TextMeshProUGUI metalScoreText;
    [SerializeField] private TextMeshProUGUI rareMetalScoreText;

    [Header("Upgrade Buttons")]
    public Button engineeringButton;
    public Button pilotingButton;
    public Button mechanicsButton;
    public Button miningButton;
    public Button roboticsButton;
    public Button combatButton;

    [Header("Engineering Upgrades")]
    [SerializeField] private TextMeshProUGUI engineeringText;
    [SerializeField] private TextMeshProUGUI engineeringMemoryCostText;
    private float engineeringMemoryCost;
    [SerializeField] private TextMeshProUGUI engineeringMetalCostText;
    private float engineeringMetalCost;
    [SerializeField] private TextMeshProUGUI engineeringRareMetalCostText;
    private float engineeringRareMetalCost;

    [Header("Piloting Upgrades")]
    [SerializeField] private TextMeshProUGUI pilotingText;
    [SerializeField] private TextMeshProUGUI pilotingMemoryCostText;
    private float pilotingMemoryCost;
    [SerializeField] private TextMeshProUGUI pilotingMetalCostText;
    private float pilotingMetalCost;
    [SerializeField] private TextMeshProUGUI pilotingRareMetalCostText;
    private float pilotingRareMetalCost;

    [Header("Mechanics Upgrades")]
    [SerializeField] private TextMeshProUGUI mechanicsText;
    [SerializeField] private TextMeshProUGUI mechanicsMemoryCostText;
    private float mechanicsMemoryCost;
    [SerializeField] private TextMeshProUGUI mechanicsMetalCostText;
    private float mechanicsMetalCost;
    [SerializeField] private TextMeshProUGUI mechanicsRareMetalCostText;
    private float mechanicsRareMetalCost;

    [Header("Mining Upgrades")]
    [SerializeField] private TextMeshProUGUI miningText;
    [SerializeField] private TextMeshProUGUI miningMemoryCostText;
    private float miningMemoryCost;
    [SerializeField] private TextMeshProUGUI miningMetalCostText;
    private float miningMetalCost;
    [SerializeField] private TextMeshProUGUI miningRareMetalCostText;
    private float miningRareMetalCost;

    [Header("Robotics Upgrades")]
    [SerializeField] private TextMeshProUGUI roboticsText;
    [SerializeField] private TextMeshProUGUI roboticsMemoryCostText;
    private float roboticsMemoryCost;
    [SerializeField] private TextMeshProUGUI roboticsMetalCostText;
    private float roboticsMetalCost;
    [SerializeField] private TextMeshProUGUI roboticsRareMetalCostText;
    private float roboticsRareMetalCost;

    [Header("Combat Upgrades")]
    [SerializeField] private TextMeshProUGUI combatText;
    [SerializeField] private TextMeshProUGUI combatMemoryCostText;
    private float combatMemoryCost;
    [SerializeField] private TextMeshProUGUI combatMetalCostText;
    private float combatMetalCost;
    [SerializeField] private TextMeshProUGUI combatRareMetalCostText;
    private float combatRareMetalCost;

    private void Start()
    {
        playerStatsManager = FindAnyObjectByType<PlayerStatsManager>();
        sFXManager = FindAnyObjectByType<SFXManager>();
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
            if (DataPersister.Instance.CurrentGameData.playerData.Count == 0)
            {
                DataPersister.Instance.CurrentGameData.playerData.Add(new PlayerSaveData());
            }
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

        //Play SFX
        if (sFXManager != null && shipUpgradeMenuOpenCloseSFX.Length > 0)
        {
            sFXManager.PlaySFX(shipUpgradeMenuOpenCloseSFX[UnityEngine.Random.Range(0, shipUpgradeMenuOpenCloseSFX.Length)]);
        }

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
        //Play SFX
        if (sFXManager != null && shipUpgradeMenuOpenCloseSFX.Length > 0)
        {
            sFXManager.PlaySFX(shipUpgradeMenuOpenCloseSFX[UnityEngine.Random.Range(0, shipUpgradeMenuOpenCloseSFX.Length)]);
        }

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
        return DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore;
    }

    public float GetMetalScore()
    {
        return DataPersister.Instance.CurrentGameData.totalMetal;
    }

    public float GetRareMetalScore()
    {
        return DataPersister.Instance.CurrentGameData.totalRareMetal;
    }
    public void UpdateResourcesText()
    {
        memoryScoreText.text = GetMemoryScore().ToString("0") + "";
        metalScoreText.text = GetMetalScore().ToString("0") + "";
        rareMetalScoreText.text = GetRareMetalScore().ToString("0") + "";
    }


    public void UpdateMemoryAndSkillsText()
    {
        UpdateResourcesText();

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

        float Squared(float value) => value * value;
        float Cube(float value) => value * value * value;

        // Memory costs for upgrades
        engineeringMemoryCost = Cube(playerStatsManager.EngineeringSkill);
        engineeringMemoryCostText.text = engineeringMemoryCost.ToString("0.00");

        pilotingMemoryCost = Cube(playerStatsManager.PilotingSkill);
        pilotingMemoryCostText.text = pilotingMemoryCost.ToString("0.00");

        mechanicsMemoryCost = Cube(playerStatsManager.MechanicsSkill);
        mechanicsMemoryCostText.text = mechanicsMemoryCost.ToString("0.00");

        miningMemoryCost = Cube(playerStatsManager.MiningSkill);
        miningMemoryCostText.text = miningMemoryCost.ToString("0.00");

        roboticsMemoryCost = Cube(playerStatsManager.RoboticsSkill);
        roboticsMemoryCostText.text = roboticsMemoryCost.ToString("0.00");

        combatMemoryCost = Cube(playerStatsManager.CombatSkill);
        combatMemoryCostText.text = combatMemoryCost.ToString("0.00");

        // Metal costs for upgrades
        engineeringMetalCost = Squared(playerStatsManager.EngineeringSkill);
        if (engineeringMetalCost < 1.5)
        {
            engineeringMetalCost = 0;
        }
        engineeringMetalCostText.text = engineeringMetalCost.ToString("0.00");

        pilotingMetalCost = Squared(playerStatsManager.PilotingSkill);
        if (pilotingMetalCost < 1.5)
        {
            pilotingMetalCost = 0;
        }
        pilotingMetalCostText.text = pilotingMetalCost.ToString("0.00");

        mechanicsMetalCost = Squared(playerStatsManager.MechanicsSkill);
        if (mechanicsMetalCost < 1.5)
        {
            mechanicsMetalCost = 0;
        }
        mechanicsMetalCostText.text = mechanicsMetalCost.ToString("0.00");

        miningMetalCost = Squared(playerStatsManager.MiningSkill);
        if (miningMetalCost < 1.5)
        {
            miningMetalCost = 0;
        }
        miningMetalCostText.text = miningMetalCost.ToString("0.00");

        roboticsMetalCost = Squared(playerStatsManager.RoboticsSkill);
        if (roboticsMetalCost < 1.5)
        {
            roboticsMetalCost = 0;
        }
        roboticsMetalCostText.text = roboticsMetalCost.ToString("0.00");

        combatMetalCost = Squared(playerStatsManager.CombatSkill);
        if (combatMetalCost < 1.5)
        {
            combatMetalCost = 0;
        }
        combatMetalCostText.text = combatMetalCost.ToString("0.00");

        // Rare Metal costs for upgrades
        engineeringRareMetalCost = Squared(playerStatsManager.EngineeringSkill);
        if (engineeringRareMetalCost < 2)
        {
            engineeringRareMetalCost = 0;
        }
        engineeringRareMetalCost = engineeringRareMetalCost / 2;
        engineeringRareMetalCostText.text = engineeringRareMetalCost.ToString("0.00");

        pilotingRareMetalCost = Squared(playerStatsManager.PilotingSkill);
        if (pilotingRareMetalCost < 2)
        {
            pilotingRareMetalCost = 0;
        }
        pilotingRareMetalCost = pilotingRareMetalCost / 2;
        pilotingRareMetalCostText.text = pilotingRareMetalCost.ToString("0.00");

        mechanicsRareMetalCost = Squared(playerStatsManager.MechanicsSkill);
        if (mechanicsRareMetalCost < 2)
        {
            mechanicsRareMetalCost = 0;
        }
        mechanicsRareMetalCost = mechanicsRareMetalCost / 2;
        mechanicsRareMetalCostText.text = mechanicsRareMetalCost.ToString("0.00");

        miningRareMetalCost = Squared(playerStatsManager.MiningSkill);
        if (miningRareMetalCost < 2)
        {
            miningRareMetalCost = 0;
        }
        miningRareMetalCost = miningRareMetalCost / 2;
        miningRareMetalCostText.text = miningRareMetalCost.ToString("0.00");

        roboticsRareMetalCost = Squared(playerStatsManager.RoboticsSkill);
        if (roboticsRareMetalCost < 2)
        {
            roboticsRareMetalCost = 0;
        }
        roboticsRareMetalCost = roboticsRareMetalCost / 2;
        roboticsRareMetalCostText.text = roboticsRareMetalCost.ToString("0.00");

        combatRareMetalCost = Squared(playerStatsManager.CombatSkill);
        if (combatRareMetalCost < 2)
        {
            combatRareMetalCost = 0;
        }
        combatRareMetalCost = combatRareMetalCost / 2;
        combatRareMetalCostText.text = combatRareMetalCost.ToString("0.00");

    }

    private void PlayPositiveButtonSFX()
    {
        if (sFXManager != null && buttonPositiveSFX.Length > 0)
        {
            sFXManager.PlaySFX(buttonPositiveSFX[UnityEngine.Random.Range(0, buttonPositiveSFX.Length)]);
        }
    }

    private void PlayNegitiveButtonSFX()
    {
        if (sFXManager != null && buttonNegitiveSFX != null)
        {
            sFXManager.PlaySFX(buttonNegitiveSFX);
        }
    }
    private void BuyEngineering()
    {
        if (GetMemoryScore() >= engineeringMemoryCost && GetMetalScore() >= engineeringMetalCost && GetRareMetalScore() >= engineeringRareMetalCost)
        {
            PlayPositiveButtonSFX();
            ChangeMemory(-engineeringMemoryCost);
            ChangeMetal(-engineeringMetalCost);
            ChangeRareMetal(-engineeringRareMetalCost);
            playerStatsManager.MultiplyEngineeringSkill();
            UpdateMemoryAndSkillsText();
        }
        else
        {
            PlayNegitiveButtonSFX();
        }
    }
    private void BuyPiloting()
    {
        if (GetMemoryScore() >= pilotingMemoryCost && GetMetalScore() >= pilotingMetalCost && GetRareMetalScore() >= pilotingRareMetalCost)
        {
            ChangeMemory(-pilotingMemoryCost);
            ChangeMetal(-pilotingMetalCost);
            ChangeRareMetal(-pilotingRareMetalCost);
            playerStatsManager.MultiplyPilotingSkill();
            UpdateMemoryAndSkillsText();
        }
    }
    private void BuyMechanics()
    {
        if (GetMemoryScore() >= mechanicsMemoryCost && GetMetalScore() >= mechanicsMetalCost && GetRareMetalScore() >= mechanicsRareMetalCost)
        {
            PlayPositiveButtonSFX();
            ChangeMemory(-mechanicsMemoryCost);
            ChangeMetal(-mechanicsMetalCost);
            ChangeRareMetal(-mechanicsRareMetalCost);
            playerStatsManager.MultiplyMechanicsSkill();
            UpdateMemoryAndSkillsText();
        }
        else
        {
            PlayNegitiveButtonSFX();
        }
    }
    private void BuyMining()
    {
        if (GetMemoryScore() >= miningMemoryCost && GetMetalScore() >= miningMetalCost && GetRareMetalScore() >= miningRareMetalCost)
        {
            ChangeMemory(-miningMemoryCost);
            ChangeMetal(-miningMetalCost);
            ChangeRareMetal(-miningRareMetalCost);
            playerStatsManager.MultiplyMiningSkill();
            UpdateMemoryAndSkillsText();
        }
    }
    private void BuyRobotics()
    {
        if (GetMemoryScore() >= roboticsMemoryCost && GetMetalScore() >= roboticsMetalCost && GetRareMetalScore() >= roboticsRareMetalCost)
        {
            PlayPositiveButtonSFX();
            ChangeMemory(-roboticsMemoryCost);
            ChangeMetal(-roboticsMetalCost);
            ChangeRareMetal(-roboticsRareMetalCost);
            playerStatsManager.MultiplyRoboticsSkill();
            UpdateMemoryAndSkillsText();
        }
        else
        {
            PlayNegitiveButtonSFX();
        }
    }
    private void BuyCombat()
    {
        if (GetMemoryScore() >= combatMemoryCost && GetMetalScore() >= combatMetalCost && GetRareMetalScore() >= combatRareMetalCost)
        {
            PlayPositiveButtonSFX();
            ChangeMemory(-combatMemoryCost);
            ChangeMetal(-combatMetalCost);
            ChangeRareMetal(-combatRareMetalCost);
            playerStatsManager.MultiplyCombatSkill();
            UpdateMemoryAndSkillsText();
        }
        else
        {
            PlayNegitiveButtonSFX();
        }
    }
    public void ChangeMemory(float amount)
    {
        DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore += amount;
        DataPersister.Instance.SaveCurrentGame();
    }
    public void ChangeMetal(float amount)
    {
        DataPersister.Instance.CurrentGameData.totalMetal += amount;
        DataPersister.Instance.SaveCurrentGame();
    }

    public void ChangeRareMetal(float amount)
    {
        DataPersister.Instance.CurrentGameData.totalRareMetal += amount;
        DataPersister.Instance.SaveCurrentGame();
    }

}
