using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCountdown : MonoBehaviour

{
    [SerializeField] private float countdownTime = 4f; // Serialized field for countdown time
    [SerializeField] private TextMeshProUGUI countdownText; // Reference to the UI Text
    [SerializeField] private float scaleDuration = 0.5f; // Duration of the scale animation

    private float currentTime;
    private int lastDisplayedNumber;

    void Update()
    {
        Countdown();

    }

    //create event subscription to run this code
    private void BeginCountdown()
    {
        currentTime = countdownTime;
        countdownText.enabled = false;
        UpdateCountdownText();
    }

    private void Countdown()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            if (Mathf.CeilToInt(currentTime) != lastDisplayedNumber)
            {
                UpdateCountdownText();
                PlayCountdownSound();
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

    private void PlayCountdownSound()
    {
        //SendMessage event to SFXManager to play a sound
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
        countdownText.enabled = false;
        Debug.Log("Countdown finished!");
        // Add additional code to trigger when countdown reaches 0
    }

}