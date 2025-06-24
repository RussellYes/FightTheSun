using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissileSellerUI : MonoBehaviour
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
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI missileText;
    [SerializeField] private TextMeshProUGUI moneyToMissileRateText;
    [SerializeField] private TextMeshProUGUI tradeAmountText;

    [Header("Trade Buttons")]
    [SerializeField] private Button moneyForMissileButton;
    [SerializeField] private AudioClip buySucessSFX;
    [SerializeField] private AudioClip buyFailSFX;

    [Header("Trade Rates")]
    private int moneyToMissileRate = 200;
    private int tradeAmount = 5;
    private void OnEnable()
    {
        DataPersister.InitializationComplete += OnInitializationComplete;
        openStoreButton.onClick.AddListener(OpenStore);
        closeStoreButton.onClick.AddListener(CloseStore);
        moneyForMissileButton.onClick.AddListener(TradeMoneyForMissile);
    }

    private void OnDisable()
    {
        DataPersister.InitializationComplete -= OnInitializationComplete;
        openStoreButton.onClick.RemoveListener(OpenStore);
        closeStoreButton.onClick.RemoveListener(CloseStore);
        moneyForMissileButton.onClick.RemoveListener(TradeMoneyForMissile);
    }

    private void OnInitializationComplete()
    {
        storeHolder.SetActive(false);
        sFXManager = SFXManager.Instance;
        moneyToMissileRateText.text = moneyToMissileRate.ToString();
        tradeAmountText.text = tradeAmount.ToString();
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
        moneyText.text = DataPersister.Instance.CurrentGameData.totalMoney.ToString();
        missileText.text = DataPersister.Instance.CurrentGameData.savedMissileCount.ToString();
    }




    private void TradeMoneyForMissile()
    {
        if (DataPersister.Instance.CurrentGameData.totalMoney >= moneyToMissileRate)
        {
            DataPersister.Instance.CurrentGameData.totalMoney -= moneyToMissileRate;
            DataPersister.Instance.CurrentGameData.savedMissileCount += tradeAmount;
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
