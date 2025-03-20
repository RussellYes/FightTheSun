using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipUIManager : MonoBehaviour
{

    [SerializeField] private GameObject shipUIBackground;

    [Header("Hull UI")]
    [SerializeField] private GameObject hullHolder;
    [SerializeField] private GameObject hullMeter;
    [SerializeField] private GameObject rightButtonObject;
    [SerializeField] private GameObject leftButtonObject;

    [Header("Thruster UI")]
    [SerializeField] private GameObject thrusterHolder;
    [SerializeField] private GameObject speedMeter;
    [SerializeField] private GameObject throttleUp;
    [SerializeField] private GameObject throttleDown;

    [Header("Progress UI")]
    [SerializeField] private GameObject checkpointUI;
    [SerializeField] private GameObject scoreMeter;
    [SerializeField] private GameObject pauseButton;

    public void TurnOnShipUI()
    {
        shipUIBackground.SetActive(true);
        hullHolder.SetActive(true);
        hullMeter.SetActive(true);
        rightButtonObject.SetActive(true);
        leftButtonObject.SetActive(true);
        thrusterHolder.SetActive(true);
        speedMeter.SetActive(true);
        throttleUp.SetActive(true);
        throttleDown.SetActive(true);
        checkpointUI.SetActive(true);
        scoreMeter.SetActive(true);
        pauseButton.SetActive(true);
    }
    public void TurnOffShipUI()
    {
        shipUIBackground.SetActive(false);
        hullHolder.SetActive(false);
        hullMeter.SetActive(false);
        rightButtonObject.SetActive(false);
        leftButtonObject.SetActive(false);
        thrusterHolder.SetActive(false);
        speedMeter.SetActive(false);
        throttleUp.SetActive(false);
        throttleDown.SetActive(false);
        checkpointUI.SetActive(false);
        scoreMeter.SetActive(false);
        pauseButton.SetActive(false);
    }

    public void Mission1_1()
    {
        //Show some ShipUI
        thrusterHolder.SetActive(false);
        speedMeter.SetActive(false);
        throttleUp.SetActive(false);
        throttleDown.SetActive(false);
    }

    public void Mission2All()
    {
        shipUIBackground.SetActive(true);
        rightButtonObject.SetActive(true);
        leftButtonObject.SetActive(true);
        speedMeter.SetActive(false);
        throttleUp.SetActive(false);
        throttleDown.SetActive(false);
        checkpointUI.SetActive(true);
        scoreMeter.SetActive(true);
        hullMeter.SetActive(true);
        pauseButton.SetActive(true);
    }

    public void Mission4_1()
    {
        throttleUp.SetActive(true);
        throttleDown.SetActive(true);
        speedMeter.SetActive(true);
    }

    public void Mission5All()
    {
        shipUIBackground.SetActive(true);
        rightButtonObject.SetActive(true);
        leftButtonObject.SetActive(true);
        speedMeter.SetActive(true);
        throttleUp.SetActive(true);
        throttleDown.SetActive(true);
        checkpointUI.SetActive(true);
        scoreMeter.SetActive(true);
        hullMeter.SetActive(true);
        pauseButton.SetActive(true);
    }



}
