using UnityEngine;

// This script stores and changes a health value for an object.

public class Health : MonoBehaviour
{
    [SerializeField] private Obstacle obstacle;
    [SerializeField] private float health;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object has a Damage component
        Damage damageComponent = collision.gameObject.GetComponent<Damage>();

        if (damageComponent != null)
        {
            // If yes, then subtract the damage from health
            ChangeHealth(damageComponent.GetDamage());
        }
    }

    public void ChangeHealth(float damage)
    {
        health -= damage;
        Debug.Log("Health changed. Current health: " + health);

        // Trigger pop-up text on obstacle
        ObstaclePopUpText popUpText = GetComponent<ObstaclePopUpText>();
        if (popUpText != null)
        {
            popUpText.DamageText(damage);
        }

        // Check if health is below zero and handle death/destruction
        if (health <= 0)
        {
            if (obstacle != null)
            {
                obstacle.Die(true);
            }
        }
    }
}