using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;

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

    [SerializeField] private GameObject healthPrefab;

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

        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        gameManager = GameManager.Instance;

        continueStartDialogueButton.onClick.AddListener(() =>
        {
            MissionDialogue();
        });
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset dialogue count when a new scene is loaded
        dialogueCount = 0;

        Debug.Log($"New scene loaded: {scene.name}. Dialogue count reset.");
    }

    private void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event when the object is destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
            case 4:
                Mission4();
                break;
            case 5:
                Mission5();
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
            dialogueText.text = "Welcome to captain training. I'm Emma, your trainer.";
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

            dialogueText.text = "Asteroid! Quick, let's dodge the asteroid with these controls.";

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
            dialogueText.text = "Time to prove yourself. More asteroids incoming.";
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
            dialogueText.text = "Let's stay in one piece. Protect the ship's hull";

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
            string endText = "You've got skills. We arrived at the checkpoint safely.";
            StartCoroutine(EndDialogueScene(endText));

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
        shipUIManager.Mission2All();

        if (dialogueCount == 0)
        {
            dialogueCount++;

            StartCoroutine(FadeInDialogueBox());
            continueStartDialogueButton.interactable = true;

            dialogueText.text = "Trainees who reach the flagship get to captain their own ship.";


            return;
        }
        else if (dialogueCount == 1)
        {
            dialogueCount++;
            StartCoroutine(FadeOutDialogueBox(0f));
            GameManager.Instance.SetState(GameManager.GameState.Playing);
            return;
        }
        else if (dialogueCount == 2)
        {
            dialogueCount++;

            StartCoroutine(FadeInDialogueBox());
            dialogueText.text = "Look! This meter shows our progress to the checkpoint.";

            RectTransform hullMeterObjectRect = hullMeterObject.GetComponent<RectTransform>();

            // Apply offsets directly
            Vector3 xOffset = new Vector3(100, 0, 0); // Adjust these values as needed
            Vector3 yOffset = new Vector3(0, 150, 0); // Adjust these values as needed
            Vector3 arrowPosition = hullMeterObjectRect.position + xOffset + yOffset;

            // Define the z axis rotation
            Quaternion rotation = Quaternion.Euler(0, 0, 90); // Rotate 90 degrees around the Z axis

            // Instantiate the arrow prefab with the specified position and rotation, and set its parent
            currentArrowInstance = Instantiate(highLightArrowPrefab, arrowPosition, rotation, hullMeterObjectRect.parent);

            StartCoroutine(FadeOutDialogueBox(4f));

            StartCoroutine(DestroyArrow(4f));
        }
        else if (dialogueCount == 3)
        {
            string endText = "The asteroids have claimed countless lives, but you survived.";
            StartCoroutine(EndDialogueScene(endText));

            dialogueCount++;

        }
        else if (dialogueCount == 4)
        {
            dialogueCount = 0;
            dialogueBoxUI.SetActive(false);
            //Trigger end
            gameManager.EndGame(true);
        }
    }
    
    private void Mission3()
    {
        shipUIManager.Mission2All();

        if (dialogueCount == 0)
        {
            dialogueCount++;

            StartCoroutine(FadeInDialogueBox());
            continueStartDialogueButton.interactable = true;

            dialogueText.text = "You get paid at the end of each mission.";


            return;
        }
        else if (dialogueCount == 1)
        {
            dialogueCount++;
            StartCoroutine(FadeOutDialogueBox(0f));
            GameManager.Instance.SetState(GameManager.GameState.Playing);
            return;
        }
        else if (dialogueCount == 2)
        {
            dialogueCount++;

            StartCoroutine(FadeInDialogueBox());
            continueStartDialogueButton.interactable = false;
            dialogueText.text = "Repair the hull with scrap metal from our company's wrecked ships.";
            Instantiate (healthPrefab, planetSpawnPosition.transform.position, Quaternion.identity);

            StartCoroutine(FadeOutDialogueBox(4f));
        }
        else if (dialogueCount == 3)
        {
            string endText = "Wreck your ship and the company keeps your earnings.";
            StartCoroutine(EndDialogueScene(endText));

            dialogueCount++;

        }
        else if (dialogueCount == 4)
        {
            dialogueCount = 0;
            dialogueBoxUI.SetActive(false);
            //Trigger end
            gameManager.EndGame(true);
        }
    }
    
    private void Mission4()
    {
        shipUIManager.Mission2All();

        if (dialogueCount == 0)
        {
            dialogueCount++;

            StartCoroutine(FadeInDialogueBox());
            continueStartDialogueButton.interactable = true;

            dialogueText.text = "Ready for more? The next lesson is speed control.";
            return;
        }
        else if (dialogueCount == 1)
        {
            dialogueCount++;
            StartCoroutine(FadeInDialogueBox());
            continueStartDialogueButton.interactable = false;
            dialogueText.text = "We can change speed with these controls";

            shipUIManager.Mission4_1();

            // Get the RectTransform of the leftButton
            RectTransform leftButtonRect = leftButton.GetComponent<RectTransform>();

            // Apply offsets directly (no need for position conversion)
            Vector3 xOffset = new Vector3(100, 0, 0); // Adjust these values as needed
            Vector3 yOffset = new Vector3(0, 80, 0); // Adjust these values as needed
            Vector3 arrowPosition = leftButtonRect.position + xOffset + yOffset;

            // Instantiate the arrow prefab and set its parent to the same canvas as the leftButton
            currentArrowInstance = Instantiate(highLightArrowPrefab, arrowPosition, Quaternion.identity, leftButtonRect.parent);

            StartCoroutine(FadeOutDialogueBox(4f));

            StartCoroutine(DestroyArrow(4f));
            return;
        }
        else if (dialogueCount == 2)
        {
            dialogueCount++;

            shipUIManager.Mission4_1();

            StartCoroutine(FadeInDialogueBox());
            continueStartDialogueButton.interactable = false;
            dialogueText.text = "Life in space moves fast. If you're slow, buy upgrades.";

            StartCoroutine(FadeOutDialogueBox(4f));
        }
        else if (dialogueCount == 3)
        {
            shipUIManager.Mission4_1();

            string endText = "Beautiful sights but we wont land. These first four planets have poison atmospheres.";
            StartCoroutine(EndDialogueScene(endText));

            dialogueCount++;

        }
        else if (dialogueCount == 4)
        {
            dialogueCount = 0;
            dialogueBoxUI.SetActive(false);
            //Trigger end
            gameManager.EndGame(true);
        }
    }
    
    private void Mission5()
    {
        shipUIManager.Mission2All();

        if (dialogueCount == 0)
        {
            dialogueCount++;

            StartCoroutine(FadeInDialogueBox());
            continueStartDialogueButton.interactable = true;

            dialogueText.text = "Life in space moves fast. If you're slow, buy upgrades.";


            return;
        }
        else if (dialogueCount == 1)
        {
            dialogueCount++;
            StartCoroutine(FadeOutDialogueBox(0f));
            GameManager.Instance.SetState(GameManager.GameState.Playing);
            return;
        }
        else if (dialogueCount == 2)
        {
            dialogueCount++;

            StartCoroutine(FadeInDialogueBox());
            dialogueText.text = "There's no problem we can't buy our way out of.";

            StartCoroutine(FadeOutDialogueBox(4f));
        }
        else if (dialogueCount == 3)
        {
            string endText = "Planet Charlie's volcanos make the air toxic. Only mutated plants survive.";
            StartCoroutine(EndDialogueScene(endText));

            dialogueCount++;

        }
        else if (dialogueCount == 4)
        {
            dialogueCount = 0;
            dialogueBoxUI.SetActive(false);
            //Trigger end
            gameManager.EndGame(true);
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

    public void HideDialogue()
    {
        StartCoroutine(FadeOutDialogueBox(0f));
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

    IEnumerator EndDialogueScene(string endText)
    {
        Instantiate(planet1, planetSpawnPosition.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(3f);

        StartCoroutine(FadeInDialogueBox());
        dialogueText.text = endText;
        continueStartDialogueButton.interactable = true; // Enable the button


    }



}