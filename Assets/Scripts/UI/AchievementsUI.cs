using System.Collections;
using UnityEngine;
using TMPro;

public class AchievementsUI : MonoBehaviour
{
    [SerializeField] private GameObject achievementHolder;
    [SerializeField] private TextMeshProUGUI achievementText;

    private void OnEnable()
    {
        // Start coroutine to handle initialization with delay
        StartCoroutine(InitializeWithDelay());
    }
    private IEnumerator InitializeWithDelay()
    {
        // Wait for end of frame to ensure all objects are loaded
        yield return new WaitForEndOfFrame();

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("DialogueManager instance not found after delay!");
            yield break;
        }

        Debug.Log("Successfully connected to DialogueManager");
        DialogueManager.MissionCompleteEvent += HandleMissionComplete;
    }


    private void OnDisable()
    {
        DialogueManager.MissionCompleteEvent -= HandleMissionComplete;
    }
    private void Start()
    {
        achievementHolder.SetActive(false);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveAchievements();
        }
    }

    private void SaveAchievements()
    {
        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            DataPersister.Instance.SaveCurrentGame();
            Debug.Log("Achievements saved to game data");
        }
        else
        {
            Debug.LogWarning("DataPersister or GameData not available when saving achievements");
        }
    }

    private void HandleMissionComplete(int missionNumber, string message)
    {
        Debug.Log("AchievementUI HandleMissionComplete");
        
        var gameData = DataPersister.Instance.CurrentGameData;

        // Check if mission was already completed
        if (!gameData.GetMissionComplete(missionNumber))
        {
            Debug.Log($"Mission {missionNumber} completed: {message}");
            gameData.SetMissionComplete(missionNumber, true);
            ShowAchievementMessage(message);
        }

        // Unlock the next mission (if not the last one)
        if (missionNumber < 10)
        {
            gameData.SetMissionUnlocked(missionNumber + 1, true);
            Debug.Log($"Unlocked Level {missionNumber + 1}");
        }

        // Save progress
        DataPersister.Instance.SaveCurrentGame();
    }


    private void ShowAchievementMessage(string message)
    {
        achievementText.text = message;
        StartCoroutine(FadeInDialogueBox());
        StartCoroutine(FadeOutDialogueBox(5f));
    }

    IEnumerator FadeInDialogueBox()
    {
        achievementHolder.SetActive(true);

        CanvasGroup canvasGroup = achievementHolder.GetComponent<CanvasGroup>();
        float fadeDuration = 0.2f; // Duration of the fade in seconds
        float elapsedTime = 0f;

        if (canvasGroup != null)
        {
            // Fade out the dialogue box
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                yield return null;
            }

            // Ensure the alpha is set to 1 at the end
            canvasGroup.alpha = 1f;
        }
    }

    IEnumerator FadeOutDialogueBox(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        CanvasGroup canvasGroup = achievementHolder.GetComponent<CanvasGroup>();
        float fadeDuration = 0.2f; // Duration of the fade in seconds
        float elapsedTime = 0f;

        if (canvasGroup != null)
        {
            // Fade out the dialogue box
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                yield return null;
            }

            // Ensure the alpha is set to 0 at the end
            canvasGroup.alpha = 0f;

            achievementHolder.SetActive(false);
        }
    }


}
