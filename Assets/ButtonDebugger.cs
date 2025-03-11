using UnityEngine;
using UnityEngine.UI;

public class ButtonDebugger : MonoBehaviour
{
    public Button leftButton;
    public Button rightButton;

    private void Start()
    {
        if (leftButton == null)
        {
            Debug.LogError("ButtonDebugger: leftButton is not assigned.");
            return;
        }

        leftButton.onClick.AddListener(OnButtonClick1);

        if (rightButton == null)
        {
            Debug.LogError("ButtonDebugger: rightButton is not assigned.");
            return;
        }

        rightButton.onClick.AddListener(OnButtonClick2);
    }

    private void OnButtonClick1()
    {
        Debug.Log("Button1 clicked!");
    }
    private void OnButtonClick2()
    {
        Debug.Log("Button2 clicked!");
    }
}