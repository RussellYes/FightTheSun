using System;
using UnityEngine;

// This script store and changes the hull value for a space ship.

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

    private void Start()
    {
        playerStatsManager = FindFirstObjectByType<PlayerStatsManager>();

        // Trigger events to initialize values
        OnHullMaxChanged?.Invoke(hullMax);
        OnCurrentHullChanged?.Invoke(hullMax); // Start with full hull
        repairCountdown = maxRepairTime;
    }

    private void Update()
    {
        RepairShip();
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
            repairCountdown = maxRepairTime - playerStatsManager.RoboticsSkill;
            if (repairCountdown < minRepairTime)
            {
                repairCountdown = minRepairTime;
            }
            // Trigger the event to notify about the current hull change
            OnCurrentHullChanged?.Invoke(playerStatsManager.PlayerCurrentHull);
        }
    }

}