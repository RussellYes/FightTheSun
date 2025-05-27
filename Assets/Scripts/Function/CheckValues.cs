using TMPro;
using UnityEngine;

public class CheckValues : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyTotalText;
    private ScoreManager scoreManager;

    void Start()
    {
        // Cache the ScoreManager reference
        scoreManager = FindFirstObjectByType<ScoreManager>();
        
        // Initial update
        UpdateMoneyText();
    }

    void Update()
    {
        UpdateMoneyText();
    }

    private void UpdateMoneyText()
    {
        if (scoreManager != null)
        {
            // Use the GetTotalMoney() method to access the value
            moneyTotalText.text = scoreManager.GetTotalMoney().ToString();
        }
    }
}