using System;
using UnityEngine;

public class Hull : MonoBehaviour
{
    // Define events for hull changes
    public static event Action<float> OnHullMaxChanged;
    public static event Action<float> OnCurrentHullChanged;

    private float currentHull;
    [SerializeField] private float hullMax;

    private void Start()
    {
        currentHull = hullMax;

        // Trigger events to initialize values
        OnHullMaxChanged?.Invoke(hullMax);
        OnCurrentHullChanged?.Invoke(currentHull);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damage damageComponent = collision.gameObject.GetComponent<Damage>();

        if (damageComponent != null)
        {
            ChangeHealth(damageComponent.GetDamage());
        }
    }

    private void ChangeHealth(float damage)
    {
        currentHull -= damage;
        Debug.Log("Hull changed. Current hull: " + currentHull);

        // Trigger event for current hull change
        OnCurrentHullChanged?.Invoke(currentHull);

        if (currentHull <= 0)
        {
            Die();
        }
        if (currentHull > hullMax)
        {
            currentHull = hullMax;
        }
    }

    private void Die()
    {
        Debug.Log("Object has died.");
        Destroy(gameObject);
    }
}