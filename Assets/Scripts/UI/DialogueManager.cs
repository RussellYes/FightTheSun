using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameManager;
using System.Security.Cryptography;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private GameObject dialogueBoxUI;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button continueStartDialogueButton;

    [Header("Mission 1 - Dashboard Controls")]
    [SerializeField] private GameObject highLightArrowPrefab;
    [SerializeField] private GameObject shipHUDUI;
    [SerializeField] private GameObject hullMeter;
    [SerializeField] private GameObject rightButton;
    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject speedMeter;
    [SerializeField] private GameObject throttleUp;
    [SerializeField] private GameObject throttleDown;
    [SerializeField] private GameObject checkpointUI;
    [SerializeField] private GameObject scoreMeter;
    [SerializeField] private GameObject pauseButton;

    private float dialogueCount = 0f;
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
        continueStartDialogueButton.onClick.AddListener(() => {
            MissionDialogue();
        });
    }

    public void ShowDialogue(string dialogueKey)
    {
        // Example: Load dialogue text based on the key
        dialogueText.text = GetDialogueText(dialogueKey);
        dialogueBoxUI.SetActive(true);
    }

    public void ShowDialogueTimed(string dialogueKey, float time)
    {
        dialogueText.text = GetDialogueText(dialogueKey);
        dialogueBoxUI.SetActive(true);
        Invoke("HideDialogue", time);
    }

    public void HideDialogue()
    {
        dialogueCount = 0;
        GameManager.Instance.SetState(GameState.Playing);
        dialogueBoxUI.SetActive(false);
    }

    private string GetDialogueText(string key)
    {
        // Replace this with your actual dialogue loading logic
        switch (key)
        {
            case "StartDialogue":
                return "Welcome to the game!";
            case "EndDialogue":
                return "Thanks for playing!";
            default:
                return "Dialogue not found.";
        }
    }

    private void MissionDialogue()
    {
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
                break;
        }
    }

    private void Mission1()
    {
        ClearArrow();
        shipHUDUI.SetActive(true);
        hullMeter.SetActive(false);
        rightButton.SetActive(false);
        leftButton.SetActive(false);
        speedMeter.SetActive(false);
        throttleUp.SetActive(false);
        throttleDown.SetActive(false);
        checkpointUI.SetActive(false);
        scoreMeter.SetActive(false);


        if (dialogueCount == 0)
        {
            dialogueCount++;
            dialogueText.text = "Good morning pilot! Welcome to your new ship";
        }
        else if (dialogueCount == 1)
        {
            dialogueText.text = "Oh no! An astroid. Let's dodge the astroid with these controls.";

            Vector3 xOffset = new Vector3(100, 0, 0);
            Vector3 yOffset = new Vector3(0, 200, 0);
            currentArrowInstance =  Instantiate(highLightArrowPrefab, leftButton.transform.position + xOffset + yOffset, Quaternion.identity);
            waitingForButtonPress = true;
        }
        else if (dialogueCount == 2)
        {
            dialogueCount = 0;
            dialogueText.text = "Great work! Keep flying to the checkpoit.";
        }
    }

    private void Mission2()
    {
        ClearArrow();
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
        ClearArrow();
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

    private void ClearArrow()
    {

        if (currentArrowInstance != null)
        {
            Destroy(highLightArrowPrefab);
        }
    }

}