using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script controls the ship UI.

public class ShipUIManager : MonoBehaviour
{

    [SerializeField] private GameObject shipUIBackground;

    [Header("Hull UI")]
    [SerializeField] private GameObject hullMeter;

    [Header("Thruster UI")]
    [SerializeField] private GameObject speedMeter;

    [Header("Progress UI")]
    [SerializeField] private GameObject checkpointUI;
    [SerializeField] private GameObject scoreMeter;
    [SerializeField] private GameObject pauseButton;

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


}
