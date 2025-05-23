using UnityEngine;

// This script defines a Damage class that holds a damage value.

public class Damage : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private bool solarFlare2;
    [SerializeField] private bool solarFlare3;
    public float GetDamage()
    {
        if (solarFlare2)
        {
            return damage * 2;
        }
        if (solarFlare3)
        {
            return damage * 4;
        }
        else 
        {
            return damage;
        }
    }

    public void ChangeDamage(float amt)
    {
        damage += amt;
    }
}