using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    private PlayerStatsManager playerStatsManager;
    [SerializeField] private Image barImage;

    private void Start()
    {
        playerStatsManager = FindAnyObjectByType<PlayerStatsManager>();
        PlayerStatsManager.OnCurrentHullChanged += PlayerStatsManager_OnCurrentHullChanged;
    }

    private void OnDisable()
    {
        PlayerStatsManager.OnCurrentHullChanged -= PlayerStatsManager_OnCurrentHullChanged;
    }

    private void PlayerStatsManager_OnCurrentHullChanged(object sender, PlayerStatsManager.OnCurrentHullChangedEventArgs e)
    {
        barImage.fillAmount = e.progressNormalized;
    }
}