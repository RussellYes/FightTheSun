using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManagerUI : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueManagerUIHolder;
    [SerializeField] private Image dialogueBoxPortraitImage;
    [SerializeField] private Sprite mavisPortraitImage;
    [SerializeField] private Sprite jermaPortraitImage;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float dialogueTimer = 4f;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Start()
    {
        dialogueManagerUIHolder.SetActive(false);
    }

    private void OnEnable()
    {
        DialogueManager.StartDialogueEvent += FadeInEventRecieved;
        DialogueManager.HideDialogueEvent += FadeOutEventRecieved;
        DialogueManager.FlipFlopPicEvent += FlipFlopPicEventRecieved;
    }
    private void OnDisable()
    {
        DialogueManager.StartDialogueEvent -= FadeInEventRecieved;
        DialogueManager.HideDialogueEvent -= FadeOutEventRecieved;
        DialogueManager.FlipFlopPicEvent -= FlipFlopPicEventRecieved;
    }

    private void FadeInEventRecieved(string name, string message, float dialogueTimerAmt)
    {
        StartCoroutine(FadeInDialogueBox(name, message, dialogueTimerAmt));
    }

    IEnumerator FadeInDialogueBox(string name, string message, float dialogueTimerAmt)
    {
        Debug.Log($"DialogueManagerUI FadeInDialogueBox - Name: {name}, Message: {message}, Timer: {dialogueTimerAmt}");

        dialogueManagerUIHolder.SetActive(true);

        // Set the person's picture
        if (name == "mavis")
        {
            dialogueBoxPortraitImage.sprite = mavisPortraitImage;
        }
        else if (name == "jerma")
        {
            dialogueBoxPortraitImage.sprite = jermaPortraitImage;
        }

        // Set the dialogue text
        dialogueText.text = message;

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

        // Fade out the dialogue box after a delay
        dialogueTimer = dialogueTimerAmt;
        StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after delay
    }

    private void FadeOutEventRecieved(float waitTime)
    {
        StartCoroutine(FadeOutDialogueBox(waitTime));
    }
    IEnumerator FadeOutDialogueBox(float waitTime)
    {
        dialogueManagerUIHolder.SetActive(true);
        Debug.Log($"DialogueManagerUI FadeOutDialogueBox - Wait Time: {waitTime}");

        yield return new WaitForSeconds(waitTime);

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

            dialogueManagerUIHolder.SetActive(false);
        }
    }

    private void FlipFlopPicEventRecieved(int flipCount, float flipWaitTime)
    {
        StartCoroutine(FlipFlopPic(flipCount, flipWaitTime));
    }
    IEnumerator FlipFlopPic(int flipCount, float flipWaitTime)
    {
        Transform portraitTransform = dialogueBoxPortraitImage.transform;

        for (int i = 0; i < flipCount; i++)
        {
            // Flip
            portraitTransform.localScale = new Vector3(-1, 1, 1);
            yield return new WaitForSeconds(flipWaitTime);

            // Unflip
            portraitTransform.localScale = Vector3.one; // Same as new Vector3(1, 1, 1)
            yield return new WaitForSeconds(flipWaitTime);
        }
    }



}
