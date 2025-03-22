using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Shoot : MonoBehaviour
{
    [SerializeField] private bool isStraightShooter;
    [SerializeField] private bool canTargetPlayer;

    [SerializeField] private float shotRange;

    [SerializeField] private float reloadTime = 1f;
    private float reloadCountdownTime;

    [SerializeField] private GameObject bulletPreFab;
    [SerializeField] private float bulletSpeed = 10f;

    private Transform target;

    private void Start()
    {
        Debug.Log("Shoot Start");
        reloadCountdownTime = reloadTime;
    }

    private void Update()
    {
        Debug.Log("Shoot Update");
        GetTarget();
        
        if (isStraightShooter)
        {
            ShootStraight();
            return;
        }

        if (canTargetPlayer && target != null)
        {
            Debug.Log("Shoot Update - Shoot at target");
            ShootAtTarget();
        }
    }

    private void GetTarget()
    {
        // Use Physics2D.OverlapCircle to detect the player within a circular range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, shotRange);
        Debug.Log("Shoot GetTarget");


        foreach (Collider2D hit in hits)
        {
            // Check if the hit object has a Cockpit component
            Cockpit player = hit.GetComponent<Cockpit>();
            if (player != null)
            {
                target = player.transform;
                Debug.Log("Shoot GetTarget - Player targeted");
                return; // Exit after finding the player
            }
        }

        // If no player is found, set target to null
        target = null;
        Debug.Log("Shoot GetTarget - No player found");
    }

    private void ShootAtTarget()
    {
        if (reloadCountdownTime > 0)
        {
            reloadCountdownTime -= Time.deltaTime;
        }
        else
        {
            reloadCountdownTime = reloadTime;

            // Calculate the direction to the target
            Vector2 direction = (target.position - transform.position).normalized;

            // Instantiate the bullet and shoot it towards the target
            GameObject bullet = Instantiate(bulletPreFab, transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
        }
    }

    private void ShootStraight()
    {
        if (reloadCountdownTime > 0)
        {
            reloadCountdownTime -= Time.deltaTime;
        }
        else
        {
            reloadCountdownTime = reloadTime;
            GameObject bullet = Instantiate(bulletPreFab, transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a red circle gizmo to show the target area
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shotRange);
    }
}