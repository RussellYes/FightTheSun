using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hull : MonoBehaviour
{


    private float currentHull;
    [SerializeField] private float hullMax;

    private void Start()
    {
        currentHull = hullMax;
    }

    private void OnTriggerEnter2D(Collider2D collision) // Add Collider2D parameter
    {
        // Check if the colliding object has a Damage component
        Damage damageComponent = collision.gameObject.GetComponent<Damage>();

        if (damageComponent != null)
        {
            // If it does, subtract the damage from health
            ChangeHealth(damageComponent.GetDamage());
        }
    }

    private void ChangeHealth(float damage)
    {
        currentHull -= damage;
        Debug.Log("Hull changed. Current hull: " + currentHull);

        //Check if hull is below zero and handle death/destruction
        if (currentHull <= 0)
        {
            Die();
        }
        //A max limit for hull
        if (currentHull > hullMax)
        {
            currentHull = hullMax;
        }
    }

    private void Die()
    {
        Debug.Log("Object has died.");
        // Handle death (e.g., destroy the object, play an animation, etc.)
        Destroy(gameObject);
    }
}
