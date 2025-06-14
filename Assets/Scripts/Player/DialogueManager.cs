using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using static GameManager;

// This script controls the game story and dialogue, including tutorial style instructions.

public class DialogueManager : MonoBehaviour
{
    public static event Action StartGameCountdownEvent;
    public static event Action <int, string> MissionCompleteEvent;
    public static event Action <GameObject> ShipGraveyardEvent;

    private GameManager gameManager;
    public static DialogueManager Instance;
    public static UnityEvent SpawnSpecialEvent = new UnityEvent();

    [SerializeField] private ShipUIManager shipUIManager;

    [Header("Dialogue UI")]
    private Image dialogueBoxPortraitImage;
    [SerializeField] private Sprite mavisPortraitImage;
    [SerializeField] private Sprite jermaPortraitImage;
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

    [Header("Boss")]

    [SerializeField] private GameObject boss1;
    [SerializeField] private GameObject boss2;
    [SerializeField] private GameObject boss3;

    [Header("Dashboard Controls")]
    [SerializeField] private GameObject highLightArrowPrefab;
    [SerializeField] private GameObject healthPrefab;
    private Transform hullBarObjectRect;
    private Transform thrustBarObjectRect;
    private Transform CheckpointMeterObjectRect;

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

        hullBarObjectRect = GameObject.Find("HullBar").GetComponent<RectTransform>();
        if (hullBarObjectRect == null)
        {
            Debug.LogError("DialogueManager Start - HullBar object not found in the scene.");
        }
        thrustBarObjectRect = GameObject.Find("ThrustBar").GetComponent<RectTransform>();
        if (thrustBarObjectRect == null)
        {
            Debug.LogError("DialogueManager Start - ThrustBar object not found in the scene.");
        }
        CheckpointMeterObjectRect = GameObject.Find("CheckpointMeter").GetComponent<RectTransform>();
        if (CheckpointMeterObjectRect == null)
        {
            Debug.LogError("DialogueManager Start - CheckpointMeter object not found in the scene.");
        }

        // Find GameObject with tag "SpeakerPortraitUI"
        GameObject portraitObject = GameObject.FindWithTag("SpeakerPortraitUI");

        if (portraitObject != null)
        {
            // Get the Image component
            dialogueBoxPortraitImage = portraitObject.GetComponent<Image>();
        }
        else
        {
            Debug.LogError("No GameObject with tag 'SpeakerPortraitUI' found");
        }

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

        if (dialogueCount == 0)
        {
            missionTitleText.text = "Mission 1: Learn To Drive";

            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after delay
            StartCoroutine(StartGameCountdown()); 
            StartCoroutine(DelayedSpawnActions1());

            if (dialogueBoxPortraitImage != null)
            {
                dialogueBoxPortraitImage.sprite = mavisPortraitImage;
            }
            dialogueCount++;
            dialogueText.text = "Asteroid! Swipe left and right to move.";

            StartCoroutine(MissionDialogueDelay(5f));
        }
        else if (dialogueCount == 1)
        {
            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after delay

            // Apply offsets directly
            Vector3 xOffset = new Vector3(0, 0, 0); // Adjust these values as needed
            Vector3 yOffset = new Vector3(0, 150, 0); // Adjust these values as needed
            Vector3 arrowPosition = hullBarObjectRect.position + xOffset + yOffset;

            // Instantiate the arrow prefab with the specified position and rotation, and set its parent
            currentArrowInstance = Instantiate(highLightArrowPrefab, arrowPosition, Quaternion.identity, hullBarObjectRect);

            // Ensure the arrow is rendered in front by setting its sibling index
            currentArrowInstance.transform.SetAsLastSibling();
            StartCoroutine(DestroyArrow(dialogueTimer));

            dialogueCount++;
            dialogueText.text = "Protect the ship's hull, this <b><color=red>RED</color></b>  gauge.";
        }
        else if (dialogueCount == 2)
        {
            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after delay

            dialogueCount++;
            dialogueText.text = "Swipe up and down to change speed.";

            StartCoroutine(MissionDialogueDelay(5f));
        }
        else if (dialogueCount == 3)
        {
            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after delay

            // Apply offsets directly
            Vector3 xOffset = new Vector3(0, 0, 0); // Adjust these values as needed
            Vector3 yOffset = new Vector3(0, 150, 0); // Adjust these values as needed
            Vector3 arrowPosition = thrustBarObjectRect.position + xOffset + yOffset;
            // Instantiate the arrow prefab with the specified position and rotation, and set its parent
            currentArrowInstance = Instantiate(highLightArrowPrefab, arrowPosition, Quaternion.identity, hullBarObjectRect);
            // Ensure the arrow is rendered in front by setting its sibling index
            currentArrowInstance.transform.SetAsLastSibling();
            StartCoroutine(DestroyArrow(dialogueTimer));
            dialogueCount++;
            dialogueText.text = "This <b><color=blue>BLUE</color></b> gauge shows your speed.";
        }
        else if (dialogueCount == 4)
        {
            Debug.Log("DialogueManager - Mission 1 - Dialogue Count 4");
            string endText = "Great flying. You're at the first planet.";

            dialogueCount = 0;
            MissionCompleteEvent?.Invoke(GameManager.Instance.CurrentMission, "Mission 1 Complete. Story unlocked.");
            StartCoroutine(EndDialogueScene(endText));
        }

    }
    private void Mission2()
    {
        Debug.Log("Mission 2 Dialogue Count = " + dialogueCount);
        shipUIManager.TurnOnShipUI();

        if (dialogueCount == 0)
        {
            missionTitleText.text = "Mission 2: Speed Control";

            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after delay
            StartCoroutine(StartGameCountdown());

            if (dialogueBoxPortraitImage != null)
            {
                dialogueBoxPortraitImage.sprite = mavisPortraitImage;
            }
            dialogueCount++;
            dialogueText.text = "Race to the sun or slow down for loot?";
        }
        else if (dialogueCount == 1)
        {
            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after

            // Apply offsets directly
            Vector3 xOffset = new Vector3(-50, 0, 0); // Adjust these values as needed
            Vector3 yOffset = new Vector3(0, 100, 0); // Adjust these values as needed
            Vector3 arrowPosition = CheckpointMeterObjectRect.position + xOffset + yOffset;

            // Define the z axis rotation
            Quaternion rotation = Quaternion.Euler(0, 0, 90); // Rotate 90 degrees around the Z axis

            // Instantiate the arrow prefab with the specified position and rotation, and set its parent
            currentArrowInstance = Instantiate(highLightArrowPrefab, arrowPosition, rotation, CheckpointMeterObjectRect.parent);

            // Ensure the arrow is rendered in front by setting its sibling index
            currentArrowInstance.transform.SetAsLastSibling();
            StartCoroutine(DestroyArrow(dialogueTimer));

            if (dialogueBoxPortraitImage != null)
            {
                dialogueBoxPortraitImage.sprite = jermaPortraitImage;
            }
            dialogueCount++;
            dialogueText.text = "Look! This meter shows the progress to our checkpoint.";
        }

        else if (dialogueCount == 2)
        {
            if (dialogueBoxPortraitImage != null)
            {
                dialogueBoxPortraitImage.sprite = mavisPortraitImage;
            }
            Debug.Log("DialogueManager - Mission 2 - Dialogue Count 2");
            string endText = "You've got skills. We arrived at the space station safely.";
            StartCoroutine(EndDialogueScene(endText));

            dialogueCount = 0;
            MissionCompleteEvent?.Invoke(GameManager.Instance.CurrentMission, "Mission 2 Complete. Story unlocked.");
        }
    }
    
    private void Mission3()
    {
        Debug.Log("Mission 3 Dialogue Count = " + dialogueCount);
        shipUIManager.TurnOnShipUI();

        if (dialogueCount == 0)
        {
            missionTitleText.text = "Mission 3: Fixing For Trouble";

            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after delay
            StartCoroutine(StartGameCountdown());
            StartCoroutine(DelayedSpawnActions1());

            if (dialogueBoxPortraitImage != null)
            {
                dialogueBoxPortraitImage.sprite = mavisPortraitImage;
            }
            dialogueCount++;
            dialogueText.text = "Collect Shipwrecks to repair your ship.";
        }
        else if (dialogueCount == 1)
        {
            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after

            dialogueCount++;
            dialogueText.text = "Shipwreck scrap is worth money too!";
        }

        else if (dialogueCount == 2)
        {
            Debug.Log("DialogueManager - Mission 3 - Dialogue Count 2");
            string endText = "Life moves fast. If you're slow, buy upgrades.";
            StartCoroutine(EndDialogueScene(endText));

            dialogueCount = 0;
            MissionCompleteEvent?.Invoke(GameManager.Instance.CurrentMission, "Mission 3 Complete. Story unlocked.");
        }
    }
    
    private void Mission4()
    {
        Debug.Log("Mission 4 Dialogue Count = " + dialogueCount);
        shipUIManager.TurnOnShipUI();

        if (dialogueCount == 0)
        {
            missionTitleText.text = "Mission 4: Trouble Finds You";

            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after delay
            StartCoroutine(StartGameCountdown());
            StartCoroutine(DelayedSpawnActions1());

            if (dialogueBoxPortraitImage != null)
            {
                dialogueBoxPortraitImage.sprite = mavisPortraitImage;
            }
            dialogueCount++;
            dialogueText.text = "Not everyone is friendly in space.";
        }
        else if (dialogueCount == 1)
        {
            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after

            if (dialogueBoxPortraitImage != null)
            {
                dialogueBoxPortraitImage.sprite = jermaPortraitImage;
            }
            dialogueCount++;
            dialogueText.text = "Fire back or fly fast?";
        }

        else if (dialogueCount == 2)
        {
            if (dialogueBoxPortraitImage != null)
            {
                dialogueBoxPortraitImage.sprite = mavisPortraitImage;
            }
            Debug.Log("DialogueManager - Mission 4 - Dialogue Count 2");
            string endText = "Repairs will be needed.";
            StartCoroutine(EndDialogueScene(endText));

            dialogueCount = 0;
            MissionCompleteEvent?.Invoke(GameManager.Instance.CurrentMission, "Mission 4 Complete. Story unlocked.");
        }
    }
    
    private void Mission5()
    {
        Debug.Log("Mission 5 Dialogue Count = " + dialogueCount);
        shipUIManager.TurnOnShipUI();

        if (dialogueCount == 0)
        {
            missionTitleText.text = "Mission 5: It's Mine";

            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after delay
            StartCoroutine(StartGameCountdown());

            if (dialogueBoxPortraitImage != null)
            {
                dialogueBoxPortraitImage.sprite = jermaPortraitImage;
            }
            dialogueCount++;
            dialogueText.text = "The mining missle uncovers ore.";
        }
        else if (dialogueCount == 1)
        {
            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after

            dialogueCount++;
            dialogueText.text = "Increase your mining skill for better loot.";
        }

        else if (dialogueCount == 2)
        {
            Debug.Log("DialogueManager - Mission 5 - Dialogue Count 2");
            string endText = "We need ore for buying upgrades.";
            StartCoroutine(EndDialogueScene(endText));

            dialogueCount = 0;
            MissionCompleteEvent?.Invoke(GameManager.Instance.CurrentMission, "Mission 5 Complete. Story unlocked.");
        }
    }

    private void Mission6()
    {
        Debug.Log("Mission 6 Dialogue Count = " + dialogueCount);
        shipUIManager.TurnOnShipUI();

        if (dialogueCount == 0)
        {
            missionTitleText.text = "Mission 6: A Really Big Rock";

            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after delay
            StartCoroutine(StartGameCountdown());

            if (dialogueBoxPortraitImage != null)
            {
                dialogueBoxPortraitImage.sprite = mavisPortraitImage;
            }
            dialogueCount++;
            dialogueText.text = "Pirates guard the ore in this area";
        }
        else if (dialogueCount == 1)
        {
            Debug.Log("Spawning boss at position: " + planetSpawnPosition.transform.position);
            Debug.Log("DialogueManager Mission6 DialogueCount1");

            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after

            if (dialogueBoxPortraitImage != null)
            {
                dialogueBoxPortraitImage.sprite = jermaPortraitImage;
            }
            dialogueCount++;
            dialogueText.text = "There is something VERY big coming your way.";

            GameManager.Instance.SetState(GameState.BossBattle);

            Instantiate(boss1, planetSpawnPosition.transform.position, Quaternion.identity);
        }

        else if (dialogueCount == 2)
        {
            if (dialogueBoxPortraitImage != null)
            {
                dialogueBoxPortraitImage.sprite = mavisPortraitImage;
            }
            Debug.Log("DialogueManager Mission6 Dialogue Count2");
            string endText = "What a fortune!";
            StartCoroutine(EndDialogueScene(endText));

            dialogueCount = 0;
            MissionCompleteEvent?.Invoke(GameManager.Instance.CurrentMission, "Mission 6 Complete. Story unlocked.");
        }
    }

    private void Mission7()
    {
        Debug.Log("Mission 7 Dialogue Count = " + dialogueCount);
        shipUIManager.TurnOnShipUI();

        if (dialogueCount == 0)
        {
            missionTitleText.text = "Mission 7: Feel The Heat";

            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after delay
            StartCoroutine(StartGameCountdown());

            if (dialogueBoxPortraitImage != null)
            {
                dialogueBoxPortraitImage.sprite = mavisPortraitImage;
            }
            dialogueCount++;
            dialogueText.text = "Radiation from the Sun is damaging the ship.";
        }
        else if (dialogueCount == 1)
        {
            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after

            dialogueCount++;
            dialogueText.text = "It's getting hotter closer to the Sun.";
        }

        else if (dialogueCount == 2)
        {
            Debug.Log("DialogueManager - Mission 7 - Dialogue Count 2");
            string endText = "How can we possibly survive?";
            StartCoroutine(EndDialogueScene(endText));

            dialogueCount = 0;
            MissionCompleteEvent?.Invoke(GameManager.Instance.CurrentMission, "Mission 7 Complete. Story unlocked.");
        }
    }

    private void Mission8()
    {
        Debug.Log("Mission 8 Dialogue Count = " + dialogueCount);
        shipUIManager.TurnOnShipUI();

        if (dialogueCount == 0)
        {
            missionTitleText.text = "Mission 8: Fireflies";

            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after delay
            StartCoroutine(StartGameCountdown());

            dialogueCount++;
            dialogueText.text = "This area shows multiple distress signals.";
        }
        else if (dialogueCount == 1)
        {
            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after

            ShipGraveyardEvent?.Invoke(boss3);

            if (dialogueBoxPortraitImage != null)
            {
                dialogueBoxPortraitImage.sprite = jermaPortraitImage;
            }
            dialogueCount++;
            dialogueText.text = "A graveyard of ships.";
        }

        else if (dialogueCount == 2)
        {
            if (dialogueBoxPortraitImage != null)
            {
                dialogueBoxPortraitImage.sprite = mavisPortraitImage;
            }
            Debug.Log("DialogueManager - Mission 8 - Dialogue Count 2");
            string endText = "You've traveled farther than most. Keep going.";
            StartCoroutine(EndDialogueScene(endText));

            dialogueCount = 0;
            MissionCompleteEvent?.Invoke(GameManager.Instance.CurrentMission, "Mission 8 Complete. Story unlocked.");
        }
    }

    private void Mission9()
    {
        Debug.Log("Mission 9 Dialogue Count = " + dialogueCount);
        shipUIManager.TurnOnShipUI();

        if (dialogueCount == 0)
        {
            missionTitleText.text = "Mission 9: Comic Chaos";

            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after delay
            StartCoroutine(StartGameCountdown());

            if (dialogueBoxPortraitImage != null)
            {
                dialogueBoxPortraitImage.sprite = jermaPortraitImage;
            }
            dialogueCount++;
            dialogueText.text = "Here comes maddness.";
        }
        else if (dialogueCount == 1)
        {
            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after

            dialogueCount++;
            dialogueText.text = "Let nothing stop you. You're so close.";
        }

        else if (dialogueCount == 2)
        {
            if (dialogueBoxPortraitImage != null)
            {
                dialogueBoxPortraitImage.sprite = mavisPortraitImage;
            }
            Debug.Log("DialogueManager - Mission 9 - Dialogue Count 2");
            string endText = "Thank you for flying. We're all cheering for you at home.";
            StartCoroutine(EndDialogueScene(endText));

            dialogueCount = 0;
            MissionCompleteEvent?.Invoke(GameManager.Instance.CurrentMission, "Mission 9 Complete. Story unlocked.");
        }
    }

    private void Mission10()
    {
        Debug.Log("Mission 10 Dialogue Count = " + dialogueCount);
        shipUIManager.TurnOnShipUI();

        if (dialogueCount == 0)
        {
            missionTitleText.text = "Mission 10: Pirate King";

            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after delay
            StartCoroutine(StartGameCountdown());

            if (dialogueBoxPortraitImage != null)
            {
                dialogueBoxPortraitImage.sprite = mavisPortraitImage;
            }
            dialogueCount++;
            dialogueText.text = "You've angered GoodCorp. They're sending their whole fleet.";
        }
        else if (dialogueCount == 1)
        {
            Debug.Log("Spawning boss at position: " + planetSpawnPosition.transform.position);
            Debug.Log("DialogueManager Mission10 DialogueCount1");

            StartCoroutine(FadeInDialogueBox());
            StartCoroutine(FadeOutDialogueBox(dialogueTimer)); //Hide dialogue box after

            if (dialogueBoxPortraitImage != null)
            {
                dialogueBoxPortraitImage.sprite = jermaPortraitImage;
            }
            dialogueCount++;
            dialogueText.text = "There is something VERY VERY big coming your way.";

            GameManager.Instance.SetState(GameState.BossBattle);

            Instantiate(boss2, planetSpawnPosition.transform.position, Quaternion.identity);
        }

        else if (dialogueCount == 2)
        {
            if (dialogueBoxPortraitImage != null)
            {
                dialogueBoxPortraitImage.sprite = mavisPortraitImage;
            }
            Debug.Log("DialogueManager - Mission 10 - Dialogue Count 2");
            string endText = "Our hero! You reached the Sun.";
            StartCoroutine(EndDialogueScene(endText));

            dialogueCount = 0;
            MissionCompleteEvent?.Invoke(GameManager.Instance.CurrentMission, "Mission 10 Complete. World Saved?");
        }

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

    IEnumerator DelayedSpawnActions1()
    {
        yield return new WaitForSeconds(3f);
        // Spawn 1 obstacle
        SpawnSpecialEvent?.Invoke();
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

    IEnumerator MissionDialogueDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        MissionDialogue();
    }



}