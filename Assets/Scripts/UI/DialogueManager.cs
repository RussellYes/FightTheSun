using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameManager;
using System.Security.Cryptography;
using Unity.VisualScripting;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private GameObject dialogueBoxUI;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button continueStartDialogueButton;
    private float dialogueCount = 0f;

    [Header("Mission 1 - Dashboard Controls")]
    [SerializeField] private GameObject highLightArrowPrefab;
    [SerializeField] private GameObject shipHUDUI;
    [SerializeField] private GameObject shipUIBackground;
    [SerializeField] private GameObject hullMeter;
    [SerializeField] private GameObject rightButtonObject;
    [SerializeField] private Button rightButton;
    [SerializeField] private GameObject leftButtonObject;
    [SerializeField] private Button leftButton;
    [SerializeField] private GameObject speedMeter;
    [SerializeField] private GameObject throttleUp;
    [SerializeField] private GameObject throttleDown;
    [SerializeField] private GameObject checkpointUI;
    [SerializeField] private GameObject scoreMeter;
    [SerializeField] private GameObject pauseButton;

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
        continueStartDialogueButton.onClick.AddListener(() =>
        {
            MissionDialogue();
        });
    }

    public void ShowDialogueTimed(string dialogueKey, float time)
    {
        dialogueText.text = (dialogueKey);
        dialogueBoxUI.SetActive(true);
        Invoke("HideDialogue", time);
    }

    public void HideDialogue()
    {
        dialogueBoxUI.SetActive(false);
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
        
        shipHUDUI.SetActive(true);
        shipUIBackground.SetActive(false);
        hullMeter.SetActive(false);
        rightButtonObject.SetActive(false);
        leftButtonObject.SetActive(false);
        speedMeter.SetActive(false);
        throttleUp.SetActive(false);
        throttleDown.SetActive(false);
        checkpointUI.SetActive(false);
        scoreMeter.SetActive(false);


        if (dialogueCount == 0)
        {
            dialogueBoxUI.SetActive(true);
            dialogueCount++;
            dialogueText.text = "Good morning pilot! Welcome to your new ship";
        }
        else if (dialogueCount == 1)
        {
            dialogueBoxUI.SetActive(true);

            continueStartDialogueButton.interactable = false; // Disable the button


            dialogueText.text = "Oh no! An asteroid. Let's dodge the asteroid with these controls.";

            //Show some ShipUI
            shipUIBackground.SetActive(true);
            leftButtonObject.SetActive(true);
            rightButtonObject.SetActive(true);

            // Add listeners to the buttons
            leftButton.onClick.AddListener(LeftRightButtonPushed);
            rightButton.onClick.AddListener(LeftRightButtonPushed);

            // Get the RectTransform of the leftButton
            RectTransform leftButtonRect = leftButton.GetComponent<RectTransform>();

            // Apply offsets directly (no need for position conversion)
            Vector3 xOffset = new Vector3(16, 0, 0); // Adjust these values as needed
            Vector3 yOffset = new Vector3(0, 60, 0); // Adjust these values as needed
            Vector3 arrowPosition = leftButtonRect.position + xOffset + yOffset;

            // Instantiate the arrow prefab and set its parent to the same canvas as the leftButton
            currentArrowInstance = Instantiate(highLightArrowPrefab, arrowPosition, Quaternion.identity, leftButtonRect.parent);

            // Ensure the arrow is rendered in front by setting its sibling index
            currentArrowInstance.transform.SetAsLastSibling();

            waitingForButtonPress = true;
        }
        else if (dialogueCount == 2)
        {
            dialogueBoxUI.SetActive(true);
            continueStartDialogueButton.interactable = true; // Enable the button
            dialogueCount = 3;
            dialogueText.text = "Look out!. More asteroid incoming.";
        }
        else if (dialogueCount == 3)
        {
            dialogueBoxUI.SetActive(false);
            GameManager.Instance.SetState(GameState.Playing);
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

}