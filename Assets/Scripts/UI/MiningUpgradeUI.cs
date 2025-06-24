using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiningUpgradeUI : MonoBehaviour
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
    [SerializeField] private TextMeshProUGUI rareMetalText;
    [SerializeField] private TextMeshProUGUI launcherText;
    [SerializeField] private TextMeshProUGUI rareMetalToLauncherRateText;
    [SerializeField] private TextMeshProUGUI tradeAmountText;

    [Header("Trade Buttons")]
    [SerializeField] private Button rareMetalForLauncherButton;
    [SerializeField] private AudioClip buySucessSFX;
    [SerializeField] private AudioClip buyFailSFX;

    [Header("Trade Rates")]
    private int rareMetalToLauncherRate = 125;
    private int tradeAmount = 1;
    private void OnEnable()
    {
        DataPersister.InitializationComplete += OnInitializationComplete;
        openStoreButton.onClick.AddListener(OpenStore);
        closeStoreButton.onClick.AddListener(CloseStore);
        rareMetalForLauncherButton.onClick.AddListener(TradeRareMetalForLauncher);
    }

    private void OnDisable()
    {
        DataPersister.InitializationComplete -= OnInitializationComplete;
        openStoreButton.onClick.RemoveListener(OpenStore);
        closeStoreButton.onClick.RemoveListener(CloseStore);
        rareMetalForLauncherButton.onClick.RemoveListener(TradeRareMetalForLauncher);
    }

    private void OnInitializationComplete()
    {
        storeHolder.SetActive(false);
        sFXManager = SFXManager.Instance;
        rareMetalToLauncherRateText.text = (rareMetalToLauncherRate * DataPersister.Instance.CurrentGameData.savedLauncherLevel).ToString();
        tradeAmountText.text = ("+") + tradeAmount.ToString();
    }
    private void OpenStore()
    {
        storeHolder.SetActive(true);
        UpdateTexts();
        PlayOpenCloseSFX();
        StartCoroutine(OpenStoreLerp());
    }

    private void CloseStore()
    {
        storeHolder.SetActive(false);
        PlayOpenCloseSFX();
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

    private void PlayOpenCloseSFX()
    {
        AudioClip sFX = storeOpenCloseSFX[Random.Range(0, storeOpenCloseSFX.Length)];

        if (sFXManager != null)
        {
            sFXManager.PlaySFX(sFX);
        }
    }

    private void UpdateTexts()
    {
        rareMetalText.text = DataPersister.Instance.CurrentGameData.totalRareMetal.ToString("F1");
        launcherText.text = DataPersister.Instance.CurrentGameData.savedLauncherLevel.ToString();
    }




    private void TradeRareMetalForLauncher()
    {
        if (DataPersister.Instance.CurrentGameData.savedLauncherLevel >= 4)
        {
            FailButtonSFX();
            return; // Cannot trade if launcher level is already at max
        }
        if (DataPersister.Instance.CurrentGameData.totalRareMetal >= rareMetalToLauncherRate)
        {
            DataPersister.Instance.CurrentGameData.totalRareMetal -= rareMetalToLauncherRate;
            DataPersister.Instance.CurrentGameData.savedLauncherLevel += tradeAmount;
            UpdateTexts();
            DataPersister.Instance.SaveCurrentGame();
            SuccessButtonSFX();
        }
        else
        {
            FailButtonSFX();
        }
    }

    private void SuccessButtonSFX()
    {
        if (sFXManager != null)
        {
            sFXManager.PlaySFX(buySucessSFX);
        }
    }

    private void FailButtonSFX()
    {
        if (sFXManager != null)
        {
            sFXManager.PlaySFX(buyFailSFX);
        }
    }





}

