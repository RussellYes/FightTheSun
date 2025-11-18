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
    [SerializeField] private float uIOpenCloseLerpTime = 2;

    [Header("Currency Displays")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI missileText;
    [SerializeField] private TextMeshProUGUI shieldText;
    [SerializeField] private TextMeshProUGUI nukeText;
    [SerializeField] private TextMeshProUGUI missileCostText;
    [SerializeField] private TextMeshProUGUI shieldCostText;
    [SerializeField] private TextMeshProUGUI nukeCostText;
    [SerializeField] private TextMeshProUGUI missileAmountText;
    [SerializeField] private TextMeshProUGUI shieldAmountText;
    [SerializeField] private TextMeshProUGUI nukeAmountText;
    [SerializeField] private TextMeshProUGUI missileDamageCostText;
    [SerializeField] private TextMeshProUGUI shieldHpCostText;
    [SerializeField] private TextMeshProUGUI nukeLowerCostText;
    [SerializeField] private TextMeshProUGUI missileDamagePotentialText;
    [SerializeField] private TextMeshProUGUI shieldHpPotentialText;
    [SerializeField] private TextMeshProUGUI nukeCostPotentialText;

    [Header("Trade Buttons")]
    [SerializeField] private Button buyMissileButton;
    [SerializeField] private Button buyMissileDamageButton;
    [SerializeField] private Button buyShieldButton;
    [SerializeField] private Button buyShieldHpButton;
    [SerializeField] private Button buyNukeButton;
    [SerializeField] private Button buyNukeLowerCostButton;
    [SerializeField] private AudioClip buySucessSFX;
    [SerializeField] private AudioClip buyFailSFX;

    [Header("Button State Colors")]
    [SerializeField] private Color canBuyUpgradeButtonColour;
    [SerializeField] private Color cantBuyUpgradeButtonColour;

    [Header("Button Images")]
    [SerializeField] private TextMeshProUGUI buyMissileButtonVisualCue;
    [SerializeField] private TextMeshProUGUI buyMissileDamageButtonVisualCue;
    [SerializeField] private TextMeshProUGUI buyShieldButtonVisualCue;
    [SerializeField] private TextMeshProUGUI buyShieldHpButtonVisualCue;
    [SerializeField] private TextMeshProUGUI buyNukeButtonVisualCue;
    [SerializeField] private TextMeshProUGUI buyNukeLowerCostButtonVisualCue;

    [Header("Trade Rates")]
    // missiles
    private float missileCostMultiplier = 1.02f;
    private float missileDamageCostMultiplier = 1.02f;
    private float missileDamageMultiplier = 1.02f;
    private int missileAmount = 5;
    // shields
    private float shieldCostMultiplier = 1.02f;
    private float shieldHpCostMultiplier = 1.02f;
    private float shieldHpMultiplier = 1.02f;
    private int shieldAmount = 1;
    // nukes
    private float nukeLowerCostMultiplier = 1.02f;
    private int nukeAmount = 1;
    private float nukeLowerCost = 60000;

    private void OnEnable()
    {
        DataPersister.InitializationComplete += OnInitializationComplete;
        openStoreButton.onClick.AddListener(OpenStore);
        closeStoreButton.onClick.AddListener(CloseStore);
        buyMissileButton.onClick.AddListener(BuyMissile);
        buyMissileDamageButton.onClick.AddListener(BuyMissileDamage);
        buyShieldButton.onClick.AddListener(BuyShield);
        buyShieldHpButton.onClick.AddListener(BuyShieldHp);
        buyNukeButton.onClick.AddListener(BuyNuke);
        buyNukeLowerCostButton.onClick.AddListener(BuyNukeLowerCost);
    }

    private void OnDisable()
    {
        DataPersister.InitializationComplete -= OnInitializationComplete;
        openStoreButton.onClick.RemoveListener(OpenStore);
        closeStoreButton.onClick.RemoveListener(CloseStore);
        buyMissileButton.onClick.RemoveListener(BuyMissile);
        buyMissileDamageButton.onClick.RemoveListener(BuyMissileDamage);
        buyShieldButton.onClick.RemoveListener(BuyShield);
        buyShieldHpButton.onClick.RemoveListener(BuyShieldHp);
        buyNukeButton.onClick.RemoveListener(BuyNuke);
        buyNukeLowerCostButton.onClick.RemoveListener(BuyNukeLowerCost);
    }

    private void OnInitializationComplete()
    {
        storeHolder.SetActive(false);
        sFXManager = SFXManager.Instance;
    }
    private void OpenStore()
    {
        storeHolder.SetActive(true);
        UpdateUI();
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

    private void BuyMissile()
    {
        if (DataPersister.Instance.CurrentGameData.totalMoney >= DataPersister.Instance.CurrentGameData.missileCost)
        {
            // cost is subtracted from money for purchase
            DataPersister.Instance.CurrentGameData.totalMoney -= DataPersister.Instance.CurrentGameData.missileCost;
            DataPersister.Instance.CurrentGameData.missileCount += missileAmount;

            // The purchase cost increases with each purchase
            DataPersister.Instance.CurrentGameData.missileCost *= missileCostMultiplier;

            UpdateUI();
            DataPersister.Instance.SaveCurrentGame();
            SuccessButtonSFX();
        }
        else
        {
            FailButtonSFX();
        }
    }
    private void BuyMissileDamage()
    {
        if (DataPersister.Instance.CurrentGameData.totalMoney >= DataPersister.Instance.CurrentGameData.missileDamageCost)
        {
            // cost is subtracted from money for purchase
            DataPersister.Instance.CurrentGameData.totalMoney -= DataPersister.Instance.CurrentGameData.missileDamageCost;
            DataPersister.Instance.CurrentGameData.missileDamage *= missileDamageMultiplier;

            // The purchase cost increases with each purchase
            DataPersister.Instance.CurrentGameData.missileDamageCost *= missileDamageCostMultiplier;

            UpdateUI();
            DataPersister.Instance.SaveCurrentGame();
            SuccessButtonSFX();
        }
        else
        {
            FailButtonSFX();
        }
    }
    private void BuyShield()
    {
        if (DataPersister.Instance.CurrentGameData.totalMoney >= DataPersister.Instance.CurrentGameData.shieldCost)
        {
            // cost is subtracted from money for purchase
            DataPersister.Instance.CurrentGameData.totalMoney -= DataPersister.Instance.CurrentGameData.shieldCost;
            DataPersister.Instance.CurrentGameData.shieldCount += shieldAmount;

            // The purchase cost increases with each purchase
            DataPersister.Instance.CurrentGameData.shieldCost *= shieldCostMultiplier;

            UpdateUI();
            DataPersister.Instance.SaveCurrentGame();
            SuccessButtonSFX();
        }
        else
        {
            FailButtonSFX();
        }
    }
    private void BuyShieldHp()
    {
        if (DataPersister.Instance.CurrentGameData.totalMoney >= DataPersister.Instance.CurrentGameData.shieldHpCost)
        {
            // cost is subtracted from money for purchase
            DataPersister.Instance.CurrentGameData.totalMoney -= DataPersister.Instance.CurrentGameData.shieldHpCost;
            DataPersister.Instance.CurrentGameData.shieldHp *= shieldHpMultiplier;

            // The purchase cost increases with each purchase
            DataPersister.Instance.CurrentGameData.shieldHpCost *= shieldHpCostMultiplier;

            UpdateUI();
            DataPersister.Instance.SaveCurrentGame();
            SuccessButtonSFX();
        }
        else
        {
            FailButtonSFX();
        }
    }
    private void BuyNuke()
    {
        if (DataPersister.Instance.CurrentGameData.totalMoney >= DataPersister.Instance.CurrentGameData.nukeCost)
        {
            // cost is subtracted from money for purchase
            DataPersister.Instance.CurrentGameData.totalMoney -= DataPersister.Instance.CurrentGameData.nukeCost;
            DataPersister.Instance.CurrentGameData.nukeCount += nukeAmount;

            // The purchase cost decreases from BuyNukeLowerCost(). No code needed here.

            UpdateUI();
            DataPersister.Instance.SaveCurrentGame();
            SuccessButtonSFX();
        }
        else
        {
            FailButtonSFX();
        }
    }
    private void BuyNukeLowerCost()
    {
        if (DataPersister.Instance.CurrentGameData.totalMoney >= nukeLowerCost)
        {
            // cost is subtracted from money for purchase
            DataPersister.Instance.CurrentGameData.totalMoney -= nukeLowerCost;
            // nuke cost reduction is applied
            float calculatedNukeCostReduction = DataPersister.Instance.CurrentGameData.nukeCost / nukeLowerCostMultiplier;
            DataPersister.Instance.CurrentGameData.nukeCost = calculatedNukeCostReduction;

            UpdateUI();
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

    private void UpdateUI()
    {
        moneyText.text = DataPersister.Instance.CurrentGameData.totalMoney.ToString("F0");
        missileText.text = DataPersister.Instance.CurrentGameData.missileCount.ToString("F0");
        shieldText.text = DataPersister.Instance.CurrentGameData.shieldCount.ToString("F0");
        nukeText.text = DataPersister.Instance.CurrentGameData.nukeCount.ToString("F0");

        missileCostText.text = FormatNumber(DataPersister.Instance.CurrentGameData.missileCost);
        shieldCostText.text = FormatNumber(DataPersister.Instance.CurrentGameData.shieldCost);
        nukeCostText.text = FormatNumber(DataPersister.Instance.CurrentGameData.nukeCost);

        missileAmountText.text = missileAmount.ToString();
        shieldAmountText.text = shieldAmount.ToString();
        nukeAmountText.text = nukeAmount.ToString();

        missileDamageCostText.text = FormatNumber(DataPersister.Instance.CurrentGameData.missileDamageCost);
        shieldHpCostText.text = FormatNumber(DataPersister.Instance.CurrentGameData.shieldHpCost);
        nukeLowerCostText.text = FormatNumber(nukeLowerCost);

        // Current and potential values
        float missileDamage = DataPersister.Instance.CurrentGameData.missileDamage;
        float potentialMissileDamage = missileDamage * missileDamageMultiplier;
        missileDamagePotentialText.text = $"{FormatNumber(missileDamage)} -> {FormatNumber(potentialMissileDamage)}";
        float shieldHp = DataPersister.Instance.CurrentGameData.shieldHp;
        float potentialShieldHp = shieldHp * shieldHpMultiplier;
        shieldHpPotentialText.text = $"{FormatNumber(shieldHp)} -> {FormatNumber(potentialShieldHp)}";
        float potentialNukeCost = DataPersister.Instance.CurrentGameData.nukeCost / nukeLowerCostMultiplier;
        nukeCostPotentialText.text = $"{FormatNumber(DataPersister.Instance.CurrentGameData.nukeCost)} -> {FormatNumber(potentialNukeCost)}";

        UpdateButtonState(buyMissileButton, buyMissileButtonVisualCue, (int)DataPersister.Instance.CurrentGameData.missileCost);
        UpdateButtonState(buyMissileDamageButton, buyMissileDamageButtonVisualCue, (int)DataPersister.Instance.CurrentGameData.missileDamageCost);
        UpdateButtonState(buyShieldButton, buyShieldButtonVisualCue, (int)DataPersister.Instance.CurrentGameData.shieldCost);
        UpdateButtonState(buyShieldHpButton, buyShieldHpButtonVisualCue, (int)DataPersister.Instance.CurrentGameData.shieldHpCost);
        UpdateButtonState(buyNukeButton, buyNukeButtonVisualCue, (int)DataPersister.Instance.CurrentGameData.nukeCost);
        UpdateButtonState(buyNukeLowerCostButton, buyNukeLowerCostButtonVisualCue, (int)nukeLowerCost);
    }

    private void UpdateButtonState(Button button, TextMeshProUGUI buttonVisualCue, int cost)
    {
        bool hasEnoughMoney = DataPersister.Instance.CurrentGameData.totalMoney >= cost;
        button.interactable = hasEnoughMoney;
        if (buttonVisualCue != null)
        {
            buttonVisualCue.color = hasEnoughMoney ? canBuyUpgradeButtonColour : cantBuyUpgradeButtonColour;
        }
    }

    private string FormatNumber(float number)
    {
        // Round to nearest whole number
        int roundedNumber = Mathf.RoundToInt(number);

        if (roundedNumber >= 1000000)
        {
            // For millions
            float millions = roundedNumber / 1000000f;
            return millions.ToString("0") + "M";
        }
        else if (roundedNumber >= 1000)
        {
            // For thousands
            float thousands = roundedNumber / 1000f;
            return thousands.ToString("0") + "k";
        }
        else
        {
            // For numbers less than 1000
            return roundedNumber.ToString();
        }
    }

}
