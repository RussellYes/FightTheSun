using System.Collections;
using UnityEngine;
using TMPro;

public class AchievmentsUI : MonoBehaviour
{
    [SerializeField] private GameObject achievementHolder;
    [SerializeField] private TextMeshProUGUI achievementText;
    private bool isMission1Complete;
    private bool isMission2Complete;
    private bool isMission3Complete;
    private bool isMission4Complete;
    private bool isMission5Complete;
    private bool isMission6Complete;
    private bool isMission7Complete;
    private bool isMission8Complete;
    private bool isMission9Complete;
    private bool isMission10Complete;

    private void Awake()
    {
        LoadAchievements();
    }
    private void OnEnable()
    {
        DialogueManager.Mission1CompleteEvent += Mission1Achievement;
        DialogueManager.Mission2CompleteEvent += Mission2Achievement;
        DialogueManager.Mission3CompleteEvent += Mission3Achievement;
        DialogueManager.Mission4CompleteEvent += Mission4Achievement;
        DialogueManager.Mission5CompleteEvent += Mission5Achievement;
        DialogueManager.Mission6CompleteEvent += Mission6Achievement;
        DialogueManager.Mission7CompleteEvent += Mission7Achievement;
        DialogueManager.Mission8CompleteEvent += Mission8Achievement;
        DialogueManager.Mission9CompleteEvent += Mission9Achievement;
        DialogueManager.Mission10CompleteEvent += Mission10Achievement;
    }

    private void OnDisable()
    {
        DialogueManager.Mission1CompleteEvent -= Mission1Achievement;
        DialogueManager.Mission2CompleteEvent -= Mission2Achievement;
        DialogueManager.Mission3CompleteEvent -= Mission3Achievement;
        DialogueManager.Mission4CompleteEvent -= Mission4Achievement;
        DialogueManager.Mission5CompleteEvent -= Mission5Achievement;
        DialogueManager.Mission6CompleteEvent -= Mission6Achievement;
        DialogueManager.Mission7CompleteEvent -= Mission7Achievement;
        DialogueManager.Mission8CompleteEvent -= Mission8Achievement;
        DialogueManager.Mission9CompleteEvent -= Mission9Achievement;
        DialogueManager.Mission10CompleteEvent -= Mission10Achievement;
    }
    private void Start()
    {
        achievementHolder.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        SaveAchievements();
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
            GameData gameData = DataPersister.Instance.CurrentGameData;

            gameData.isMission1Complete = isMission1Complete;
            gameData.isMission2Complete = isMission2Complete;
            gameData.isMission3Complete = isMission3Complete;
            gameData.isMission4Complete = isMission4Complete;
            gameData.isMission5Complete = isMission5Complete;
            gameData.isMission6Complete = isMission6Complete;
            gameData.isMission7Complete = isMission7Complete;
            gameData.isMission8Complete = isMission8Complete;
            gameData.isMission9Complete = isMission9Complete;
            gameData.isMission10Complete = isMission10Complete;

            DataPersister.Instance.SaveCurrentGame();
            Debug.Log("Achievements saved to game data");
        }
        else
        {
            Debug.LogWarning("DataPersister or GameData not available when saving achievements");
        }
    }

    private void LoadAchievements()
    {
        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            GameData gameData = DataPersister.Instance.CurrentGameData;

            isMission1Complete = gameData.isMission1Complete;
            isMission2Complete = gameData.isMission2Complete;
            isMission3Complete = gameData.isMission3Complete;
            isMission4Complete = gameData.isMission4Complete;
            isMission5Complete = gameData.isMission5Complete;
            isMission6Complete = gameData.isMission6Complete;
            isMission7Complete = gameData.isMission7Complete;
            isMission8Complete = gameData.isMission8Complete;
            isMission9Complete = gameData.isMission9Complete;
            isMission10Complete = gameData.isMission10Complete;

            Debug.Log("Achievements loaded from saved data");
        }
        else
        {
            Debug.LogWarning("DataPersister or GameData not available when loading achievements");
        }
    }

    private void Mission1Achievement(string message)
    {
        isMission1Complete = true;
        ShowAchievementMessage(message);
        SaveAchievements();
    }

    private void Mission2Achievement(string message)
    {
        isMission2Complete = true;
        ShowAchievementMessage(message);
        SaveAchievements();
    }

    private void Mission3Achievement(string message)
    {
        isMission3Complete = true;
        ShowAchievementMessage(message);
        SaveAchievements();
    }

    private void Mission4Achievement(string message)
    {
        isMission4Complete = true;
        ShowAchievementMessage(message);
        SaveAchievements();
    }

    private void Mission5Achievement(string message)
    {
        isMission5Complete = true;
        ShowAchievementMessage(message);
        SaveAchievements();
    }

    private void Mission6Achievement(string message)
    {
        isMission6Complete = true;
        ShowAchievementMessage(message);
        SaveAchievements();
    }

    private void Mission7Achievement(string message)
    {
        isMission7Complete = true;
        ShowAchievementMessage(message);
        SaveAchievements();
    }

    private void Mission8Achievement(string message)
    {
        isMission8Complete = true;
        ShowAchievementMessage(message);
        SaveAchievements();
    }

    private void Mission9Achievement(string message)
    {
        isMission9Complete = true;
        ShowAchievementMessage(message);
        SaveAchievements();
    }

    private void Mission10Achievement(string message)
    {
        isMission10Complete = true;
        ShowAchievementMessage(message);
        SaveAchievements();
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
