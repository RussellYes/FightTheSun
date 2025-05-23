using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This script controls the ship UI.

public class ShipUIManager : MonoBehaviour
{
    public static event Action FireMissilesEvent;

    [SerializeField] private GameObject shipUIBackground;

    [Header("Hull UI")]
    [SerializeField] private GameObject hullMeter;

    [Header("Thruster UI")]
    [SerializeField] private GameObject speedMeter;

    [Header("Progress UI")]
    [SerializeField] private GameObject checkpointUI;
    [SerializeField] private GameObject scoreMeter;
    [SerializeField] private GameObject pauseButton;

    [Header("Missile UI")]
    [SerializeField] private Button fireMissileButton;
    [SerializeField] private TextMeshProUGUI missileCountText;

    private void OnEnable()
    {
        fireMissileButton.onClick.AddListener(() => FireMissilesEvent?.Invoke());
        MiningMissileLauncher.LauncherActiveEvent += UpdateMissileButton;
    }
    private void OnDisable()
    {
        fireMissileButton.onClick.RemoveListener(() => FireMissilesEvent?.Invoke());
        MiningMissileLauncher.LauncherActiveEvent -= UpdateMissileButton;
    }
    public void TurnOnShipUI()
    {
        shipUIBackground.SetActive(true);

        hullMeter.SetActive(true);
        speedMeter.SetActive(true);
        checkpointUI.SetActive(true);
        scoreMeter.SetActive(true);
        pauseButton.SetActive(true);
    }
    public void TurnOffShipUI()
    {
        shipUIBackground.SetActive(false);
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
}
