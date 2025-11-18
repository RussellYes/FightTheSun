using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementsUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject achievementHolder;
    [SerializeField] private TextMeshProUGUI achievementText;
    [SerializeField] private CanvasGroup achievementCanvasGroup;
    [SerializeField] private Image achievementBackingImage;

    [Header("Gradient Settings")]
    [SerializeField] private Color achievementColor1 = Color.red;
    [SerializeField] private Color achievementColor2 = Color.blue;
    [SerializeField] private float colorCycleSpeed = 4f;

    private Material gradientMaterial;
    private Coroutine currentFadeCoroutine;
    private bool isShowingAchievement;

    private void Awake()
    {
        // Load the shader
        Shader gradientShader = Shader.Find("Custom/RadialGradientWithMask");
        if (gradientShader == null)
        {
            Debug.LogError("Shader not found! Make sure it's named correctly.");
            return;
        }

        // Create material with the shader
        gradientMaterial = new Material(gradientShader)
        {
            mainTexture = achievementBackingImage.mainTexture
        };

        achievementBackingImage.material = gradientMaterial;
        achievementBackingImage.material.SetFloat("_RotationSpeed", colorCycleSpeed);
        UpdateGradientColors();
    }

    private void UpdateGradientColors()
    {
        if (gradientMaterial != null)
        {
            gradientMaterial.SetColor("_Color1", achievementColor1);
            gradientMaterial.SetColor("_Color2", achievementColor2);
        }
    }

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
        achievementCanvasGroup.alpha = 0f;
    }

    private void HandleMissionComplete(int missionNumber, string message)
    {
        var gameData = DataPersister.Instance.CurrentGameData;

        // Check if mission was already completed
        if (!gameData.GetMissionComplete(missionNumber))
        {
            Debug.Log("AchievementsUI HandleMissionComplete - showing achievement message.");
            ShowAchievementMessage(message);
        }

        // Unlock the next mission (if not the last one)
        if (missionNumber < 10)
        {
            gameData.SetMissionUnlocked(missionNumber + 1, true);
            Debug.Log($"Unlocked Level {missionNumber + 1}");
        }

        Debug.Log($"AchievementsUI HandleMissionComplete Mission {missionNumber} completed: {message}");
        gameData.SetMissionComplete(missionNumber, true);

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
        isShowingAchievement = true;

        // Fade in all elements together
        achievementHolder.SetActive(true);
        yield return StartCoroutine(Fade(0f, 1f, 0.2f));

        // Show for duration
        yield return new WaitForSeconds(5f);

        // Fade out all elements together
        yield return StartCoroutine(Fade(1f, 0f, 0.2f));

        // Only deactivate after everything has faded
        achievementHolder.SetActive(false);
        isShowingAchievement = false;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);

            // Fade canvas group
            achievementCanvasGroup.alpha = currentAlpha;

            // Fade gradient material
            if (gradientMaterial != null)
            {
                gradientMaterial.SetFloat("_FadeAmount", currentAlpha);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final values
        achievementCanvasGroup.alpha = endAlpha;
        if (gradientMaterial != null)
        {
            gradientMaterial.SetFloat("_FadeAmount", endAlpha);
        }
    }


}