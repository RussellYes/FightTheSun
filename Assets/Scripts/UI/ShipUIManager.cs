using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

// This script controls the ship UI.

public class ShipUIManager : MonoBehaviour
{
    public static event Action FireMissilesEvent;
    public static event Action PauseButtonEvent;

    SFXManager sFXManager;

    [SerializeField] private GameObject shipDashboardUIHolder;
    [SerializeField] private GameObject shipTotalTimeUIHolder;
    [SerializeField] private GameObject shipSunCountHolder;
    [SerializeField] private TextMeshProUGUI SunCountText;

    [Header("Hull UI")]
    [SerializeField] private GameObject hullMeter;

    [Header("Thruster UI")]
    [SerializeField] private GameObject speedMeter;

    [Header("Progress UI")]
    [SerializeField] private GameObject checkpointUI;
    [SerializeField] private GameObject scoreMeter;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private TextMeshProUGUI totalTimeText;
    private bool initialized = false;
    [SerializeField] private AudioClip clockTimeIsUpSFX;
    private Color defaultTimeTextColor;

    [Header("Missile UI")]
    [SerializeField] private Button fireMissileButton;
    [SerializeField] private TextMeshProUGUI missileCountText;

    private void Start()
    {
        sFXManager = FindFirstObjectByType<SFXManager>();
        defaultTimeTextColor = totalTimeText.color;
    }

    private void OnEnable()
    {
        fireMissileButton.onClick.AddListener(() => FireMissilesEvent?.Invoke());
        pauseButton.GetComponent<Button>().onClick.AddListener(() => PauseButtonEvent?.Invoke());
        MiningMissileLauncher.LauncherActiveEvent += UpdateMissileButton;
        DataPersister.InitializationComplete += OnInitializationComplete;
    }
    private void OnDisable()
    {
        fireMissileButton.onClick.RemoveListener(() => FireMissilesEvent?.Invoke());
        pauseButton.GetComponent<Button>().onClick.RemoveListener(() => PauseButtonEvent?.Invoke());
        MiningMissileLauncher.LauncherActiveEvent -= UpdateMissileButton;
        DataPersister.InitializationComplete -= OnInitializationComplete;
    }

    private void OnInitializationComplete()
    {
        initialized = true;
    }
    private void Update()
    {
        if (initialized)
        {
            KeepingTime();
            SunCount();
        }
    }
    public void TurnOnShipUI()
    {
        shipDashboardUIHolder.SetActive(true);
        shipTotalTimeUIHolder.SetActive(true);

        hullMeter.SetActive(true);
        speedMeter.SetActive(true);
        checkpointUI.SetActive(true);
        scoreMeter.SetActive(true);
        pauseButton.SetActive(true);
    }
    public void TurnOffShipUI()
    {
        shipDashboardUIHolder.SetActive(false);
        shipTotalTimeUIHolder.SetActive(false);

        hullMeter.SetActive(false);
        speedMeter.SetActive(false);
        checkpointUI.SetActive(false);
        scoreMeter.SetActive(false);
        pauseButton.SetActive(false);
    }

    private void UpdateMissileButton(int missileCount)
    {
        if (missileCount > 0)
        {
            fireMissileButton.interactable = true;
            missileCountText.text = missileCount.ToString();
        }
        else if (missileCount <= 0)
        {
            fireMissileButton.interactable = false;
            missileCountText.text = "0";
        }
    }

    private void KeepingTime()
    {
        if (totalTimeText != null)
        {
            float totalCountdownTime = GameManager.Instance.TotalCountdownTime;

            int minutes = Mathf.FloorToInt(totalCountdownTime / 60);
            int seconds = Mathf.FloorToInt(totalCountdownTime % 60);
            totalTimeText.text = $"{minutes:00}:{seconds:00}";

            if (Math.Floor(totalCountdownTime) == totalCountdownTime && totalCountdownTime >= 1 && totalCountdownTime <= 5)
            {
                totalTimeText.color = Color.red;
                sFXManager.PlaySFX(clockTimeIsUpSFX);
            }
            else
            {
                totalTimeText.color = defaultTimeTextColor;
            }
        }
        else
        {
            Debug.LogError("Total Time Text is not found by ShipUIManager");
        }
    }

    private void SunCount()
    {
        if (SunCountText != null)
        {
            SunCountText.text = DataPersister.Instance.CurrentGameData.sunCount.ToString();
        }
        else
        {
            Debug.LogError("Sun Count Text is not found by ShipUIManager");
        }
    }


}
