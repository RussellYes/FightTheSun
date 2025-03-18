using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameCountdown : MonoBehaviour

{
    public static event Action countdownNoise;

    private SFXManager sFXManager;

    [SerializeField] private float countdownTime = 4f; // Serialized field for countdown time
    [SerializeField] private TextMeshProUGUI countdownText; // Reference to the UI Text
    [SerializeField] private float scaleDuration = 0.5f; // Duration of the scale animation

    [SerializeField] private AudioClip countdownSound;
    private bool isCountdownSoundPlayed;

    private float currentTime;
    private int lastDisplayedNumber;

    private void Start()
    {
        currentTime = 0;
        countdownText.enabled = false;
        sFXManager = FindAnyObjectByType<SFXManager>();
        isCountdownSoundPlayed = false;
        //BeginCountdown();
    }

    private void OnEnable()
    {
        Debug.Log("GameCountdown: OnEnable - Subscribing to StartGameCountdownEvent");
        // Subscribe to the event when this script is enabled
        DialogueManager.StartGameCountdownEvent += BeginCountdown;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event when this script is disabled
        DialogueManager.StartGameCountdownEvent -= BeginCountdown;
    }

    void Update()
    {
        Countdown();

    }

    private void BeginCountdown()
    {
        Debug.Log("GameCountdown: BeginCountdown");

        if (countdownText == null)
        {
            Debug.LogError("countdownText is null!");
            return;
        }

        Debug.Log("countdownText enabled state before: " + countdownText.enabled);
        countdownText.enabled = true;
        Debug.Log("countdownText enabled state after: " + countdownText.enabled);

        Debug.Log("currentTime before: " + currentTime);
        currentTime = countdownTime;
        Debug.Log("currentTime after: " + currentTime);
        UpdateCountdownText();
    }


    

    private void Countdown()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            if (isCountdownSoundPlayed == false)
            {
                sFXManager.PlaySFX(countdownSound);
                isCountdownSoundPlayed = true;
            }

            if (Mathf.CeilToInt(currentTime) != lastDisplayedNumber)
            {
                UpdateCountdownText();

                StartCoroutine(AnimateTextScale());
            }
            if (currentTime <= 0)
            {
                OnCountdownFinished();
            }
        }
    }

    

    private void UpdateCountdownText()
    {
        lastDisplayedNumber = Mathf.CeilToInt(currentTime);
        countdownText.text = lastDisplayedNumber.ToString();
    }

    private IEnumerator AnimateTextScale()
    {
        float elapsedTime = 0f;
        Vector3 originalScale = countdownText.transform.localScale;
        Vector3 targetScale = originalScale * 1.5f; // Scale up

        // Scale up
        while (elapsedTime < scaleDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            countdownText.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / (scaleDuration / 2));
            yield return null;
        }

        // Scale down
        elapsedTime = 0f;
        while (elapsedTime < scaleDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            countdownText.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / (scaleDuration / 2));
            yield return null;
        }

        countdownText.transform.localScale = originalScale; // Reset to original scale
    }

    private void OnCountdownFinished()
    {
        StartCoroutine(GoText());
        Debug.Log("Countdown finished!");
    }

    private IEnumerator GoText()
    {
        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);
        countdownText.enabled = false;
        isCountdownSoundPlayed = false;
    }



}