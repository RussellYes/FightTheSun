using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvestingUI : MonoBehaviour
{
    public static event Action AdRequestDividendReward;

    private SFXManager sFXManager;

    [Header("UI References")]
    [SerializeField] private Button openStoreButton;
    [SerializeField] private Button closeStoreButton;
    [SerializeField] private GameObject storeHolder;
    [SerializeField] private GameObject storeButtonHolder;
    [SerializeField] private float uIOpenCloseLerpTime = 1;

    [Header("Currency Displays")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI company1Shares;
    [SerializeField] private TextMeshProUGUI company2Shares;
    [SerializeField] private TextMeshProUGUI company3Shares;
    [SerializeField] private TextMeshProUGUI company4Shares;
    [SerializeField] private TextMeshProUGUI company5Shares;

    [Header("Chart UI")]
    [SerializeField] private GameObject chartPointPrefab;
    [SerializeField] private GameObject chartUIImage;
    [SerializeField] private TextMeshProUGUI[] dayTexts = new TextMeshProUGUI[7];
    [SerializeField] private TextMeshProUGUI chartValueMinText;
    [SerializeField] private TextMeshProUGUI chartValueOneQuarterText;
    [SerializeField] private TextMeshProUGUI chartValueHalfText;
    [SerializeField] private TextMeshProUGUI chartValueThreeQuarterText;
    [SerializeField] private TextMeshProUGUI chartValueMaxText;

    [Header("Company Data")]
    [SerializeField] private CompanyData[] companies = new CompanyData[5];
    // Name texts
    [SerializeField] private TextMeshProUGUI company1NameText;
    [SerializeField] private TextMeshProUGUI company2NameText;
    [SerializeField] private TextMeshProUGUI company3NameText;
    [SerializeField] private TextMeshProUGUI company4NameText;
    [SerializeField] private TextMeshProUGUI company5NameText;

    [Header("Trade Buttons")]
    [SerializeField] private Image[] tradeBackgroundImages;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private TextMeshProUGUI buyButtonVisualCue;
    [SerializeField] private TextMeshProUGUI sellButtonVisualCue;
    [SerializeField] private TextMeshProUGUI buyShareMoneyText;
    [SerializeField] private TextMeshProUGUI sellShareMoneyText;
    [SerializeField] private Button increasePurchaseAmountButton;
    [SerializeField] private Button decreasePurchaseAmountButton;
    private int tradeShareQuantity = 1;
    [SerializeField] private TextMeshProUGUI tradeShareQuantityText;
    [SerializeField] private Button changeCompanyTradeButton;
    [SerializeField] private Button selectCompany1TradeButton;
    [SerializeField] private Button selectCompany2TradeButton;
    [SerializeField] private Button selectCompany3TradeButton;
    [SerializeField] private Button selectCompany4TradeButton;
    [SerializeField] private Button selectCompany5TradeButton;
    [SerializeField] private int companyIndex;
    [SerializeField] private TextMeshProUGUI tradingCompanyNameText;

    [Header("Stock Price Settings")]
    [SerializeField] private float minStockPrice = 0f;
    [SerializeField] private float maxStockPrice = 100f;
    [SerializeField] private float initialPriceRangeMin = 20f;
    [SerializeField] private float initialPriceRangeMax = 80f;

    [Header("Button State Colors")]
    [SerializeField] private Color canTradeUpgradeButtonColour;
    [SerializeField] private Color cantTradeUpgradeButtonColour;

    [Header("Time")]
    [SerializeField] private TextMeshProUGUI todaysDayText;
    [SerializeField] private TextMeshProUGUI todaysDateText;
    private float currentDayOfTheWeek;
    private float currentTime;
    private float timeDifference;
    private bool isCheckTime = true;
    [SerializeField] private float timeCheckTime = 60f;
    private float timeCheckCountdown;
    private bool isNewDay = false;

    [Header("Scrolling Text Settings")]
    [SerializeField] private float scrollSpeed = 50f;
    [SerializeField] private float textResetGap = 100f; // Gap before resetting scroll position
    private RectTransform todaysDateTextRect;
    private float textWidth;
    private string scrollingTextContent;
    private bool isTextInitialized = false;

    [Header("Dividend")]
    [SerializeField] private Button dividendButton;
    [SerializeField] private GameObject adImage;
    [SerializeField] private GameObject moneyIcon;
    [SerializeField] private TextMeshProUGUI dividendButtonText;
    private float dailyDividendCount;
    private float dailyDividendCountMax = 3;
    private float dividendPayoutPercentageMin = 0.01f;
    private float dividendPayoutPercentageMax = 0.03f;
    private float calculatedDividend;
    [SerializeField] private GameObject adRewardRecievedHolder;
    [SerializeField] private TextMeshProUGUI adRewardRecievedText;

    [Header("SFX")]
    [SerializeField] private AudioClip[] storeOpenCloseSFX;
    [SerializeField] private AudioClip buySuccessSFX;
    [SerializeField] private AudioClip buyFailSFX;
    [SerializeField] private AudioClip[] singleButtonSFX;
    [SerializeField] private AudioClip[] doubleButtonSFX;

    [System.Serializable]
    public class CompanyData
    {
        public string companyName;
        public Color companyColor;
        public LineRenderer lineRenderer;
        public List<GameObject> lineSegments;
        public TextMeshProUGUI[] priceTexts = new TextMeshProUGUI[7];
        public float[] sharePrices = new float[7];
        public float variance;
        public float futureSharePrice;
        public string sharesOwnedFieldName;
    }

    [Header("Greeting")]
    [SerializeField] private GameObject greetingHolder;
    [SerializeField] private Button closeGreetingButton;
    [SerializeField] private Button openGreetingButton;
    [SerializeField] private TextMeshProUGUI greetingText;
    private int greetingTextIndex = 0;
    [SerializeField] private GameObject arrowPrefab;
    private GameObject currentArrow;

    #region Unity Events
    private void OnEnable()
    {
        DataPersister.InitializationComplete += OnInitializationComplete;
        openStoreButton.onClick.AddListener(OpenStore);
        closeStoreButton.onClick.AddListener(CloseStore);
        openGreetingButton.onClick.AddListener(GreetingsWindow);
        closeGreetingButton.onClick.AddListener(GreetingsDialogue);
        increasePurchaseAmountButton.onClick.AddListener(HandleIncreaseTradeAmountButton);
        decreasePurchaseAmountButton.onClick.AddListener(HandleDecreaseTradeAmountButton);
        buyButton.onClick.AddListener(HandleBuyButton);
        sellButton.onClick.AddListener(HandleSellButton);
        changeCompanyTradeButton.onClick.AddListener(HandleChangeCompanyTradeButton);
        selectCompany1TradeButton.onClick.AddListener(() => HandleSelectCompanyTradeButton(0));
        selectCompany2TradeButton.onClick.AddListener(() => HandleSelectCompanyTradeButton(1));
        selectCompany3TradeButton.onClick.AddListener(() => HandleSelectCompanyTradeButton(2));
        selectCompany4TradeButton.onClick.AddListener(() => HandleSelectCompanyTradeButton(3));
        selectCompany5TradeButton.onClick.AddListener(() => HandleSelectCompanyTradeButton(4));
        dividendButton.onClick.AddListener(HandleDividendButton);
        RewardedAdPlayer.RewardGranted += HandleAdRewardGranted;
    }

    private void OnDisable()
    {
        DataPersister.InitializationComplete -= OnInitializationComplete;
        openStoreButton.onClick.RemoveListener(OpenStore);
        closeStoreButton.onClick.RemoveListener(CloseStore);
        openGreetingButton.onClick.RemoveListener(GreetingsWindow);
        closeGreetingButton.onClick.RemoveListener(GreetingsDialogue);
        increasePurchaseAmountButton.onClick.RemoveListener(HandleIncreaseTradeAmountButton);
        decreasePurchaseAmountButton.onClick.RemoveListener(HandleDecreaseTradeAmountButton);
        buyButton.onClick.RemoveListener(HandleBuyButton);
        sellButton.onClick.RemoveListener(HandleSellButton);
        changeCompanyTradeButton.onClick.RemoveListener(HandleChangeCompanyTradeButton);
        selectCompany1TradeButton.onClick.RemoveAllListeners();
        selectCompany2TradeButton.onClick.RemoveAllListeners();
        selectCompany3TradeButton.onClick.RemoveAllListeners();
        selectCompany4TradeButton.onClick.RemoveAllListeners();
        selectCompany5TradeButton.onClick.RemoveAllListeners();
        dividendButton.onClick.RemoveListener(HandleDividendButton);
        RewardedAdPlayer.RewardGranted -= HandleAdRewardGranted;
    }

    private void Start()
    {
        storeHolder.SetActive(false);
        adRewardRecievedHolder.SetActive(false);
        timeCheckCountdown = timeCheckTime;
    }

    private void Update()
    {
        if (storeHolder.activeSelf && isCheckTime)
        {
            isCheckTime = false;
            GetTime();
            UpdateUI();
        }

        TimeCheckTimer();

        if (isNewDay)
        {
            DailyCompanyLootShip();
            ShiftDaysOfTheWeek();
            isNewDay = false;
        }

        if (isTextInitialized && storeHolder.activeSelf)
        {
            UpdateScrollingText();
        }
    }
    #endregion

    #region Initialization
    private void OnInitializationComplete()
    {
        storeHolder.SetActive(false);
        sFXManager = SFXManager.Instance;
        LoadCompanyDataFromSave();
        InitializeCompanies();
        InitializeLineSegments();
        InitializeStockPrices();
    }

    private void InitializeCompanies()
    {
        // Initialize company data
        companies[0].sharesOwnedFieldName = "company1Shares";
        companies[1].sharesOwnedFieldName = "company2Shares";
        companies[2].sharesOwnedFieldName = "company3Shares";
        companies[3].sharesOwnedFieldName = "company4Shares";
        companies[4].sharesOwnedFieldName = "company5Shares";
    }

    private void InitializeLineSegments()
    {
        // Initialize the line segments list for each company
        foreach (CompanyData company in companies)
        {
            company.lineSegments = new List<GameObject>();
        }
    }
    #endregion

    #region UI Management
    private void OpenStore()
    {
        SingleButtonClick();
        storeHolder.SetActive(true);
        adRewardRecievedHolder.SetActive(false);
        GetTime();
        UpdateUI();
        PlayOpenCloseSFX();
        StartCoroutine(OpenStoreLerp());
        StartCoroutine(InitializeScrollingText());
        if (!DataPersister.Instance.CurrentGameData.hasOpenedInvesting)
        {
            // If this is the first time opening ShipUpgrades, show the greeting window
            GreetingsWindow();
        }
    }

    private void CloseStore()
    {
        SingleButtonClick();

        storeHolder.SetActive(false);
        PlayOpenCloseSFX();
        StartCoroutine(CloseStoreLerp());
        StopCoroutine(InitializeScrollingText());
    }


    private IEnumerator OpenStoreLerp()
    {
        RectTransform rectTransform = storeButtonHolder.GetComponent<RectTransform>();
        Vector3 originalPosition = rectTransform.localPosition;
        Vector3 startPosition = originalPosition + new Vector3(0, 2000, 0);
        rectTransform.localPosition = startPosition;

        float elapsedTime = 0f;
        while (elapsedTime < uIOpenCloseLerpTime)
        {
            rectTransform.localPosition = Vector3.Lerp(startPosition, originalPosition, elapsedTime / uIOpenCloseLerpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rectTransform.localPosition = originalPosition;
    }

    private IEnumerator CloseStoreLerp()
    {
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
        rectTransform.localPosition = originalPosition;
    }

    private void PlayOpenCloseSFX()
    {
        AudioClip sFX = storeOpenCloseSFX[UnityEngine.Random.Range(0, storeOpenCloseSFX.Length)];
        sFXManager?.PlaySFX(sFX);
    }
    #endregion

    #region Company Management
    private void LoadCompanyDataFromSave()
    {
        var gameData = DataPersister.Instance.CurrentGameData;

        for (int i = 0; i < companies.Length; i++)
        {
            string companyPrefix = $"company{i + 1}";
            companies[i].sharePrices[0] = GetCompanyPrice(gameData, companyPrefix, "Sunday");
            companies[i].sharePrices[1] = GetCompanyPrice(gameData, companyPrefix, "Monday");
            companies[i].sharePrices[2] = GetCompanyPrice(gameData, companyPrefix, "Tuesday");
            companies[i].sharePrices[3] = GetCompanyPrice(gameData, companyPrefix, "Wednesday");
            companies[i].sharePrices[4] = GetCompanyPrice(gameData, companyPrefix, "Thursday");
            companies[i].sharePrices[5] = GetCompanyPrice(gameData, companyPrefix, "Friday");
            companies[i].sharePrices[6] = GetCompanyPrice(gameData, companyPrefix, "Saturday");
        }
    }

    private void InitializeStockPrices()
    {
        var gameData = DataPersister.Instance.CurrentGameData;
        bool needsInitialization = false;

        // Check if prices need to be initialized
        for (int i = 0; i < companies.Length; i++)
        {
            for (int day = 0; day < 7; day++)
            {
                if (companies[i].sharePrices[day] <= 0)
                {
                    needsInitialization = true;
                    break;
                }
            }
            if (needsInitialization) break;
        }

        if (needsInitialization)
        {
            Debug.Log("InvestingUI InitializeStockPrices - Initializing stock prices for all companies");

            for (int i = 0; i < companies.Length; i++)
            {
                // Start with random initial price
                float basePrice = UnityEngine.Random.Range(initialPriceRangeMin, initialPriceRangeMax);

                for (int day = 0; day < 7; day++)
                {
                    // Add some variation for each day
                    float variation = UnityEngine.Random.Range(-5f, 5f);
                    companies[i].sharePrices[day] = Mathf.Clamp(basePrice + variation, minStockPrice, maxStockPrice);
                }
            }

            SaveCompanyData();
        }
    }

    private float GetCompanyPrice(GameData gameData, string companyPrefix, string day)
    {
        var field = typeof(GameData).GetField($"{companyPrefix}{day}SharePrice");
        return field != null ? (float)field.GetValue(gameData) : 0f;
    }

    private void SetCompanyPrice(GameData gameData, string companyPrefix, string day, float value)
    {
        var field = typeof(GameData).GetField($"{companyPrefix}{day}SharePrice");
        field?.SetValue(gameData, value);
    }

    private float GetCurrentSharePrice(int companyIndex)
    {
        int dayIndex = Mathf.FloorToInt(currentDayOfTheWeek);
        if (dayIndex >= 0 && dayIndex < 7)
        {
            return companies[companyIndex].sharePrices[dayIndex];
        }
        return companies[companyIndex].sharePrices[0]; // Default to Sunday
    }

    private float GetSharesOwned(int companyIndex)
    {
        var field = typeof(GameData).GetField(companies[companyIndex].sharesOwnedFieldName);
        return field != null ? (float)field.GetValue(DataPersister.Instance.CurrentGameData) : 0f;
    }

    private void SetSharesOwned(int companyIndex, float value)
    {
        var field = typeof(GameData).GetField(companies[companyIndex].sharesOwnedFieldName);
        field?.SetValue(DataPersister.Instance.CurrentGameData, value);
    }
    #endregion

    #region Time Management
    private void GetTime()
    {
        float lastSavedDayOfTheWeek = DataPersister.Instance.CurrentGameData.investingDayOfTheWeek;
        float lastSavedCurrentTime = DataPersister.Instance.CurrentGameData.investingSavedTime;

        currentDayOfTheWeek = GetCurrentDayOfWeekAsFloat();
        currentTime = GetCurrentTimeAsFloat();
        Debug.Log($"InvestingUI GetTime - Current Day: {currentDayOfTheWeek}, Current Time: {currentTime}, last Saved Day: {lastSavedDayOfTheWeek} ");

        isNewDay = lastSavedDayOfTheWeek != currentDayOfTheWeek;
        dailyDividendCount = DataPersister.Instance.CurrentGameData.dailyDividendCount;

        if (isNewDay)
        {
            Debug.Log($"InvestingUI GetTime - New day detected! Resetting dividend count from {dailyDividendCount} to {dailyDividendCountMax}");
            dailyDividendCount = dailyDividendCountMax;
        }

        if (lastSavedCurrentTime != 0f)
        {
            timeDifference = currentTime - lastSavedCurrentTime;
        }

        DataPersister.Instance.CurrentGameData.investingDayOfTheWeek = currentDayOfTheWeek;
        DataPersister.Instance.CurrentGameData.investingSavedTime = currentTime;
        DataPersister.Instance.CurrentGameData.dailyDividendCount = dailyDividendCount;
        DataPersister.Instance.SaveCurrentGame();
    }

    private void TimeCheckTimer()
    {
        timeCheckCountdown -= Time.deltaTime;
        if (timeCheckCountdown <= 0f)
        {
            isCheckTime = true;
            timeCheckCountdown = timeCheckTime;
        }
    }

    // Convert day of week to float (0-6)
    private float GetCurrentDayOfWeekAsFloat()
    {
        if (!TimeManager.Instance.IsTimeAvailable())
        {
            Debug.LogWarning("InvestingUI GetCurrentDayOfWeekAsFloat - WorldTimeAPI time not available, using fallback");
            return (float)System.DateTime.UtcNow.DayOfWeek;
        }

        string dayString = TimeManager.Instance.GetDayOfWeek();
        return ConvertDayOfWeekToFloat(dayString);
    }

    // Convert time to float (hours as decimal)
    private float GetCurrentTimeAsFloat()
    {
        if (!TimeManager.Instance.IsTimeAvailable())
        {
            Debug.LogWarning("InvestingUI GetCurrentTimeAsFloat - WorldTimeAPI time not available, using fallback");
            var now = System.DateTime.UtcNow;
            return now.Hour + now.Minute / 60f;
        }

        DateTime gmtTime = TimeManager.Instance.GetGMTTime();
        return gmtTime.Hour + gmtTime.Minute / 60f;
    }

    // Convert day string to float (Sunday = 0 to Saturday = 6)
    private float ConvertDayOfWeekToFloat(string dayOfWeek)
    {
        switch (dayOfWeek.ToLower())
        {
            case "sunday": return 0f;
            case "monday": return 1f;
            case "tuesday": return 2f;
            case "wednesday": return 3f;
            case "thursday": return 4f;
            case "friday": return 5f;
            case "saturday": return 6f;
            default:
                Debug.LogWarning($"InvestingUI ConvertDayOfWeekToFloat - Unknown day of week: {dayOfWeek}, defaulting to Sunday");
                return 0f;
        }
    }
    #endregion

    #region UI Updates
    private void UpdateUI()
    {
        UpdateDayAndDateText();
        UpdateMoneyDisplay();
        UpdateCompanyNames();
        UpdateSharesOwnedDisplays();
        UpdateDayTexts();
        UpdateButtonStates();
        UpdateDividendButton();
        UpdateTradingCompanyName();
        UpdateTradeBackgroundColors();
        UpdateChartValueTexts();
        ClearOldChartPointsAndLineRenderers();
        UpdateChartPointsAndLines();
        DebugChartPointPrefab();
    }

    private void UpdateDayAndDateText()
    {
        if (todaysDateText == null) return;

        try
        {
            string dayOfWeek = GetCurrentDayOfWeekString();
            string dateString = GetCurrentDateString();

            // Get current share prices for all companies
            string company1CurrentSharePrice = GetCurrentSharePrice(0).ToString("0.00");
            string company2CurrentSharePrice = GetCurrentSharePrice(1).ToString("0.00");
            string company3CurrentSharePrice = GetCurrentSharePrice(2).ToString("0.00");
            string company4CurrentSharePrice = GetCurrentSharePrice(3).ToString("0.00");
            string company5CurrentSharePrice = GetCurrentSharePrice(4).ToString("0.00");

            // Get company names
            string company1Name = companies[0].companyName;
            string company2Name = companies[1].companyName;
            string company3Name = companies[2].companyName;
            string company4Name = companies[3].companyName;
            string company5Name = companies[4].companyName;

            // Scrolling text content
            string scrollingText = $"7 day price history chart. --- {dateString} --- {company1Name} {company1CurrentSharePrice} --- {company2Name} {company2CurrentSharePrice} --- {company3Name} {company3CurrentSharePrice} --- {company4Name} {company4CurrentSharePrice} --- {company5Name} {company5CurrentSharePrice} ---";

            todaysDayText.text = $"{dayOfWeek}";
            SetupScrollingText(scrollingText);

            Debug.Log($"InvestingUI UpdateDayAndTimeText - Day: {dayOfWeek} Date: {dateString}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"InvestingUI UpdateDayAndTimeText - Error updating day and time text: {e.Message}");
            todaysDateText.text = "Date Unavailable";
        }
    }

    private string GetCurrentDayOfWeekString()
    {
        if (!TimeManager.Instance.IsTimeAvailable())
        {
            Debug.LogWarning("InvestingUI GetCurrentDayOfWeekString - WorldTimeAPI time not available, using fallback");
            return System.DateTime.UtcNow.DayOfWeek.ToString();
        }

        return TimeManager.Instance.GetDayOfWeek();
    }

    private string GetCurrentDateString()
    {
        if (!TimeManager.Instance.IsTimeAvailable())
        {
            Debug.LogWarning("InvestingUI GetCurrentDateString - WorldTimeAPI time not available, using fallback");
            var now = System.DateTime.UtcNow;
            return now.ToString("MMM dd, yyyy");
        }

        DateTime gmtTime = TimeManager.Instance.GetGMTTime();
        return gmtTime.ToString("MMM dd, yyyy");
    }

    private void UpdateMoneyDisplay()
    {
        moneyText.text = DataPersister.Instance.CurrentGameData.totalMoney.ToString("0");
        tradeShareQuantityText.text = tradeShareQuantity.ToString();
    }

    private void UpdateCompanyNames()
    {
        company1NameText.text = companies[0].companyName;
        company2NameText.text = companies[1].companyName;
        company3NameText.text = companies[2].companyName;
        company4NameText.text = companies[3].companyName;
        company5NameText.text = companies[4].companyName;
    }

    private void UpdateSharesOwnedDisplays()
    {
        company1Shares.text = GetSharesOwned(0).ToString("0");
        company2Shares.text = GetSharesOwned(1).ToString("0");
        company3Shares.text = GetSharesOwned(2).ToString("0");
        company4Shares.text = GetSharesOwned(3).ToString("0");
        company5Shares.text = GetSharesOwned(4).ToString("0");
    }

    private void UpdateDayTexts()
    {
        string[] dayNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

        // Get the current day index (0-6) from TimeManager
        float currentDayFloat = GetCurrentDayOfWeekAsFloat();
        int currentDayIndex = Mathf.FloorToInt(currentDayFloat);

        // Ensure the index is within valid range
        if (currentDayIndex < 0 || currentDayIndex > 6)
        {
            currentDayIndex = 0; // Default to Sunday if out of range
            Debug.LogWarning($"InvestingUI UpdateDayTexts - Invalid current day index: {currentDayIndex}, defaulting to 0");
        }

        // Update day texts starting from current day and working backwards.
        for (int i = 0; i < dayTexts.Length && i < dayNames.Length; i++)
        {
            if (dayTexts[i] != null)
            {
                // Calculate the 7 day index for this position (wrapping around)
                int days = 7;
                int dayIndex = (currentDayIndex - (6 - i) + days) % days;
                dayTexts[i].text = dayNames[dayIndex];
            }
        }

        Debug.Log($"InvestingUI UpdateDayTexts - Current day index: {currentDayIndex}, Day order: {dayTexts[0].text}, {dayTexts[1].text}, {dayTexts[2].text}, {dayTexts[3].text}, {dayTexts[4].text}, {dayTexts[5].text}, {dayTexts[6].text}");
    }

    private void UpdateDividendButton()
    {
        dailyDividendCount = DataPersister.Instance.CurrentGameData.dailyDividendCount;
        Debug.Log($"InvestingUI UpdateDividendButton - dailyDividendCount loaded: {dailyDividendCount}");

        // Check if player owns any shares across all companies
        bool ownsAnyShares = false;
        for (int i = 0; i < companies.Length; i++)
        {
            if (GetSharesOwned(i) > 0)
            {
                ownsAnyShares = true;
                break;
            }
        }

        if (dailyDividendCount >= 1 && ownsAnyShares)
        {
            dividendButton.interactable = true;
            if (dailyDividendCount == 1)
            {
                dividendButtonText.text = $"Dividend";
                moneyIcon.SetActive(true);
                adImage.SetActive(false);
            }
            else // dailyDividendCount >= 2
            {
                dividendButtonText.text = $"Dividend {dailyDividendCount}/{dailyDividendCountMax}";
                moneyIcon.SetActive(false);
                adImage.SetActive(true);
            }
        }
        else
        {
            dividendButton.interactable = false;
            if (!ownsAnyShares)
            {
                dividendButtonText.text = $"No Shares";
            }
            else
            {
                dividendButtonText.text = $"{dailyDividendCount}/{dailyDividendCountMax}";
            }
            moneyIcon.SetActive(true);
            adImage.SetActive(false);
        }
    }

    private void UpdateButtonStates()
    {
        // Only process the currently selected company
        float currentPrice = GetCurrentSharePrice(companyIndex);
        float totalCost = currentPrice * tradeShareQuantity;
        float sharesOwned = GetSharesOwned(companyIndex);

        UpdateBuyButtonState(buyButton, buyButtonVisualCue,
            DataPersister.Instance.CurrentGameData.totalMoney, totalCost);
        UpdateSellButtonState(sellButton, sellButtonVisualCue,
            sharesOwned, tradeShareQuantity);

        UpdateBuySellMoneyTexts(currentPrice, tradeShareQuantity, sharesOwned);
    }

    private void UpdateBuyButtonState(Button button, TextMeshProUGUI buttonVisualCue, float currencyOwned, float totalCost)
    {
        bool hasEnoughMoney = currencyOwned >= totalCost;
        button.interactable = hasEnoughMoney;
        if (buttonVisualCue != null)
        {
            buttonVisualCue.color = hasEnoughMoney ? canTradeUpgradeButtonColour : cantTradeUpgradeButtonColour;
        }
    }

    private void UpdateSellButtonState(Button button, TextMeshProUGUI buttonVisualCue, float sharesOwned, int sellQuantity)
    {
        bool hasEnoughShares = sharesOwned >= sellQuantity;
        button.interactable = hasEnoughShares;
        if (buttonVisualCue != null)
        {
            buttonVisualCue.color = hasEnoughShares ? canTradeUpgradeButtonColour : cantTradeUpgradeButtonColour;
        }
    }

    private void UpdateBuySellMoneyTexts(float currentPrice, int tradeQuantity, float sharesOwned)
    {
        // Update buy share money text - cost to buy the specified quantity
        if (buyShareMoneyText != null)
        {
            float buyTotalCost = currentPrice * tradeQuantity;
            buyShareMoneyText.text = FormatMoneyText(buyTotalCost);
        }

        // Update sell share money text - total value of all shares owned
        if (sellShareMoneyText != null)
        {
            float actualSellQuantity = Mathf.Min(tradeQuantity, sharesOwned);
            float sellTotalValue = currentPrice * actualSellQuantity;
            sellShareMoneyText.text = FormatMoneyText(sellTotalValue);
        }
    }

    private string FormatMoneyText(float value)
    {
        // Text should be 5 characters max (ones, tens, hundreds +2 decimals).
        string formattedText;

        if (value >= 1000000f) // 1 million or more
        {
            float millions = value / 1000000f;
            formattedText = "$" + millions.ToString("0.##") + "M";
        }
        else if (value >= 1000f) // 1 thousand or more
        {
            float thousands = value / 1000f;
            formattedText = "$" + thousands.ToString("0.##") + "k";
        }
        else // Less than 1000
        {
            formattedText = "$" + value.ToString("0.##");
        }

        return formattedText;
    }

    private void UpdateTradingCompanyName()
    {
        if (tradingCompanyNameText != null && companies.Length > 0)
        {
            tradingCompanyNameText.text = companies[companyIndex].companyName;
        }
    }

    private void UpdateTradeBackgroundColors()
    {
        // Update trade background images with current company color
        if (tradeBackgroundImages != null && tradeBackgroundImages.Length > 0)
        {
            Color companyColor = companies[companyIndex].companyColor;
            foreach (Image backgroundImage in tradeBackgroundImages)
            {
                if (backgroundImage != null)
                {
                    backgroundImage.color = companyColor;
                }
            }
            Debug.Log($"InvestingUI UpdateTradeBackgroundColors - Updated trade backgrounds to {companyColor}");
        }
    }
    #endregion

    #region Scrolling Text Implementation
    private IEnumerator InitializeScrollingText()
    {
        yield return null;

        if (todaysDateText != null)
        {
            // Set up text for proper clipping
            todaysDateText.overflowMode = TextOverflowModes.Truncate;
            todaysDateText.horizontalAlignment = HorizontalAlignmentOptions.Left;
            todaysDateText.textWrappingMode = TextWrappingModes.NoWrap;
            isTextInitialized = true;
        }
    }

    private void UpdateScrollingText()
    {
        if (!isTextInitialized || string.IsNullOrEmpty(scrollingTextContent)) return;

        float textBoxWidth = todaysDateText.rectTransform.rect.width;

        // Simple scroll position calculation
        float scrollPos = (Time.time * scrollSpeed) % (textWidth + textResetGap);

        // Reset when we've scrolled past the full text
        if (scrollPos > textWidth)
        {
            scrollPos = 0;
        }

        UpdateVisibleText(scrollPos, textBoxWidth);
    }

    private void UpdateVisibleText(float scrollPos, float textBoxWidth)
    {
        // Calculate which characters should be visible based on scroll position
        int totalChars = scrollingTextContent.Length;
        float charWidth = textWidth / totalChars;

        // Find start character index
        int startChar = Mathf.FloorToInt(scrollPos / charWidth);
        startChar = Mathf.Clamp(startChar, 0, totalChars - 1);

        // Calculate how many characters can fit in the text box
        int visibleChars = Mathf.CeilToInt(textBoxWidth / charWidth);
        int endChar = Mathf.Min(startChar + visibleChars, totalChars);

        string visibleText = scrollingTextContent.Substring(startChar, endChar - startChar);

        // Only add beginning text if we've scrolled past the end
        if (endChar >= totalChars && scrollPos + textBoxWidth > textWidth)
        {
            int remainingChars = visibleChars - (endChar - startChar);
            if (remainingChars > 0)
            {
                visibleText += scrollingTextContent.Substring(0, Mathf.Min(remainingChars, totalChars));
            }
        }

        todaysDateText.text = visibleText;
    }

    private void SetupScrollingText(string text)
    {
        if (todaysDateText == null) return;

        scrollingTextContent = text + "        "; // Add spacing for loop
        todaysDateText.text = scrollingTextContent;

        // Calculate text width
        Canvas.ForceUpdateCanvases();
        textWidth = todaysDateText.preferredWidth;

        Debug.Log($"InvestingUI SetupScrollingText - Full text: '{scrollingTextContent}', Text width: {textWidth}, Text box width: {todaysDateText.rectTransform.rect.width}");
    }
    #endregion


    #region Chart Management

    private void UpdateChartValueTexts()
    {
        if (chartValueMinText == null || chartValueMaxText == null) return;

        // Calculate quarter values.
        float priceRange = maxStockPrice - minStockPrice;
        float oneQuarterPrice = minStockPrice + (priceRange * 0.25f);
        float halfPrice = minStockPrice + (priceRange * 0.5f);
        float threeQuarterPrice = minStockPrice + (priceRange * 0.75f);

        chartValueMinText.text = Mathf.RoundToInt(minStockPrice) + "-".ToString();
        chartValueOneQuarterText.text = Mathf.RoundToInt(oneQuarterPrice) + "-".ToString();
        chartValueHalfText.text = Mathf.RoundToInt(halfPrice) + "-".ToString();
        chartValueThreeQuarterText.text = Mathf.RoundToInt(threeQuarterPrice) + "-".ToString();
        chartValueMaxText.text = Mathf.RoundToInt(maxStockPrice) + "-".ToString();

        Debug.Log($"InvestingUI UpdateChartValueTexts - Min: {minStockPrice}, 1/4: {oneQuarterPrice}, 1/2: {halfPrice}, 3/4: {threeQuarterPrice}, Max: {maxStockPrice}");
    }

    private void UpdateChartPointsAndLines()
    {
        if (chartUIImage == null) return;

        // Get the current day order to match the day labels
        float currentDayFloat = GetCurrentDayOfWeekAsFloat();
        int currentDayIndex = Mathf.FloorToInt(currentDayFloat);

        foreach (CompanyData company in companies)
        {
            List<Vector2> pointPositions = new List<Vector2>();

            // Create chart points in the same order as the day labels
            for (int displayIndex = 0; displayIndex < 7; displayIndex++)
            {
                // Calculate which day's price should be shown at this display position
                int days = 7;
                int dayIndex = (currentDayIndex - (6 - displayIndex) + days) % days;

                // Get the price for the correct day
                float sharePrice = company.sharePrices[dayIndex];
                Vector2 canvasPosition = CalculateChartPointCanvasPosition(sharePrice, displayIndex);

                // Create chart point
                GameObject chartPoint = Instantiate(chartPointPrefab, chartUIImage.transform);
                chartPoint.tag = "ChartPoint";

                RectTransform pointRect = chartPoint.GetComponent<RectTransform>();
                if (pointRect != null)
                {
                    pointRect.anchoredPosition = canvasPosition;
                }

                // Set color
                Image pointImage = chartPoint.GetComponentInChildren<Image>();
                if (pointImage == null) pointImage = chartPoint.GetComponent<Image>();
                if (pointImage != null) pointImage.color = company.companyColor;

                pointPositions.Add(canvasPosition);
            }

            // Clear old line segments
            if (company.lineSegments != null)
            {
                foreach (GameObject segment in company.lineSegments)
                {
                    if (segment != null) Destroy(segment);
                }
                company.lineSegments.Clear();
            }
            else
            {
                company.lineSegments = new List<GameObject>();
            }

            // Create UI image line segments between points
            for (int i = 0; i < pointPositions.Count - 1; i++)
            {
                CreateUILineSegment(company, pointPositions[i], pointPositions[i + 1]);
            }
        }
    }

    private void CreateUILineSegment(CompanyData company, Vector2 startPos, Vector2 endPos)
    {
        GameObject lineSegment = new GameObject($"{company.companyName}_LineSegment");
        lineSegment.transform.SetParent(chartUIImage.transform);

        // Reset transform to avoid inherited scaling
        lineSegment.transform.localPosition = Vector3.zero;
        lineSegment.transform.localRotation = Quaternion.identity;
        lineSegment.transform.localScale = Vector3.one;

        // Add and configure RectTransform
        RectTransform rectTransform = lineSegment.AddComponent<RectTransform>();

        // Add and configure Image
        Image lineImage = lineSegment.AddComponent<Image>();
        lineImage.color = company.companyColor;

        // Calculate line properties
        Vector2 direction = endPos - startPos;
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Position and size the line segment with fixed scale
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = startPos + direction * 0.5f;
        rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
        rectTransform.localScale = Vector3.one; // Ensure no scaling

        // Store reference
        company.lineSegments.Add(lineSegment);
    }

    private Vector2 CalculateChartPointCanvasPosition(float sharePrice, int displayIndex)
    {
        if (chartUIImage == null) return Vector2.zero;

        RectTransform chartRect = chartUIImage.GetComponent<RectTransform>();
        if (chartRect == null) return Vector2.zero;

        // Get the size of the chart UI element
        Vector2 chartSize = chartRect.rect.size;
        Vector2 chartPivot = chartRect.pivot;

        // Calculate normalized positions within the chart
        float normalizedX = displayIndex / 6f;
        float normalizedY = Mathf.InverseLerp(minStockPrice, maxStockPrice, sharePrice);

        // Convert to local coordinates within the chart
        float xPos = (normalizedX - chartPivot.x) * chartSize.x;
        float yPos = (normalizedY - chartPivot.y) * chartSize.y;

        // Padding to keep points away from edges
        float xPadding = chartSize.x * 0.05f;
        float yPadding = chartSize.y * 0.05f;

        xPos = Mathf.Clamp(xPos, -chartSize.x * 0.5f + xPadding, chartSize.x * 0.5f - xPadding);
        yPos = Mathf.Clamp(yPos, -chartSize.y * 0.5f + yPadding, chartSize.y * 0.5f - yPadding);

        return new Vector2(xPos, yPos);
    }

    private void ClearOldChartPointsAndLineRenderers()
    {
        if (chartUIImage == null) return;

        // Clear chart points
        foreach (Transform child in chartUIImage.transform)
        {
            if (child.CompareTag("ChartPoint"))
            {
                Destroy(child.gameObject);
            }
        }

        // Clear UI line segments
        foreach (CompanyData company in companies)
        {
            if (company.lineSegments != null)
            {
                foreach (GameObject segment in company.lineSegments)
                {
                    if (segment != null) Destroy(segment);
                }
                company.lineSegments.Clear();
            }

            // Clear the old LineRenderer if it exists
            if (company.lineRenderer != null)
            {
                company.lineRenderer.positionCount = 0;
            }
        }
    }

    private void DebugChartPointPrefab()
    {
        if (chartPointPrefab == null)
        {
            Debug.LogError("InvestingUI DebugChartPointPrefab - Chart point prefab is null!");
            return;
        }

        Image[] images = chartPointPrefab.GetComponentsInChildren<Image>(true);
        Debug.Log($"InvestingUI DebugChartPointPrefab - Chart point prefab has {images.Length} Image components");

        foreach (Image image in images)
        {
            Debug.Log($"InvestingUI DebugChartPointPrefab - Image found on: {image.gameObject.name}, Color: {image.color}");
        }

        if (images.Length == 0)
        {
            Debug.LogError("InvestingUI DebugChartPointPrefab - No Image components found in chart point prefab.");
        }
    }
    #endregion

    #region Trading Logic
    private void HandleIncreaseTradeAmountButton()
    {
        SingleButtonClick();
        tradeShareQuantity++;
        UpdateUI();
    }

    private void HandleDecreaseTradeAmountButton()
    {
        if (tradeShareQuantity > 1)
        {
            SingleButtonClick();
            tradeShareQuantity--;
            UpdateUI();
        }
    }

    private void HandleBuyButton()
    {
        float currentPrice = GetCurrentSharePrice(companyIndex);
        float totalCost = currentPrice * tradeShareQuantity;

        if (DataPersister.Instance.CurrentGameData.totalMoney >= totalCost)
        {
            DataPersister.Instance.CurrentGameData.totalMoney -= totalCost;
            float currentShares = GetSharesOwned(companyIndex);
            SetSharesOwned(companyIndex, currentShares + tradeShareQuantity);

            DataPersister.Instance.SaveCurrentGame();
            GetTime();
            UpdateUI();
            PositiveButtonClick();
        }
        else
        {
            NegitiveButtonClick();
            Debug.Log("InvestingUI HandleBuyButton - Not enough money for this trade");
        }
    }

    private void HandleSellButton()
    {
        float currentPrice = GetCurrentSharePrice(companyIndex);
        float currentShares = GetSharesOwned(companyIndex);

        if (currentShares >= tradeShareQuantity)
        {
            float totalValue = currentPrice * tradeShareQuantity;
            DataPersister.Instance.CurrentGameData.totalMoney += totalValue;
            SetSharesOwned(companyIndex, currentShares - tradeShareQuantity);

            DataPersister.Instance.SaveCurrentGame();
            GetTime();
            UpdateUI();
            PositiveButtonClick();
        }
        else
        {
            NegitiveButtonClick();
            Debug.Log("InvestingUI HandleSellButton - Not enough shares to sell");
        }
    }

    private void HandleChangeCompanyTradeButton()
    {
        SingleButtonClick();
        companyIndex = (companyIndex + 1) % companies.Length;
        tradingCompanyNameText.text = companies[companyIndex].companyName;
        GetTime();
        UpdateUI();
    }

    private void HandleSelectCompanyTradeButton(int selectedCompanyIndex)
    {
        SingleButtonClick();
        companyIndex = selectedCompanyIndex;
        tradingCompanyNameText.text = companies[companyIndex].companyName;
        GetTime();
        UpdateUI();
    }

    private void HandleDividendButton()
    {
        if (dailyDividendCount > 0)
        {
            if (dailyDividendCount >= 3)
            {
                // Free dividend without ad
                DividendRewards();
            }
            else
            {
                // Show ad for dividend reward
                AdRequestDividendReward?.Invoke();
            }
        }
    }

    private void HandleAdRewardGranted()
    {
        if (dailyDividendCount >= 2)
        {
            DividendRewards();
        }
    }

    private void DividendRewards()
    {
        // Decrement daily dividend count
        dailyDividendCount--;

        float dividendMultiplier = UnityEngine.Random.Range(dividendPayoutPercentageMin, dividendPayoutPercentageMax);
        // go through all companies and get the share cost for each company and the shares owned for each company.

        calculatedDividend = 0f;

        for (int i = 0; i < companies.Length; i++)
        {
            // Get current share price for this company
            float currentSharePrice = GetCurrentSharePrice(i);

            // Get shares owned for this company
            float sharesOwned = GetSharesOwned(i);

            // Calculate dividend for this company and add to total
            calculatedDividend += currentSharePrice * sharesOwned * dividendMultiplier;
        }


        // Add dividend to player's money
        DataPersister.Instance.CurrentGameData.totalMoney += calculatedDividend;
        DataPersister.Instance.CurrentGameData.dailyDividendCount = dailyDividendCount;
        DataPersister.Instance.SaveCurrentGame();

        UpdateUI();

        Debug.Log($"Dividend paid: ${calculatedDividend: 0.00}, dividend count: {dailyDividendCount: 0}");

        StartCoroutine(AdRewardRecievedMessage()); 
    }

    IEnumerator AdRewardRecievedMessage()
    {
        adRewardRecievedHolder.SetActive(true);
        adRewardRecievedText.text = ($"Dividend paid: ${calculatedDividend: 0.00}");
        yield return new WaitForSeconds(3f);
        adRewardRecievedHolder.SetActive(false);
    }


    #endregion

    #region Day Management
    private void ShiftDaysOfTheWeek()
    {
        for (int i = 0; i < companies.Length; i++)
        {
            // Shift prices for the week
            for (int day = 0; day < 6; day++)
            {
                companies[i].sharePrices[day] = companies[i].sharePrices[day + 1];
            }

            // Generate new price for Saturday with variance, clamped to min/max
            float randomVariance = UnityEngine.Random.Range(-companies[i].variance, companies[i].variance);
            companies[i].sharePrices[6] = companies[i].sharePrices[5] * (1 + randomVariance);
            companies[i].sharePrices[6] = Mathf.Clamp(companies[i].sharePrices[6], minStockPrice, maxStockPrice);
        }

        SaveCompanyData();
        UpdateUI();
    }

    private void SaveCompanyData()
    {
        var gameData = DataPersister.Instance.CurrentGameData;

        for (int i = 0; i < companies.Length; i++)
        {
            string companyPrefix = $"company{i + 1}";
            SetCompanyPrice(gameData, companyPrefix, "Sunday", companies[i].sharePrices[0]);
            SetCompanyPrice(gameData, companyPrefix, "Monday", companies[i].sharePrices[1]);
            SetCompanyPrice(gameData, companyPrefix, "Tuesday", companies[i].sharePrices[2]);
            SetCompanyPrice(gameData, companyPrefix, "Wednesday", companies[i].sharePrices[3]);
            SetCompanyPrice(gameData, companyPrefix, "Thursday", companies[i].sharePrices[4]);
            SetCompanyPrice(gameData, companyPrefix, "Friday", companies[i].sharePrices[5]);
            SetCompanyPrice(gameData, companyPrefix, "Saturday", companies[i].sharePrices[6]);
        }

        DataPersister.Instance.SaveCurrentGame();
    }
    #endregion

    #region SFX
    //******************************* SFX *******************************

    private void PositiveButtonClick()
    {
        SFXManager.Instance.PlaySFX(buySuccessSFX);
    }

    private void NegitiveButtonClick()
    {
        SFXManager.Instance.PlaySFX(buyFailSFX);
    }

    private void SingleButtonClick()
    {
        if (singleButtonSFX.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, singleButtonSFX.Length);
            SFXManager.Instance.PlaySFX(singleButtonSFX[randomIndex]);
        }
    }

    private void DoubleButtonClick()
    {
        if (doubleButtonSFX.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, doubleButtonSFX.Length);
            SFXManager.Instance.PlaySFX(doubleButtonSFX[randomIndex]);
        }
    }
#endregion

    #region Greetings

    private void GreetingsWindow()
    {
        greetingHolder.SetActive(true);
        DataPersister.Instance.CurrentGameData.hasOpenedInvesting = true;
        DataPersister.Instance.SaveCurrentGame();
        GreetingsDialogue();
    }

    private void GreetingsDialogue()
    {
        SingleButtonClick();
        ClearExistingArrows();

        if (greetingTextIndex == 0)
        {
            greetingText.text = "Welcome investor! Are you ready to make PROFITS?";
            greetingTextIndex = 1;
            return;
        }
        if (greetingTextIndex == 1)
        {
            greetingText.text = "I'm the CEO of GoodCorp, the greatest company at everything.";
            greetingTextIndex = 2;
            return;
        }
        if (greetingTextIndex == 2)
        {
            if (chartUIImage != null && arrowPrefab != null)
            {
                GameObject arrow = Instantiate(arrowPrefab, chartUIImage.transform);
                RectTransform arrowRect = arrow.GetComponent<RectTransform>();
                if (arrowRect != null)
                {
                    // Position arrow at the chart
                    Vector3 location = arrowRect.anchoredPosition;
                    float xOffset = 0f;
                    float yOffset = 0f;
                    float zRotation = 225f;
                    arrowRect.anchoredPosition = new Vector3(location.x + xOffset, location.y + yOffset, location.z);
                    arrowRect.localRotation = Quaternion.Euler(0, 0, zRotation);
                    arrowRect.localScale = Vector3.one;

                    // Store reference to arrow for later removal
                    currentArrow = arrow;
                }
            }
            greetingText.text = "This chart shows 1 week of company share prices";
            greetingTextIndex = 3;
            return;
        }
        if (greetingTextIndex == 3)
        {
            greetingText.text = "Once a day the prices change.";
            greetingTextIndex = 4;
            return;
        }
        if (greetingTextIndex == 4)
        {
            if (selectCompany3TradeButton != null && arrowPrefab != null)
            {
                GameObject arrow = Instantiate(arrowPrefab, greetingHolder.transform);
                RectTransform arrowRect = arrow.GetComponent<RectTransform>();
                if (arrowRect != null)
                {
                    // Position arrow to the right of company 3 button
                    Vector3 buttonPos = selectCompany3TradeButton.transform.position;
                    float xOffset = 50f;
                    float yOffset = 0f;
                    float zRotation = 270f;
                    arrowRect.position = new Vector3(buttonPos.x + xOffset, buttonPos.y + yOffset, buttonPos.z);
                    arrowRect.localRotation = Quaternion.Euler(0, 0, zRotation);
                    arrowRect.localScale = Vector3.one;

                    // Store reference to arrow for later removal
                    currentArrow = arrow;
                }
            }
            greetingText.text = "Choose a company.";
            greetingTextIndex = 5;
            return;
        }
        if (greetingTextIndex == 5)
        {
            if (tradeShareQuantityText != null && arrowPrefab != null)
            {
                GameObject arrow = Instantiate(arrowPrefab, greetingHolder.transform);
                RectTransform arrowRect = arrow.GetComponent<RectTransform>();
                if (arrowRect != null)
                {
                    // Position arrow to the right of quantity text
                    Vector3 textPos = tradeShareQuantityText.transform.position;
                    float xOffset = 50f;
                    float yOffset = 0f;
                    float zRotation = 270f;
                    arrowRect.position = new Vector3(textPos.x + xOffset, textPos.y + yOffset, textPos.z);
                    arrowRect.localRotation = Quaternion.Euler(0, 0, zRotation);
                    arrowRect.localScale = Vector3.one;

                    // Store reference to arrow for later removal
                    currentArrow = arrow;
                }
            }
            greetingText.text = "Select an amount of shares.";
            greetingTextIndex = 6;
            return;
        }
        if (greetingTextIndex == 6)
        {
            if (buyButton != null && arrowPrefab != null)
            {
                GameObject arrow = Instantiate(arrowPrefab, greetingHolder.transform);
                RectTransform arrowRect = arrow.GetComponent<RectTransform>();
                if (arrowRect != null)
                {
                    // Position arrow to the right of buy button
                    Vector3 buttonPos = buyButton.transform.position;
                    float xOffset = 50f;
                    float yOffset = 0f;
                    float zRotation = 270f;
                    arrowRect.position = new Vector3(buttonPos.x + xOffset, buttonPos.y + yOffset, buttonPos.z);
                    arrowRect.localRotation = Quaternion.Euler(0, 0, zRotation);
                    arrowRect.localScale = Vector3.one;

                    // Store reference to arrow for later removal
                    currentArrow = arrow;
                }
            }
            greetingText.text = "Buy and sell.";
            greetingTextIndex = 7;
            return;
        }
        if (greetingTextIndex == 7)
        {
            greetingText.text = "Collect up to 3% dividend money daily for owning shares.";
            greetingTextIndex = 8;
            return;
        }

        if (greetingTextIndex >= 8)
        {
            greetingHolder.SetActive(false);
            greetingTextIndex = 0; // Reset for next time
        }

    }

    private void ClearExistingArrows()
    {
        if (currentArrow != null)
        {
            Destroy(currentArrow);
            currentArrow = null;
        }

        // Also clear any arrows that might have been created as children
        foreach (Transform child in greetingHolder.transform)
        {
            if (child.name.Contains(arrowPrefab.name) || child.name.Contains("Arrow"))
            {
                Destroy(child.gameObject);
            }
        }
    }
    #endregion

    #region Daily Company Loot Ship
    private void DailyCompanyLootShip()
    {
        SelectRandomLootShipLevel();
        SelectRandomLootShipCompany();
    }

    private void SelectRandomLootShipLevel()
    {
        int minLevel = 1;
        int maxLevelPlusOneForArray = 10;
        int randomLootShipLevel = UnityEngine.Random.Range(minLevel, maxLevelPlusOneForArray);
        DataPersister.Instance.CurrentGameData.randomLootShipLevel = randomLootShipLevel;
        DataPersister.Instance.SaveCurrentGame();
        Debug.Log($"InvestingUI SelectRandomLootShipLevel - Level: {randomLootShipLevel}");
    }

    private void SelectRandomLootShipCompany()
    {
        string randomLootShipCompanyName = "";
        Color randomLootShipCompanyColour = Color.white;
        int companyNumber = 0;
        int randomNumber = UnityEngine.Random.Range(0, companies.Length);
        if (randomNumber == 0)
        {
            randomLootShipCompanyName = companies[0].companyName;
            randomLootShipCompanyColour = companies[0].companyColor;
            companyNumber = 0;
        }
        else if (randomNumber == 1)
        {
            randomLootShipCompanyName = companies[1].companyName;
            randomLootShipCompanyColour = companies[1].companyColor;
            companyNumber = 1;
        }
        else if (randomNumber == 2)
        {
            randomLootShipCompanyName = companies[2].companyName;
            randomLootShipCompanyColour = companies[2].companyColor;
            companyNumber = 2;
        }
        else if (randomNumber == 3)
        {
            randomLootShipCompanyName = companies[3].companyName;
            randomLootShipCompanyColour = companies[3].companyColor;
            companyNumber = 3;
        }
        else if (randomNumber == 4)
        {
            randomLootShipCompanyName = companies[4].companyName;
            randomLootShipCompanyColour = companies[4].companyColor;
            companyNumber = 4;
        }

        DataPersister.Instance.CurrentGameData.randomLootShipCompanyName = randomLootShipCompanyName;
        DataPersister.Instance.CurrentGameData.randomLootShipCompanyColour = randomLootShipCompanyColour;
        DataPersister.Instance.CurrentGameData.investingCompanyNumber = companyNumber;
        DataPersister.Instance.SaveCurrentGame();
        Debug.Log($"InvestingUI SelectRandomCompany - Company: {randomLootShipCompanyName} and Colour: {randomLootShipCompanyColour}");

        StartCoroutine(DailyLootShipMessage());
    }

    IEnumerator DailyLootShipMessage()
    {
        adRewardRecievedHolder.SetActive(true);
        adRewardRecievedText.text = ($"Loot ship spotted on level {DataPersister.Instance.CurrentGameData.randomLootShipLevel}");
        yield return new WaitForSeconds(4f);
        adRewardRecievedHolder.SetActive(false);
    }
    #endregion
}