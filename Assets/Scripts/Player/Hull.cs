using System;
using UnityEngine;

// This script stores and changes the hull value for a space ship.

public class Hull : MonoBehaviour
{
    // Define events for hull changes
    public static event Action<float> OnHullMaxChanged;
    public static event Action<float> OnCurrentHullChanged;
    public static event Action<float> PopUpTextEvent;

    private PlayerStatsManager playerStatsManager;

    [SerializeField] private float hullMax;
    private float maxRepairTime = 200f;
    private float minRepairTime = 1f;
    private float repairCountdown;
    private bool isInitialized = false;

    private void OnEnable()
    {
        DataPersister.InitializationComplete += HandleInitializationComplete;
    }
    private void OnDisable()
    {
        DataPersister.InitializationComplete -= HandleInitializationComplete;
    }

    private void HandleInitializationComplete()
    {
        playerStatsManager = FindFirstObjectByType<PlayerStatsManager>();
        OnHullMaxChanged?.Invoke(hullMax);
        repairCountdown = maxRepairTime;
        isInitialized = true;
    }

    private void Update()
    {
        if (isInitialized)
        {
            RepairShip();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damage damageComponent = collision.gameObject.GetComponent<Damage>();

        if (damageComponent != null)
        {
            // Notify PlayerStatsManager to change health
            PlayerStatsManager.Instance.ChangeHealth(-damageComponent.GetDamage());
            PopUpTextEvent?.Invoke(-damageComponent.GetDamage());
        }
    }

    private void RepairShip()
    {
        repairCountdown -= Time.deltaTime;

        if (repairCountdown <= 0f && playerStatsManager.PlayerCurrentHull > 0)
        {
            float repairValue = 1f;
            OnCurrentHullChanged?.Invoke(repairValue);
            repairCountdown = maxRepairTime - DataPersister.Instance.CurrentGameData.playerData[0].roboticsSkill;
            if (repairCountdown < minRepairTime)
            {
                repairCountdown = minRepairTime;
            }
            // Trigger the event to notify about the current hull change
            OnCurrentHullChanged?.Invoke(playerStatsManager.PlayerCurrentHull);
        }
    }

}