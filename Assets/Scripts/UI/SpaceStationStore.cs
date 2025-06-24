using System.Collections;
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
    [SerializeField] private GameObject storeButtonHolder;
    [SerializeField] private AudioClip[] storeOpenCloseSFX;
    [SerializeField] private float uIOpenCloseLerpTime = 1;

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
        storeHolder.SetActive(false);
        sFXManager = SFXManager.Instance;
    }

    private void OpenStore()
    {
        storeHolder.SetActive(true);
        UpdateTexts();
        playOpenCloseSFX();
        OpenStoreLerp();
    }

    private void CloseStore()
    {
        storeHolder.SetActive(false);
        playOpenCloseSFX();
        CloseStoreLerp();
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


