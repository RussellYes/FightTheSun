using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameManager;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private GameObject dialogueBoxUI;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button closeStartDialogueButton;
    [SerializeField] private Button continueStartDialogueButton;

    [Header("Mission 1 - Dashboard Controls")]
    [SerializeField] private GameObject higlightArrowPrefab;
    [SerializeField] private GameObject ShipHUDUI;
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
        closeStartDialogueButton.onClick.AddListener(() => {
            HideDialogue();
        });
        continueStartDialogueButton.onClick.AddListener(() => {
            Mission1();
        });
    }

    public void ShowDialogue(string dialogueKey)
    {
        // Example: Load dialogue text based on the key
        dialogueText.text = GetDialogueText(dialogueKey);
        dialogueBoxUI.SetActive(true);
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

    private void Mission1()
    {
        if (dialogueCount == 0)
        {
            dialogueCount++;
            dialogueText.text = "Mission 1: Destroy 10 obstacles";
        }
        else if (dialogueCount == 1)
        {
            dialogueCount++;
            dialogueText.text = "Good job! Now, reach the goal!";
        }
        else
        {
            HideDialogue();
        }
        




    }



}