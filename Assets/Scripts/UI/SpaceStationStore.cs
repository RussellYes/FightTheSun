using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpaceStationStore : MonoBehaviour
{

    private SFXManager sFXManager;

    [Header("UI References")]
    [SerializeField] private Button openStoreButton;
    [SerializeField] private Button closeStoreButton;
    [SerializeField] private GameObject storeHolder;

    [Header("Currency Displays")]
    [SerializeField] private TextMeshProUGUI memoriesText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI metalText;
    [SerializeField] private TextMeshProUGUI rareMetalText;

    [Header("Trade Buttons")]
    [SerializeField] private Button memoriesForMoneyButton;
    [SerializeField] private Button moneyForMetalButton;
    [SerializeField] private Button metalForRareMetalButton;
    [SerializeField] private Button rareMetalForMemoryButton;
    [SerializeField] private AudioClip buySucessSFX;
    [SerializeField] private AudioClip buyFailSFX;

    [Header("Trade Rates")]
    private int memoriesToMoneyRate = 200;
    private int moneyToMetalRate = 200;
    private int metalToRareMetalRate = 200;
    private int rareMetalToMemorRate = 200;
    private int tradeAmount = 100;
    private void OnEnable()
    {
        DataPersister.InitializationComplete += OnInitializationComplete;
        openStoreButton.onClick.AddListener(OpenStore);
        closeStoreButton.onClick.AddListener(CloseStore);
        memoriesForMoneyButton.onClick.AddListener(TradeMemoriesForMoney);
        moneyForMetalButton.onClick.AddListener(TrandeMoneyForMetal);
        metalForRareMetalButton.onClick.AddListener(TradeMetalForRareMetal);
        rareMetalForMemoryButton.onClick.AddListener(TradeRareMetalForMemories);
    }

    private void OnDisable()
    {
        DataPersister.InitializationComplete -= OnInitializationComplete;
        openStoreButton.onClick.RemoveListener(OpenStore);
        closeStoreButton.onClick.RemoveListener(CloseStore);
        memoriesForMoneyButton.onClick.RemoveListener(TradeMemoriesForMoney);
        moneyForMetalButton.onClick.RemoveListener(TrandeMoneyForMetal);
        metalForRareMetalButton.onClick.RemoveListener(TradeMetalForRareMetal);
        rareMetalForMemoryButton.onClick.RemoveListener(TradeRareMetalForMemories);
    }

    private void OnInitializationComplete()
    {
        // Get memories, money, metal, and rare metal values from DataPersister
    }

    private void Start()
    {
        storeHolder.SetActive(false);

    }
    private void OpenStore()
    {
        storeHolder.SetActive(true);
        UpdateTexts();
    }

    private void CloseStore()
    {
        storeHolder.SetActive(false);
    }

    private void UpdateTexts()
    {
        memoriesText.text = DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore.ToString();
        moneyText.text = DataPersister.Instance.CurrentGameData.totalMoney.ToString();
        metalText.text = DataPersister.Instance.CurrentGameData.totalMetal.ToString();
        rareMetalText.text = DataPersister.Instance.CurrentGameData.totalRareMetal.ToString();
    }
    private void TradeMemoriesForMoney()
    {
        if (DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore >= memoriesToMoneyRate)
        {
            DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore -= memoriesToMoneyRate;
            DataPersister.Instance.CurrentGameData.totalMoney += tradeAmount;
            UpdateTexts();
            // Play success SFX
            if (sFXManager != null)
            {
                sFXManager.PlaySFX(buySucessSFX);
            }
        }
        else
        {
            // Play failure SFX
            if (sFXManager != null)
            {
                sFXManager.PlaySFX(buyFailSFX);
            }
            Debug.Log("Not enough memories for this trade");
        }
    }

    private void TrandeMoneyForMetal()
    {
        if (DataPersister.Instance.CurrentGameData.totalMoney >= moneyToMetalRate)
        {
            DataPersister.Instance.CurrentGameData.totalMoney -= moneyToMetalRate;
            DataPersister.Instance.CurrentGameData.totalMetal += tradeAmount;
            UpdateTexts();
            // Play success SFX
            if (sFXManager != null)
            {
                sFXManager.PlaySFX(buySucessSFX);
            }
        }
        else
        {
            // Play failure SFX
            if (sFXManager != null)
            {
                sFXManager.PlaySFX(buyFailSFX);
            }
            Debug.Log("Not enough memories for this trade");
        }
    }

    private void TradeMetalForRareMetal()
    {
        if (DataPersister.Instance.CurrentGameData.totalMetal >= metalToRareMetalRate)
        {
            DataPersister.Instance.CurrentGameData.totalMetal -= metalToRareMetalRate;
            DataPersister.Instance.CurrentGameData.totalRareMetal += tradeAmount;
            UpdateTexts();
            // Play success SFX
            if (sFXManager != null)
            {
                sFXManager.PlaySFX(buySucessSFX);
            }
        }
        else
        {
            // Play failure SFX
            if (sFXManager != null)
            {
                sFXManager.PlaySFX(buyFailSFX);
            }
            Debug.Log("Not enough memories for this trade");
        }
    }

    private void TradeRareMetalForMemories()
    {
        if (DataPersister.Instance.CurrentGameData.totalRareMetal >= rareMetalToMemorRate)
        {
            DataPersister.Instance.CurrentGameData.totalRareMetal -= rareMetalToMemorRate;
            DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore += tradeAmount;
            // Play success SFX
            if (sFXManager != null)
            {
                sFXManager.PlaySFX(buySucessSFX);
            }
        }
        else
        {
            // Play failure SFX
            if (sFXManager != null)
            {
                sFXManager.PlaySFX(buyFailSFX);
            }
            Debug.Log("Not enough memories for this trade");
        }
    }





}


