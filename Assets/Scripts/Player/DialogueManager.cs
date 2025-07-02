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
    public static DialogueManager Instance;
    public static UnityEvent SpawnSpecialEvent = new UnityEvent();
    public static event Action<string, string, float> StartDialogueEvent;
    public static event Action<float> HideDialogueEvent;
    public static event Action<int, float> FlipFlopPicEvent;

    private GameManager gameManager;
    [SerializeField] private ShipUIManager shipUIManager;

    [Header("Dialogue UI")]
    [SerializeField] private Button continueStartDialogueButton;
    private int dialogueCount = 99;

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
    private Transform missileButtonObjectRect;
    private Transform miningClawButtonObjectRect;
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
        OnDataInitialized();
    }
    private void OnDataInitialized()
    {
        Debug.Log("DialogueManager OnDataInitialized");

        gameManager = GameManager.Instance;


        miningClawButtonObjectRect = GameObject.FindFirstObjectByType<FindMiningClawButton>()?.GetComponent<RectTransform>();
        if (miningClawButtonObjectRect == null)
        {
            Debug.LogError("DialogueManager Start - MiningClawButton object not found in the scene.");
        }
        missileButtonObjectRect = GameObject.FindFirstObjectByType<FindMissileButton>()?.GetComponent<RectTransform>();
        if (missileButtonObjectRect == null)
        {
            Debug.LogError("DialogueManager Start - MissileButton object not found in the scene.");
        }
        hullBarObjectRect = GameObject.FindFirstObjectByType<FindHullBar>()?.GetComponent<RectTransform>();
        if (hullBarObjectRect == null)
        {
            Debug.LogError("DialogueManager Start - HullBar object not found in the scene.");
        }
        thrustBarObjectRect = GameObject.FindFirstObjectByType<FindThrustBar>()?.GetComponent<RectTransform>();
        if (thrustBarObjectRect == null)
        {
            Debug.LogError("DialogueManager Start - ThrustBar object not found in the scene.");
        }
        CheckpointMeterObjectRect = GameObject.FindFirstObjectByType<FindCheckpointMeterObject>()?.GetComponent<RectTransform>();
        if (CheckpointMeterObjectRect == null)
        {
            Debug.LogError("DialogueManager Start - CheckpointMeter object not found in the scene.");
        }
    }
    private void OnEnable()
    {
        PlayerStatsManager.GoalProgressEvent += MissionDialogue;
        GameManager.MissionDialogueEvent += MissionDialogue;
        //DataPersister.InitializationComplete += OnDataInitialized;
    }

    private void OnDisable()
    {
        PlayerStatsManager.GoalProgressEvent -= MissionDialogue;
        GameManager.MissionDialogueEvent -= MissionDialogue;
        DataPersister.InitializationComplete -= OnDataInitialized;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset dialogue count when a new scene is loaded
        dialogueCount = 99;

        Debug.Log($"New scene loaded: {scene.name}. Dialogue count reset.");
    }

    public void MissionDialogue(int amt)
    {
        dialogueCount = amt;
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

    //Missions use a numbering system as follows:
    // 0 = Mission start
    // 1 = 25% complete
    // 2 = 50% complete
    // 3 = 75% complete
    // 4 = 100% complete
    // 5 = mission end
    // 6+ = additional dialogue for the mission

    private void Mission1()
    {
        Debug.Log("Mission 1 Dialogue Count = " + dialogueCount);
        shipUIManager.TurnOnShipUI();
        int sunCountDialogue = DataPersister.Instance.CurrentGameData.sunCount;

        if (dialogueCount == 0)
        {
            missionTitleText.text = "Mission 1: Learn To Drive";

            StartCoroutine(DelayedSpawnActions1());
            StartGameCountdownEvent?.Invoke();

            FlipFlopPicEvent?.Invoke(4, 0.4f);

            if (sunCountDialogue <= 1)
            {
                StartDialogueEvent?.Invoke("mavis", "Asteroid! Swipe left and right to move.", 4f);
            }
            if (sunCountDialogue > 1)
            {
                int randomPick = UnityEngine.Random.Range(0, 4);

                if (randomPick == 0)
                {
                    StartDialogueEvent?.Invoke("mavis", "Asteroid... This is all so familiar.", 4f);
                }
                else if (randomPick == 1)
                {
                    StartDialogueEvent?.Invoke("mavis", "Left and right and left and right.", 4f);
                }
                else if (randomPick == 2)
                {
                    StartDialogueEvent?.Invoke("mavis", "Asteroid! Have we been here before?", 4f);
                }
                else
                {
                    StartDialogueEvent?.Invoke("mavis", "Asteroids! Don't fly into one this time.", 4f);
                }
            }
            float delayTime = 7;
            StartCoroutine(MissionDialogueDelay(delayTime, 6));
        }
        else if (dialogueCount == 6)
        {
            float dialogueTimer = 4f;
            StartDialogueEvent?.Invoke("mavis", "Protect the ship's hull, this <b><color=red>RED</color></b>  gauge.", dialogueTimer);

            // Apply offsets directly
            Vector3 xOffset = new Vector3(0, 0, 0); // Adjust these values as needed
            Vector3 yOffset = new Vector3(0, 150, 0); // Adjust these values as needed
            Vector3 arrowPosition = hullBarObjectRect.position + xOffset + yOffset;

            // Instantiate the arrow prefab with the specified position and rotation, and set its parent
            currentArrowInstance = Instantiate(highLightArrowPrefab, arrowPosition, Quaternion.identity, hullBarObjectRect);

            // Ensure the arrow is rendered in front by setting its sibling index
            currentArrowInstance.transform.SetAsLastSibling();
            StartCoroutine(DestroyArrow(dialogueTimer));
        }
        else if (dialogueCount == 2)
        {
            StartDialogueEvent?.Invoke("mavis", "Swipe up and down to change speed.", 4f);

            StartCoroutine(MissionDialogueDelay(5f, 8));
        }
        else if (dialogueCount == 8)
        {
            float dialogueTimer = 4f;
            StartDialogueEvent?.Invoke("mavis", "This <b><color=blue>BLUE</color></b> gauge shows your speed", dialogueTimer);

            // Apply offsets directly
            Vector3 xOffset = new Vector3(0, 0, 0); // Adjust these values as needed
            Vector3 yOffset = new Vector3(0, 150, 0); // Adjust these values as needed
            Vector3 arrowPosition = thrustBarObjectRect.position + xOffset + yOffset;
            // Instantiate the arrow prefab with the specified position and rotation, and set its parent
            currentArrowInstance = Instantiate(highLightArrowPrefab, arrowPosition, Quaternion.identity, hullBarObjectRect);
            // Ensure the arrow is rendered in front by setting its sibling index
            currentArrowInstance.transform.SetAsLastSibling();
            StartCoroutine(DestroyArrow(dialogueTimer));
        }
        else if (dialogueCount == 4)
        {
            Debug.Log("DialogueManager - Mission 1 - Dialogue Count 4");
            StartDialogueEvent?.Invoke("mavis", "Race to the sun or slow down for loot?", 4f);
            dialogueCount = 4;
            MissionCompleteEvent?.Invoke(GameManager.Instance.CurrentMission, "Mission 1 Complete. Story unlocked.");
            StartCoroutine(EndDialogueScene());
        }
        
    }
    private void Mission2()
    {
        Debug.Log("Mission 2 Dialogue Count = " + dialogueCount);
        shipUIManager.TurnOnShipUI();

        if (dialogueCount == 0)
        {
            missionTitleText.text = "Mission 2: Claw Forward";

            StartGameCountdownEvent?.Invoke();

            int sunCountDialogue = DataPersister.Instance.CurrentGameData.sunCount;

            if (sunCountDialogue <= 1)
            {
                StartDialogueEvent?.Invoke("jerma", "Use the mining claw to collect ore.", 4f);
            }

            if (sunCountDialogue > 1)
            {
                int randomPick = UnityEngine.Random.Range(0, 4);

                if (randomPick == 0)
                {
                    StartDialogueEvent?.Invoke("jerma", "Have you used the mining claw? It crushes asteroids.", 4f);
                }
                else if (randomPick == 1)
                {
                    StartDialogueEvent?.Invoke("jerma", "THE CLAAAAAW!!", 4f);
                }
                else if (randomPick == 2)
                {
                    StartDialogueEvent?.Invoke("jerma", "Can the claw mine faster?", 4f);
                }
                else
                {
                    StartDialogueEvent?.Invoke("jerma", "What can the mining claw... claw?", 4f);
                }

            }
        }
        else if (dialogueCount == 1)
        {
            float dialogueTimer = 4f;
            StartDialogueEvent?.Invoke("jerma", "Activate, aim, then wait for the claw to mine.", dialogueTimer);

            // Apply offsets directly
            Vector3 xOffset = new Vector3(0, 0, 0); // Adjust these values as needed
            Vector3 yOffset = new Vector3(0, 150, 0); // Adjust these values as needed
            Vector3 arrowPosition = miningClawButtonObjectRect.position + xOffset + yOffset;

            // Instantiate the arrow prefab with the specified position and rotation, and set its parent
            currentArrowInstance = Instantiate(highLightArrowPrefab, arrowPosition, Quaternion.identity, miningClawButtonObjectRect);

            // Ensure the arrow is rendered in front by setting its sibling index
            currentArrowInstance.transform.SetAsLastSibling();
            StartCoroutine(DestroyArrow(dialogueTimer));
        }
        else if (dialogueCount == 3)
        {
            float dialogueTimer = 4f;
            StartDialogueEvent?.Invoke("jerma", "Look! This meter shows the progress to our checkpoint.", dialogueTimer);

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
        }

        else if (dialogueCount == 4)
        {
            Debug.Log("DialogueManager - Mission 2 - Dialogue Count 2");
            StartDialogueEvent?.Invoke("mavis", "Yeah! We're at the space station. Check out the exchange.", 4f);

            StartCoroutine(EndDialogueScene());
            dialogueCount = 4;
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

            StartDialogueEvent?.Invoke("mavis", "Collect Shipwrecks to repair your ship.", 4f);
            StartGameCountdownEvent?.Invoke();
            StartCoroutine(DelayedSpawnActions1());
        }
        else if (dialogueCount == 2)
        {
            StartDialogueEvent?.Invoke("mavis", "Shipwreck scrap is worth money too!", 4f);
        }

        else if (dialogueCount == 4)
        {
            Debug.Log("DialogueManager - Mission 3 - Dialogue Count 2");
            StartDialogueEvent?.Invoke("mavis", "Life moves fast. If you're slow, buy upgrades.", 4f);
            StartCoroutine(EndDialogueScene());

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

            StartDialogueEvent?.Invoke("mavis", "Not everyone is friendly in space.", 4f);
            StartGameCountdownEvent?.Invoke();
            StartCoroutine(DelayedSpawnActions1());
        }
        else if (dialogueCount == 2)
        {
            StartDialogueEvent?.Invoke("jerma", "Fire back or fly fast?", 4f);
        }

        else if (dialogueCount == 4)
        {
            Debug.Log("DialogueManager - Mission 4 - Dialogue Count 2");
            StartDialogueEvent?.Invoke("mavis", "Repairs will be needed.", 4f);
            StartCoroutine(EndDialogueScene());

            dialogueCount = 4;
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

            StartDialogueEvent?.Invoke("jerma", "The mining missle uncovers ore.", 4f);
            StartGameCountdownEvent?.Invoke();
        }
        else if (dialogueCount == 2)
        {
            StartDialogueEvent?.Invoke("jerma", "Increase your mining skill for better loot.", 4f);
        }

        else if (dialogueCount == 4)
        {
            Debug.Log("DialogueManager - Mission 5 - Dialogue Count 2");
            StartDialogueEvent?.Invoke("jerma", "We need ore for buying upgrades.", 4f);
            StartCoroutine(EndDialogueScene());

            dialogueCount = 4;
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

            StartDialogueEvent?.Invoke("mavis", "Pirates guard the ore in this area.", 4f);
            StartGameCountdownEvent?.Invoke();
        }
        else if (dialogueCount == 2)
        {
            Debug.Log("Spawning boss at position: " + planetSpawnPosition.transform.position);
            Debug.Log("DialogueManager Mission6 DialogueCount1");

            StartDialogueEvent?.Invoke("jerma", "There is something VERY big coming your way.", 4f);

            GameManager.Instance.SetState(GameState.BossBattle);

            Instantiate(boss1, planetSpawnPosition.transform.position, Quaternion.identity);
        }

        else if (dialogueCount == 4)
        {
            Debug.Log("DialogueManager Mission6 Dialogue Count2");
            StartDialogueEvent?.Invoke("mavis", "What a fortune!", 4f);
            StartCoroutine(EndDialogueScene());

            dialogueCount = 4;
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

            StartDialogueEvent?.Invoke("mavis", "Radiation from the Sun is damaging the ship.", 4f);
            StartGameCountdownEvent?.Invoke();
        }
        else if (dialogueCount == 2)
        {
            StartDialogueEvent?.Invoke("mavis", "It's getting hotter closer to the Sun.", 4f);
        }

        else if (dialogueCount == 4)
        {
            Debug.Log("DialogueManager - Mission 7 - Dialogue Count 2");
            StartDialogueEvent?.Invoke("mavis", "How can we possibly survive?", 4f);
            StartCoroutine(EndDialogueScene());
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

            StartDialogueEvent?.Invoke("jerma", "This area shows multiple distress signals.", 4f);
            StartGameCountdownEvent?.Invoke();
        }
        else if (dialogueCount == 2)
        {
            StartDialogueEvent?.Invoke("jerma", "A graveyard of ships.", 4f);
            ShipGraveyardEvent?.Invoke(boss3);
        }

        else if (dialogueCount == 4)
        {
            Debug.Log("DialogueManager - Mission 8 - Dialogue Count 2");
            StartDialogueEvent?.Invoke("mavis", "You've traveled farther than most. Keep going.", 4f);
            StartCoroutine(EndDialogueScene());

            dialogueCount = 4;
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

            StartDialogueEvent?.Invoke("jerma", "Here comes maddness.", 4f);
            StartGameCountdownEvent?.Invoke();
        }
        else if (dialogueCount == 2)
        {
            StartDialogueEvent?.Invoke("mavis", "Let nothing stop you. You're so close.", 4f);
        }

        else if (dialogueCount == 4)
        {
            Debug.Log("DialogueManager - Mission 9 - Dialogue Count 2");
            StartDialogueEvent?.Invoke("mavis", "Thank you for flying. We're all cheering for you at home.", 4f);
            StartCoroutine(EndDialogueScene());

            dialogueCount = 4;
            MissionCompleteEvent?.Invoke(GameManager.Instance.CurrentMission, "Mission 9 Complete. Story unlocked.");
        }
    }

    private void Mission10()
    {
        Debug.Log("Mission 10 Dialogue Count = " + dialogueCount);
        shipUIManager.TurnOnShipUI();

        if (dialogueCount == 0)
        {
            missionTitleText.text = "Mission 10: Robber Barron";

            StartDialogueEvent?.Invoke("mavis", "You've angered GoodCorp. They're sending their whole fleet.", 4f);
            StartGameCountdownEvent?.Invoke();
        }
        else if (dialogueCount == 2)
        {
            StartDialogueEvent?.Invoke("jerma", "There is something VERY VERY big coming your way.", 4f);

            GameManager.Instance.SetState(GameState.BossBattle);
            Instantiate(boss2, planetSpawnPosition.transform.position, Quaternion.identity);
            Debug.Log("Spawning boss at position: " + planetSpawnPosition.transform.position);
        }

        else if (dialogueCount == 4)
        {
            Debug.Log("DialogueManager - Mission 10 - Dialogue Count 2");
            StartDialogueEvent?.Invoke("mavis", "Our hero! You reached the Sun.", 4f);
            StartCoroutine(EndDialogueScene());

            dialogueCount = 4;
            MissionCompleteEvent?.Invoke(GameManager.Instance.CurrentMission, "Mission 10 Complete. World Saved?");
        }

    }
    public void HideDialogue()
    {
        HideDialogueEvent?.Invoke(0f);
    }
  

    IEnumerator DelayedSpawnActions1()
    {
        float delayTime = 3f; // Adjust the delay time as needed
        yield return new WaitForSeconds(delayTime);
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

    IEnumerator EndDialogueScene()
    {
        Debug.Log("DialogueManager - End Dialogue Scene");
        gameManager.SetState(GameManager.GameState.EndDialogue);

        Instantiate(planet1, planetSpawnPosition.transform.position, Quaternion.identity);

        float planetMoveTime = 3f;
        yield return new WaitForSeconds(planetMoveTime);

        //Trigger end
        gameManager.EndGame(true);
    }

    IEnumerator MissionDialogueDelay(float delayTime, int amt)
    {
        yield return new WaitForSeconds(delayTime);
        MissionDialogue(amt);
    }
}