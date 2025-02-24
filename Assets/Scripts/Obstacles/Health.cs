using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private Obstacle obstacle;

    [SerializeField] private float health;

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
        health -= damage;
        Debug.Log("Health changed. Current health: " + health);

        // Optional: Check if health is below zero and handle death/destruction
        if (health <= 0)
        {
            if (obstacle != null)
            {
                obstacle.Die(true);
            }
        }
    }
}