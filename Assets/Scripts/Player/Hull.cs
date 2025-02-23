using System;
using UnityEngine;

public class Hull : MonoBehaviour
{
    // Define events for hull changes
    public static event Action<float> OnHullMaxChanged;
    public static event Action<float> OnCurrentHullChanged;

    [SerializeField] private float hullMax;

    private void Start()
    {
        // Trigger events to initialize values
        OnHullMaxChanged?.Invoke(hullMax);
        OnCurrentHullChanged?.Invoke(hullMax); // Start with full hull
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damage damageComponent = collision.gameObject.GetComponent<Damage>();

        if (damageComponent != null)
        {
            // Notify PlayerStatsManager to change health
            PlayerStatsManager.Instance.ChangeHealth(-damageComponent.GetDamage());
        }
    }
}