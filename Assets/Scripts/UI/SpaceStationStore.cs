using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpaceStationStore : MonoBehaviour
{
    [SerializeField] private Button openStoreButton;
    [SerializeField] private Button closeStoreButton;

    [SerializeField] private GameObject storeHolder;
    [SerializeField] private TextMeshProUGUI memoriesText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI metalText;
    [SerializeField] private TextMeshProUGUI rareMetalText;
    [SerializeField] private Button memoriesForMoneyButton;
    [SerializeField] private Button moneyForMetalButton;
    [SerializeField] private Button metalForRareMetalButton;
    [SerializeField] private Button rareMetalForMemoriesButton;


    private void OnEnable()
    {
        DataPersister.InitializationComplete += OnInitializationComplete;
        openStoreButton.onClick.AddListener(OpenStore);
        closeStoreButton.onClick.AddListener(CloseStore);
        memoriesForMoneyButton.onClick.AddListener(TradeMemoriesForMoney);
        moneyForMetalButton.onClick.AddListener(TrandeMoneyForMetal);
        metalForRareMetalButton.onClick.AddListener(TradeMetalForRareMetal);
        rareMetalForMemoriesButton.onClick.AddListener(TradeRareMetalForMemories);
    }

    private void OnDisable()
    {
        DataPersister.InitializationComplete -= OnInitializationComplete;
        openStoreButton.onClick.RemoveListener(OpenStore);
        closeStoreButton.onClick.RemoveListener(CloseStore);
        memoriesForMoneyButton.onClick.RemoveListener(TradeMemoriesForMoney);
        moneyForMetalButton.onClick.RemoveListener(TrandeMoneyForMetal);
        metalForRareMetalButton.onClick.RemoveListener(TradeMetalForRareMetal);
        rareMetalForMemoriesButton.onClick.RemoveListener(TradeRareMetalForMemories);
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

    }

    private void CloseStore()
    {
        storeHolder.SetActive(false);
    }

    private void TradeMemoriesForMoney()
    {
        // Ensure that the trade is valid (e.g., player has enough rare metal)
        // Trade 200 memories for 100 money logic here
        SaveTrade();
        UpdateTexts();
    }

    private void TrandeMoneyForMetal()
    {
        // Ensure that the trade is valid (e.g., player has enough rare metal)
        // Trade 200 money for 100 metal logic here
        SaveTrade();
        UpdateTexts();
    }

    private void TradeMetalForRareMetal()
    {
        // Ensure that the trade is valid (e.g., player has enough rare metal)
        // Trade 200 metal for 100 rare metal logic here
        SaveTrade();
        UpdateTexts();
    }

    private void TradeRareMetalForMemories()
    {
        // Ensure that the trade is valid (e.g., player has enough rare metal)
        // Trade 200 rare metal for 100 memories logic here
        SaveTrade();
        UpdateTexts();
    }
    
    private void SaveTrade()
    {
        // Save memories, money, metal, and rare metal to DataPersister
    }
    private void UpdateTexts()
    {
        // Ensure that the trade is valid (e.g., player has enough rare metal)
        // Update UI texts with current values of money, metal, rare metal, and memories
    }


}


