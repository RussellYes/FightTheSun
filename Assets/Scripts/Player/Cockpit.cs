using System;
using UnityEngine;

// This script stores the values and visuals of the cockpit ship module.

public class Cockpit : MonoBehaviour
{
    // Define the event
    public static event Action<float> OnCockpitMassChanged;
    public static event Action<float> OnCockpitThrustChanged;

    private PlayerStatsManager playerStatsManager;
    private Damage damage;

    [Header("Cockpit Settings")]
    [SerializeField] private float mass;
    private float thrust = 1;


    [Header("Visuals")]
    [SerializeField] private SpriteRenderer hullSpriteRenderer;
    [SerializeField] private Sprite hull100;
    [SerializeField] private Sprite hull75;
    [SerializeField] private Sprite hull50;
    [SerializeField] private Sprite hull25;


    private void OnEnable()
    {
        PlayerStatsManager.PlayerHullPercentEvent += Visual;
    }

    private void OnDisable()
    {
        PlayerStatsManager.PlayerHullPercentEvent -= Visual;
    }

    private void Start()
    {
        playerStatsManager = FindFirstObjectByType<PlayerStatsManager>();
        damage = GetComponent<Damage>();
        damage.ChangeDamage(playerStatsManager.CombatSkill);

        // Invoke the event to add mass
        OnCockpitMassChanged?.Invoke(mass);
        // Invoke the event to add thrust
        OnCockpitThrustChanged?.Invoke(thrust);
    }
    private void OnDestroy()
    {
        // Invoke the event to subtract mass
        OnCockpitMassChanged?.Invoke(-mass);
        // Invoke the event to subtract thrust
        OnCockpitThrustChanged?.Invoke(-thrust);
    }

    private void Visual(int percentHull)
    {
        if (percentHull >= 100)
        {
            hullSpriteRenderer.sprite = hull100;
        }
        else if (percentHull >= 75)
        {
            hullSpriteRenderer.sprite = hull75;
        }
        else if (percentHull >= 50)
        {
            hullSpriteRenderer.sprite = hull50;
        }
        else if (percentHull >= 25)
        {
            hullSpriteRenderer.sprite = hull25;
        }

    }


}