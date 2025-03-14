using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipUIManager : MonoBehaviour
{

    [SerializeField] private GameObject shipUIBackground;
    [SerializeField] private GameObject hullMeter;
    [SerializeField] private GameObject rightButtonObject;
    [SerializeField] private GameObject leftButtonObject;

    [SerializeField] private GameObject speedMeter;
    [SerializeField] private GameObject throttleUp;
    [SerializeField] private GameObject throttleDown;
    [SerializeField] private GameObject checkpointUI;
    [SerializeField] private GameObject scoreMeter;
    [SerializeField] private GameObject pauseButton;

    public void TurnOnShipUI()
    {
        shipUIBackground.SetActive(true);
        hullMeter.SetActive(true);
        rightButtonObject.SetActive(true);
        leftButtonObject.SetActive(true);
        speedMeter.SetActive(true);
        throttleUp.SetActive(true);
        throttleDown.SetActive(true);
        checkpointUI.SetActive(true);
        scoreMeter.SetActive(true);
        pauseButton.SetActive(true);
    }
    public void Mission1All()
    {
        shipUIBackground.SetActive(false);
        hullMeter.SetActive(false);
        rightButtonObject.SetActive(false);
        leftButtonObject.SetActive(false);
        speedMeter.SetActive(false);
        throttleUp.SetActive(false);
        throttleDown.SetActive(false);
        checkpointUI.SetActive(false);
        scoreMeter.SetActive(false);
        pauseButton.SetActive(false);
    }

    public void Mission1_2()
    {
        //Show some ShipUI
        shipUIBackground.SetActive(true);
        leftButtonObject.SetActive(true);
        rightButtonObject.SetActive(true);
        pauseButton.SetActive(true);
    }

    public void Mission1_3()
    {
        //Show some ShipUI
        shipUIBackground.SetActive(true);
        leftButtonObject.SetActive(true);
        rightButtonObject.SetActive(true);
        pauseButton.SetActive(true);
    }

    public void Mission1_4()
    {
        shipUIBackground.SetActive(true);
        rightButtonObject.SetActive(true);
        leftButtonObject.SetActive(true);
        checkpointUI.SetActive(true);
        scoreMeter.SetActive(true);
        pauseButton.SetActive(true);
    }
    public void Mission1_5()
    {
        shipUIBackground.SetActive(true);
        rightButtonObject.SetActive(true);
        leftButtonObject.SetActive(true);
        checkpointUI.SetActive(true);
        scoreMeter.SetActive(true);
        hullMeter.SetActive(true);
        pauseButton.SetActive(true);
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
