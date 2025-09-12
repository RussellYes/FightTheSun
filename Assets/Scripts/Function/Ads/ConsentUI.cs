using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// This script gets consent to show personalized ads once, saves the consent forever, and its event is subscribed to by AdsBootstrap.
public class ConsentUI : MonoBehaviour
{
    // Static event so anyone can listen
    public static event Action<bool> OnConsentChosen;

    [Header("Wire these in Inspector")]
    [SerializeField] private CanvasGroup panel;   // container group (set Interactable/BlocksRaycasts)
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private void Awake()
    {
        yesButton.onClick.AddListener(() => Choose(true));
        noButton.onClick.AddListener(() => Choose(false));
        HideInstant();
    }

    public void Show(string text)
    {
        message.text = text;
        panel.alpha = 1f;
        panel.blocksRaycasts = true;
        panel.interactable = true;
    }

    public void HideInstant()
    {
        panel.alpha = 0f;
        panel.blocksRaycasts = false;
        panel.interactable = false;
    }

    private void Choose(bool consent)
    {
        HideInstant();
        OnConsentChosen?.Invoke(consent);
    }
}
