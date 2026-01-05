using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyExchangeUI : MonoBehaviour
{

    private SFXManager sFXManager;

    [Header("UI References")]
    [SerializeField] private Button openStoreButton;
    [SerializeField] private Button closeStoreButton;
    [SerializeField] private GameObject storeHolder;
    [SerializeField] private GameObject storeButtonHolder;
    [SerializeField] private AudioClip[] storeOpenCloseSFX;
    [SerializeField] private float uIOpenCloseLerpTime = 1;

    [Header("Trade Rates Texts")]
    [SerializeField] private TextMeshProUGUI memoriesToMoneyRateText;
    [SerializeField] private TextMeshProUGUI moneyToMetalRateText;
    [SerializeField] private TextMeshProUGUI metalToRareMetalRateText;
    [SerializeField] private TextMeshProUGUI rareMetalToMemoryRateText;

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
    [SerializeField] private TextMeshProUGUI memoriesForMoneyButtonVisualCue;
    [SerializeField] private TextMeshProUGUI moneyForMetalButtonVisualCue;
    [SerializeField] private TextMeshProUGUI metalForRareMetalButtonVisualCue;
    [SerializeField] private TextMeshProUGUI rareMetalForMemoryButtonVisualCue;
    [SerializeField] private AudioClip buySucessSFX;
    [SerializeField] private AudioClip buyFailSFX;

    [Header("Button State Colors")]
    [SerializeField] private Color canBuyUpgradeButtonColour;
    [SerializeField] private Color cantBuyUpgradeButtonColour;

    [Header("Trade Rates")]
    private int memoriesToMoneyRate = 200;
    private int moneyToMetalRate = 500;
    private int metalToRareMetalRate = 600;
    private int rareMetalToMemoryRate = 100;
    private int tradeAmount = 100;
    private void OnEnable()
    {
        DataPersister.InitializationComplete += OnInitializationComplete;
        openStoreButton.onClick.AddListener(OpenStore);
        closeStoreButton.onClick.AddListener(CloseStore);
        memoriesForMoneyButton.onClick.AddListener(TradeMemoriesForMoney);
        moneyForMetalButton.onClick.AddListener(TradeMoneyForMetal);
        metalForRareMetalButton.onClick.AddListener(TradeMetalForRareMetal);
        rareMetalForMemoryButton.onClick.AddListener(TradeRareMetalForMemories);
    }

    private void OnDisable()
    {
        DataPersister.InitializationComplete -= OnInitializationComplete;
        openStoreButton.onClick.RemoveListener(OpenStore);
        closeStoreButton.onClick.RemoveListener(CloseStore);
        memoriesForMoneyButton.onClick.RemoveListener(TradeMemoriesForMoney);
        moneyForMetalButton.onClick.RemoveListener(TradeMoneyForMetal);
        metalForRareMetalButton.onClick.RemoveListener(TradeMetalForRareMetal);
        rareMetalForMemoryButton.onClick.RemoveListener(TradeRareMetalForMemories);
    }

    private void OnInitializationComplete()
    {
        storeHolder.SetActive(false);
        sFXManager = SFXManager.Instance;
    }

    private void OpenStore()
    {
        storeHolder.SetActive(true);
        UpdateTexts();
        SetPriceTexts();
        playOpenCloseSFX();
        StartCoroutine(OpenStoreLerp());
    }

    private void CloseStore()
    {
        storeHolder.SetActive(false);
        playOpenCloseSFX();
        StartCoroutine(CloseStoreLerp());
    }

    IEnumerator OpenStoreLerp()
    {
        // without delay, move storeButtonHolder up 2000 on the y axis.
        RectTransform rectTransform = storeButtonHolder.GetComponent<RectTransform>();
        Vector3 originalPosition = rectTransform.localPosition;
        Vector3 startPosition = originalPosition + new Vector3(0, 2000, 0);
        rectTransform.localPosition = startPosition;

        // lerp storeButtonHolder's position from its +2000 y axis position to its original position over UIOpenCloseLerpTime seconds.
        float elapsedTime = 0f;
        while (elapsedTime < uIOpenCloseLerpTime)
        {
            rectTransform.localPosition = Vector3.Lerp(startPosition, originalPosition, elapsedTime / uIOpenCloseLerpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rectTransform.localPosition = originalPosition;
    }
    IEnumerator CloseStoreLerp()
    {
        // lerp storeButtonHolder's position from its original position to +2000 y over UIOpenCloseLerpTime seconds.
        RectTransform rectTransform = storeButtonHolder.GetComponent<RectTransform>();
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
    }

    private void playOpenCloseSFX()
    {
        AudioClip sFX = storeOpenCloseSFX[Random.Range(0, storeOpenCloseSFX.Length)];

        if (sFXManager != null)
        {
            sFXManager.PlaySFX(sFX);
        }
    }

    private void UpdateTexts()
    {
        memoriesText.text = DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore.ToString("0.0");
        moneyText.text = DataPersister.Instance.CurrentGameData.totalMoney.ToString("0");
        metalText.text = DataPersister.Instance.CurrentGameData.totalMetal.ToString("0.0");
        rareMetalText.text = DataPersister.Instance.CurrentGameData.totalRareMetal.ToString("0.0");

        UpdateButtonState(memoriesForMoneyButton, memoriesForMoneyButtonVisualCue, (int)DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore, memoriesToMoneyRate);             
        UpdateButtonState(moneyForMetalButton, moneyForMetalButtonVisualCue, (int)DataPersister.Instance.CurrentGameData.totalMoney, moneyToMetalRate);
        UpdateButtonState(metalForRareMetalButton, metalForRareMetalButtonVisualCue, (int)DataPersister.Instance.CurrentGameData.totalMetal, metalToRareMetalRate);
        UpdateButtonState(rareMetalForMemoryButton, rareMetalForMemoryButtonVisualCue, (int)DataPersister.Instance.CurrentGameData.totalRareMetal, rareMetalToMemoryRate);
    }

    private void UpdateButtonState(Button button, TextMeshProUGUI buttonVisualCue, int currencyOwned, int currencyCost)
    {
        bool hasEnoughMoney = currencyOwned >= currencyCost;
        button.interactable = hasEnoughMoney;
        if (buttonVisualCue != null)
        {
            buttonVisualCue.color = hasEnoughMoney ? canBuyUpgradeButtonColour : cantBuyUpgradeButtonColour;
        }
    }
    private void TradeMemoriesForMoney()
    {
        if (DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore >= memoriesToMoneyRate)
        {
            DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore -= memoriesToMoneyRate;
            DataPersister.Instance.CurrentGameData.totalMoney += tradeAmount;
            DataPersister.Instance.SaveCurrentGame();
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

    private void TradeMoneyForMetal()
    {
        if (DataPersister.Instance.CurrentGameData.totalMoney >= moneyToMetalRate)
        {
            DataPersister.Instance.CurrentGameData.totalMoney -= moneyToMetalRate;
            DataPersister.Instance.CurrentGameData.totalMetal += tradeAmount;
            DataPersister.Instance.SaveCurrentGame();
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
            DataPersister.Instance.SaveCurrentGame();
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
        if (DataPersister.Instance.CurrentGameData.totalRareMetal >= rareMetalToMemoryRate)
        {
            DataPersister.Instance.CurrentGameData.totalRareMetal -= rareMetalToMemoryRate;
            DataPersister.Instance.CurrentGameData.playerData[0].playerMemoryScore += tradeAmount;
            DataPersister.Instance.SaveCurrentGame();
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

    private void SetPriceTexts()
    {
        memoriesToMoneyRateText.text = memoriesToMoneyRate.ToString("0");
        moneyToMetalRateText.text = moneyToMetalRate.ToString("0");
        metalToRareMetalRateText.text = metalToRareMetalRate.ToString("0");
        rareMetalToMemoryRateText.text = rareMetalToMemoryRate.ToString("0");
    }



}


