using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class DialogueManager : MonoBehaviour
{
    public static event Action StartGameCountdownEvent;

    private GameManager gameManager;
    public static DialogueManager Instance;
    public static UnityEvent SpawnSingleSingleEvent = new UnityEvent();

    [SerializeField] private ShipUIManager shipUIManager;

    [SerializeField] private GameObject dialogueBoxUI;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float dialogueTimer = 4f;
    [SerializeField] private Button continueStartDialogueButton;
    private float dialogueCount = 0f;

    [Header("Mission Title / Countdown")]
    [SerializeField] private TextMeshProUGUI missionTitleText;
    [SerializeField] private TextMeshProUGUI countdownText;

    [Header("Planets")]
    [SerializeField] private GameObject planetSpawnPosition;
    [SerializeField] private GameObject planet1;

    [Header("Dashboard Controls")]
    [SerializeField] private GameObject highLightArrowPrefab;
    [SerializeField] private GameObject hullMeterObject;
    [SerializeField] private GameObject healthPrefab;

    private GameObject currentArrowInstance;

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
            case 6:
                Mission6();
                break;
            case 7:
                Mission7();
                break;
            case 8:
                Mission8();
                break;
            case 9:
                Mission9();
                break;
            case 10:
                Mission10();
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
        shipUIManager.TurnOnShipUI();
        shipUIManager.Mission1_1();

        if (dialogueCount == 0)
        {
            missionTitleText.text = "Mission 1: Learn to drive";

            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after delay
            StartCoroutine(StartGameCountdown()); 
            StartCoroutine(DelayedStartActions1()); 

            RectTransform hullMeterObjectRect = hullMeterObject.GetComponent<RectTransform>();

            // Apply offsets directly
            Vector3 xOffset = new Vector3(0, 0, 0); // Adjust these values as needed
            Vector3 yOffset = new Vector3(0, 100, 0); // Adjust these values as needed
            Vector3 arrowPosition = hullMeterObjectRect.position + xOffset + yOffset;

            // Instantiate the arrow prefab with the specified position and rotation, and set its parent
            currentArrowInstance = Instantiate(highLightArrowPrefab, arrowPosition, Quaternion.identity, hullMeterObjectRect.parent);

            // Ensure the arrow is rendered in front by setting its sibling index
            currentArrowInstance.transform.SetAsLastSibling();
            StartCoroutine(DestroyArrow(dialogueTimer));

            dialogueCount++;
            dialogueText.text = "Asteroid! Move with these buttons.";
        }
        else if (dialogueCount == 1)
        {
            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after

            RectTransform hullMeterObjectRect = hullMeterObject.GetComponent<RectTransform>();

            // Apply offsets directly
            Vector3 xOffset = new Vector3(0, 0, 0); // Adjust these values as needed
            Vector3 yOffset = new Vector3(0, 100, 0); // Adjust these values as needed
            Vector3 arrowPosition = hullMeterObjectRect.position + xOffset + yOffset;

            // Instantiate the arrow prefab with the specified position and rotation, and set its parent
            currentArrowInstance = Instantiate(highLightArrowPrefab, arrowPosition, Quaternion.identity, hullMeterObjectRect.parent);

            // Ensure the arrow is rendered in front by setting its sibling index
            currentArrowInstance.transform.SetAsLastSibling();
            StartCoroutine(DestroyArrow(dialogueTimer));

            dialogueCount++;
            dialogueText.text = "Protect the ship's hull, this <b><color=red>RED</color></b>  gauge.";
        }

        else if (dialogueCount == 2)
        {
            Debug.Log("DialogueManager - Mission 1 - Dialogue Count 2");
            string endText = "You've got skills. We arrived at the checkpoint safely.";
            StartCoroutine(EndDialogueScene(endText));

            dialogueCount = 0;
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
            RectTransform leftButtonRect = hullMeterObject.GetComponent<RectTransform>();

            // Apply offsets directly (no need for position conversion)
            Vector3 xOffset = new Vector3(0, 0, 0); // Adjust these values as needed
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
        shipUIManager.Mission5All();

        if (dialogueCount == 0)
        {
            dialogueCount++;

            StartCoroutine(FadeInDialogueBox());
            continueStartDialogueButton.interactable = true;

            dialogueText.text = "The outer asteroid belt is the most chaotic.";


            return;
        }
        if (dialogueCount == 1)
        {
            dialogueCount++;

            StartCoroutine(FadeInDialogueBox());
            continueStartDialogueButton.interactable = true;

            dialogueText.text = "Rocks are all that's left of a destroyed planet";


            return;
        }
        else if (dialogueCount == 2)
        {
            dialogueCount++;
            StartCoroutine(FadeOutDialogueBox(0f));
            GameManager.Instance.SetState(GameManager.GameState.Playing);
            return;
        }
        else if (dialogueCount == 3)
        {
            dialogueCount++;

            StartCoroutine(FadeInDialogueBox());
            dialogueText.text = "You've got this! We're almost at the Flagship.";

            StartCoroutine(FadeOutDialogueBox(4f));
        }
        else if (dialogueCount == 4)
        {
            StartCoroutine(FadeInDialogueBox());
            string endText = "We're alive! Thanks for not crashing this time.";
            StartCoroutine(EndDialogueScene(endText));

            dialogueCount++;

        }
        else if (dialogueCount == 5)
        {
            dialogueCount = 0;
            dialogueBoxUI.SetActive(false);
            //Trigger end
            gameManager.EndGame(true);
        }
    }

    private void Mission6()
    {

    }

    private void Mission7()
    {

    }

    private void Mission8()
    {

    }

    private void Mission9()
    {

    }

    private void Mission10()
    {

    }

    IEnumerator StartGameCountdown()
    {
        yield return new WaitForSeconds(0.1f); // Wait for one frame to ensure all scripts are enabled and subscribed
        StartGameCountdownEvent?.Invoke();
    }

    public void HideDialogue()
    {
        StartCoroutine(FadeOutDialogueBox(0f));
    }
    IEnumerator FadeInDialogueBox()
    {
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

            dialogueBoxUI.SetActive(false);
        }
    }

    IEnumerator DelayedStartActions1()
    {
        yield return new WaitForSeconds(3f);
        // Spawn 1 asteroid
        SpawnSingleSingleEvent?.Invoke();
    }
    IEnumerator DestroyArrow(float time)
    {
        Debug.Log("Destroying arrow wait");
        yield return new WaitForSeconds(time);
        Debug.Log("Destroying arrow");

        if (currentArrowInstance != null)
        {
            Destroy(currentArrowInstance);
        }
    }

    IEnumerator EndDialogueScene(string endText)
    {
        Debug.Log("DialogueManager - End Dialogue Scene");
        gameManager.SetState(GameManager.GameState.EndDialogue);

        Instantiate(planet1, planetSpawnPosition.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(3f);

        StartCoroutine(FadeInDialogueBox());
        dialogueText.text = endText;
        StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after delay

        yield return new WaitForSeconds(dialogueTimer + 1f);

        //Trigger end
        gameManager.EndGame(true);


    }



}