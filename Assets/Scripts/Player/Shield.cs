using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour
{
    [SerializeField] private Image barImage;
    private float shieldMaxHp;
    private float shieldCurrentHp;
    private float shieldDamage;
    private float shieldRechargeTime;
    private float shieldRechargeCountdown;
    private float shieldRepeatingDamageCountdown;
    private float shieldRepeatingDamageTime = 0.5f;

    // This field tracks obstacles in contact
    private List<GameObject> obstaclesInContact = new List<GameObject>();

    private void Start()
    {
        LoadData();
        shieldRechargeCountdown = shieldRechargeTime;
        shieldCurrentHp = shieldMaxHp;
        shieldRepeatingDamageCountdown = shieldRepeatingDamageTime;
        SetDamage();
        UpdateShieldBar();
    }

    private void Update()
    {
        RepairShip();
        if (obstaclesInContact.Count > 0)
        {
            DamageObstacle();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damage obstacleDamage = collision.gameObject.GetComponent<Damage>();
        Obstacle obstacle = collision.gameObject.GetComponent<Obstacle>();

        if (obstacleDamage != null && obstacle != null)
        {
            // Stop obstacle movement
            ObstacleMovement obstacleMovement = collision.gameObject.GetComponent<ObstacleMovement>();
            if (obstacleMovement != null)
            {
                obstacleMovement.enabled = false;
            }

            // Add to list of obstacles in contact
            if (!obstaclesInContact.Contains(collision.gameObject))
            {
                obstaclesInContact.Add(collision.gameObject);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Damage obstacleDamage = collision.gameObject.GetComponent<Damage>();
        Obstacle obstacle = collision.gameObject.GetComponent<Obstacle>();

        if (obstacleDamage != null && obstacle != null)
        {
            // Resume obstacle movement
            ObstacleMovement obstacleMovement = collision.gameObject.GetComponent<ObstacleMovement>();
            if (obstacleMovement != null)
            {
                obstacleMovement.enabled = true;
            }

            // Remove from list of obstacles in contact
            if (obstaclesInContact.Contains(collision.gameObject))
            {
                obstaclesInContact.Remove(collision.gameObject);
            }
        }
    }

    private void DamageObstacle()
    {
        shieldRepeatingDamageCountdown -= Time.deltaTime;
        if (shieldRepeatingDamageCountdown <= 0f)
        {
            // Deal damage to obstacles and take damage from obstacles
            foreach (GameObject obstacleObj in obstaclesInContact.ToArray())
            {
                if (obstacleObj == null)
                {
                    obstaclesInContact.Remove(obstacleObj);
                    continue;
                }

                Damage obstacleDamage = obstacleObj.GetComponent<Damage>();
                Obstacle obstacle = obstacleObj.GetComponent<Obstacle>();
                Health obstacleHealth = obstacleObj.GetComponent<Health>();

                if (obstacleDamage != null && obstacle != null && obstacleHealth != null)
                {
                    // Deal damage to obstacle
                    obstacleHealth.ChangeHealth(-shieldDamage);

                    // Take damage from obstacle
                    float damageAmt = obstacleDamage.GetDamage();
                    shieldCurrentHp -= damageAmt;

                    UpdateShieldBar();

                    if (shieldCurrentHp <= 0)
                    {
                        shieldCurrentHp = 0;
                        Destroy(gameObject);
                        return;
                    }

                    // Reset recharge timer when taking damage
                    shieldRechargeCountdown = shieldRechargeTime;
                }
            }
            shieldRepeatingDamageCountdown = shieldRepeatingDamageTime;
        }
    }

    private void RepairShip()
    {
        shieldRechargeCountdown -= Time.deltaTime;

        if (shieldRechargeCountdown <= 0f)
        {
            shieldCurrentHp = Mathf.Min(shieldCurrentHp + 1f, shieldMaxHp);
            UpdateShieldBar();
            // Reset timer
            shieldRechargeCountdown = shieldRechargeTime;
        }
    }

    private void UpdateShieldBar()
    {
        if (barImage != null)
        {
            barImage.fillAmount = shieldCurrentHp / shieldMaxHp;
        }
    }

    private void SetDamage()
    {
        Damage damageComponent = GetComponent<Damage>();
        if (damageComponent != null)
        {
            damageComponent.ChangeDamage(shieldDamage);
        }
    }

    private void LoadData()
    {
        if (DataPersister.Instance != null && DataPersister.Instance.CurrentGameData != null)
        {
            shieldMaxHp = DataPersister.Instance.CurrentGameData.shieldHp;
            shieldDamage = DataPersister.Instance.CurrentGameData.shieldDamage;
            shieldRechargeTime = DataPersister.Instance.CurrentGameData.sheildRechargeRate;
        }
    }
}
