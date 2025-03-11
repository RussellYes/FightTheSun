using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    private GameManager gameManager;
    public static DialogueManager Instance;
    public static UnityEvent SpawnSingleSingleEvent = new UnityEvent();

    [SerializeField] private ShipUIManager shipUIManager;

    [SerializeField] private GameObject dialogueBoxUI;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button continueStartDialogueButton;
    private float dialogueCount = 0f;

    [Header("Planets")]
    [SerializeField] private GameObject planetSpawnPosition;
    [SerializeField] private GameObject planet1;

    [Header("Mission 1 - Dashboard Controls")]
    [SerializeField] private GameObject highLightArrowPrefab;

    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private GameObject hullMeterObject;

    


    private GameObject currentArrowInstance;
    private bool waitingForButtonPress = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


    }

    private void Start()
    {
        gameManager = GameManager.Instance;

        continueStartDialogueButton.onClick.AddListener(() =>
        {
            MissionDialogue();
        });
    }

    public void HideDialogue()
    {
        StartCoroutine(FadeOutDialogueBox(0f));
    }


    public void MissionDialogue()
    {
        Debug.Log("Dialogue Count = " + dialogueCount);

        // Debug log to verify the current mission
        Debug.Log("Current Mission = " + GameManager.Instance.CurrentMission);

        switch (GameManager.Instance.CurrentMission)
        {
            case 1:
                Mission1();
                break;
            case 2:
                Mission2();
                break;
            case 3:
                Mission3();
                break;
            default:
                HideDialogue();
                Debug.LogWarning("Current Mission is not set or invalid. Defaulting to Mission 1.");
                Mission1(); // Default to Mission 1 if CurrentMission is not set
                break;
        }
    }

    private void Mission1()
    {
        Debug.Log("Mission 1 Dialogue Count = " + dialogueCount);

        shipUIManager.Mission1All();


        if (dialogueCount == 0)
        {
            StartCoroutine(FadeInDialogueBox());
            continueStartDialogueButton.interactable = true;

            dialogueCount++;
            dialogueText.text = "Good morning pilot! Welcome to your new ship";
        }
        else if (dialogueCount == 1)
        {
            StartCoroutine(FadeInDialogueBox());
            continueStartDialogueButton.interactable = true;

            dialogueCount++;
            dialogueText.text = "Let's fly to the flagship. It's 5 checkpoints away.";
        }

        else if (dialogueCount == 2)
        {
            StartCoroutine(FadeInDialogueBox());

            continueStartDialogueButton.interactable = false; // Disable the button


            dialogueText.text = "Oh no! An asteroid. Let's dodge the asteroid with these controls.";

            shipUIManager.Mission1_2();

            // Add listeners to the buttons
            leftButton.onClick.AddListener(LeftRightButtonPushed);
            rightButton.onClick.AddListener(LeftRightButtonPushed);

            // Get the RectTransform of the leftButton
            RectTransform leftButtonRect = leftButton.GetComponent<RectTransform>();

            // Apply offsets directly (no need for position conversion)
            Vector3 xOffset = new Vector3(20, 0, 0); // Adjust these values as needed
            Vector3 yOffset = new Vector3(0, 80, 0); // Adjust these values as needed
            Vector3 arrowPosition = leftButtonRect.position + xOffset + yOffset;

            // Instantiate the arrow prefab and set its parent to the same canvas as the leftButton
            currentArrowInstance = Instantiate(highLightArrowPrefab, arrowPosition, Quaternion.identity, leftButtonRect.parent);

            SpawnSingleSingleEvent?.Invoke();

            // Ensure the arrow is rendered in front by setting its sibling index
            currentArrowInstance.transform.SetAsLastSibling();

            waitingForButtonPress = true;
        }
        else if (dialogueCount == 3)
        {
            shipUIManager.Mission1_3();

            StartCoroutine(FadeInDialogueBox());

            continueStartDialogueButton.interactable = true; // Enable the button

            dialogueCount++;
            dialogueText.text = "Look out!. More asteroids incoming.";
        }
        else if (dialogueCount == 4)
        {
            dialogueCount++;

            StartCoroutine(FadeOutDialogueBox(0f));

            shipUIManager.Mission1_4();

            GameManager.Instance.SetState(GameManager.GameState.Playing);
        }
        else if (dialogueCount == 5)
        {
            dialogueCount++;

            StartCoroutine(FadeInDialogueBox());
            dialogueText.text = "Let's stay in one piece. Don't damage the ship's hull";

            shipUIManager.Mission1_5();

            // Get the RectTransform of the leftButton
            RectTransform hullMeterObjectRect = hullMeterObject.GetComponent<RectTransform>();

            // Apply offsets directly (no need for position conversion)
            Vector3 xOffset = new Vector3(0, 0, 0); // Adjust these values as needed
            Vector3 yOffset = new Vector3(0, 80, 0); // Adjust these values as needed
            Vector3 arrowPosition = hullMeterObjectRect.position + xOffset + yOffset;

            // Instantiate the arrow prefab and set its parent to the same canvas as the leftButton
            currentArrowInstance = Instantiate(highLightArrowPrefab, arrowPosition, Quaternion.identity, hullMeterObjectRect.parent);

            StartCoroutine(FadeOutDialogueBox(4f));

            StartCoroutine(DestroyArrow(4f));
        }
        else if (dialogueCount == 6)
        {
            StartCoroutine(EndDialogueScene());

            dialogueCount++;

        }
        else if (dialogueCount == 7)
        {
            dialogueCount = 0;
            dialogueBoxUI.SetActive(false);
            //Trigger end
            gameManager.EndGame(true);
        }
    }

    private void Mission2()
    {

        if (dialogueCount == 0)
        {
            dialogueCount++;
            dialogueText.text = "Mission 2 dialogue here";
        }
        if (dialogueCount == 1)
        {
            dialogueCount = 0;
            dialogueText.text = "Mission 2 dialogue here";
        }
    }

    private void Mission3()
    {

        if (dialogueCount == 0)
        {
            dialogueCount++;
            dialogueText.text = "Mission 3 dialogue here";
        }
        if (dialogueCount == 1)
        {
            dialogueCount = 0;
            dialogueText.text = "Mission 3 dialogue here";
        }
    }

    private void LeftRightButtonPushed()
    {
        Debug.Log("Left or Right Button Pushed");

        if (waitingForButtonPress)
        {
            if (currentArrowInstance != null)
            {
                Destroy(currentArrowInstance);
            }
            waitingForButtonPress = false;
            dialogueCount++;
            MissionDialogue();
        }
    }

    IEnumerator FadeInDialogueBox()
    {
        continueStartDialogueButton.interactable = false;
        dialogueBoxUI.SetActive(true);

        CanvasGroup canvasGroup = dialogueBoxUI.GetComponent<CanvasGroup>();
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
        continueStartDialogueButton.interactable = false;

        yield return new WaitForSeconds(waitTime);             

        CanvasGroup canvasGroup = dialogueBoxUI.GetComponent<CanvasGroup>();
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

            continueStartDialogueButton.interactable = true;
            dialogueBoxUI.SetActive(false);
            GameManager.Instance.SetState(GameManager.GameState.Playing);
        }
    }

    IEnumerator DestroyArrow(float time)
    {
        yield return new WaitForSeconds(time);

        if (currentArrowInstance != null)
        {
            Destroy(currentArrowInstance);
        }
    }

    IEnumerator EndDialogueScene()
    {
        Instantiate(planet1, planetSpawnPosition.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(3f);

        StartCoroutine(FadeInDialogueBox());
        dialogueText.text = "You've got skills. We arrived at the checkpoint safely.";
        continueStartDialogueButton.interactable = true; // Enable the button


    }



}