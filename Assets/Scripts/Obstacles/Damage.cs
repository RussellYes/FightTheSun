using UnityEngine;

// This script defines a Damage class that holds a damage value.

public class Damage : MonoBehaviour
{
    [SerializeField] private float damage;

    public float GetDamage()
    {
        return damage;
    }
}