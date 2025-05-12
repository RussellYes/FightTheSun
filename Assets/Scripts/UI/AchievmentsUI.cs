using System.Collections;
using UnityEngine;
using TMPro;

public class AchievmentsUI : MonoBehaviour
{
    [SerializeField] private GameObject achievementHolder;
    [SerializeField] private TextMeshProUGUI achievementText;
    public bool isMission1Complete;
    public bool isMission2Complete;
    public bool isMission3Complete;
    public bool isMission4Complete;
    public bool isMission5Complete;
    public bool isMission6Complete;
    public bool isMission7Complete;
    public bool isMission8Complete;
    public bool isMission9Complete;
    public bool isMission10Complete;


    private void OnEnable()
    {
        DialogueManager.Mission1CompleteEvent += Mission1Achievement;
    }
    private void OnDisable()
    {
        DialogueManager.Mission1CompleteEvent -= Mission1Achievement;
    }
    private void Start()
    {
        achievementHolder.SetActive(false);
    }
    private void Mission1Achievement(string message)
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
