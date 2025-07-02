using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class AchievementsUI : MonoBehaviour
{
    [SerializeField] private GameObject achievementHolder;
    [SerializeField] private TextMeshProUGUI achievementText;
    [SerializeField] private CanvasGroup achievementCanvasGroup;

    [Header("Achievement Backing")]
    [SerializeField] private Image achievementBackingImage;
    [SerializeField] private Color achievementColor1;
    [SerializeField] private Color achievementColor2;
    private float dialogueTime = 5f;
    private Coroutine currentFadeCoroutine;
    private Coroutine gradientCoroutine;
    private bool isShowingAchievement = false;

    private void OnEnable()
    {
        DialogueManager.MissionCompleteEvent += HandleMissionComplete;
    }

    private void OnDisable()
    {
        DialogueManager.MissionCompleteEvent -= HandleMissionComplete;
    }

    private void Start()
    {
        achievementHolder.SetActive(false);
        if (achievementCanvasGroup != null) achievementCanvasGroup.alpha = 0f;
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

        Debug.Log($"Mission {missionNumber} completed: {message}");
        gameData.SetMissionComplete(missionNumber, true);

        // Check if mission was already completed
        if (!gameData.GetMissionComplete(missionNumber))
        {
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
        if (isShowingAchievement) return;
        isShowingAchievement = true;
        achievementText.text = message;

        // Stop any ongoing fade coroutines
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        // Start gradient effect
        if (gradientCoroutine != null)
        {
            StopCoroutine(gradientCoroutine);
        }
        gradientCoroutine = StartCoroutine(SpinningGradientBacking());

        // Fade in
        achievementHolder.SetActive(true);
        achievementCanvasGroup.alpha = 0f; // Reset alpha

        float elapsedTime = 0f;
        float fadeDuration = 0.2f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            achievementCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }
        achievementCanvasGroup.alpha = 1f;

        // Wait before fading out
        yield return new WaitForSeconds(dialogueTime);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            achievementCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }
        achievementCanvasGroup.alpha = 0f;

        // Clean up
        achievementHolder.SetActive(false);
        if (gradientCoroutine != null)
        {
            StopCoroutine(gradientCoroutine);
        }
        isShowingAchievement = false;
    }

    IEnumerator FadeInDialogueBox()
    {
        achievementHolder.SetActive(true);
        StartCoroutine(SpinningGradientBacking());

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

    IEnumerator SpinningGradientBacking()
    {
        // Repeatly rotate the gradient backing image
        while (true)
        {
            if (achievementBackingImage != null)
            {
                achievementBackingImage.color = Color.Lerp(
                    achievementColor1,
                    achievementColor2,
                    Mathf.PingPong(Time.time * 0.5f, 1)
                );
            }
            yield return null;
        }
    }

    

}
