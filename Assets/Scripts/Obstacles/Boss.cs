using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public static event Action StartSpawnersEvent;
    public static event Action StopSpawnersEvent;
    public static event Action SpawnEventGroup1;
    public static event Action SpawnEventGroup2;
    public static event Action SpawnEventGroup3;
    public static event Action SpawnEventGroup4;
    public static event Action SpawnEventGroup5;

    private SFXManager sFXManager;

    [Header("Boss Type")]
    [SerializeField] private bool isBoss1;
    [SerializeField] private bool isBoss2;


    [Header("Movement")]
    private float startingMovementSpeed = 2;
    private PlanetEndPosition planetEndPosition;
    private Vector3 startPosition; // Store the starting position of the planet
    private float journeyLength; // Total distance between start and end positions
    private float startTime;
    private bool isMovingAtStart = true;

    private bool isAttacking = false;
    private bool isDefending= false;

    [Header("Attack")]
    [SerializeField] private float attackTime = 6f;
    [SerializeField] private float defenceTime = 6f;
    [SerializeField] private int attackCount;
    [SerializeField] private int attackSequence = 0;

    [Header("Destruction")]
    [SerializeField] private AudioClip destructionSound;
    [SerializeField] private ParticleSystem destructionParticles;


    private void Start()
    {
        planetEndPosition = FindAnyObjectByType<PlanetEndPosition>();

        // Store the starting position of the planet
        startPosition = transform.position;

        // Calculate the total distance between the start and end positions
        journeyLength = Vector3.Distance(startPosition, planetEndPosition.transform.position);

        // Record the start time
        startTime = Time.time;
    }

    private void OnEnable()
    {
        Obstacle.BossDefeatedEvent += Destruction;
    }

    private void OnDisable()
    {
        Obstacle.BossDefeatedEvent -= Destruction;
    }

    private void Update()
    {
        MovePlanetAtStart();

        BattleMode();
    }

    private void MovePlanetAtStart()
    {
        if (isMovingAtStart)
        {
            // Calculate the fraction of the journey completed
            float distanceCovered = (Time.time - startTime) * startingMovementSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            // Smoothly interpolate between the start and end positions
            transform.position = Vector3.Lerp(startPosition, planetEndPosition.transform.position, fractionOfJourney);

            // Optional: Stop moving once the planet reaches the end position
            if (fractionOfJourney >= 1f)
            {
                // Disable further movment
                isMovingAtStart = false;
                isAttacking = true;
            }
        }

    }

    private void BattleMode()
    {
        if (isBoss1)
        {
            if (!isMovingAtStart && isAttacking)
            {
                StartCoroutine(Attack1());
            }
            if (!isMovingAtStart && isDefending)
            {
                StartCoroutine(Defend());
            }
        }

        if (isBoss2)
        {
            if (!isMovingAtStart && isAttacking)
            {
                if (attackTime <= 0)
                {
                    StartCoroutine(Defend());
                    attackSequence = 0;
                }
                if (attackSequence == 0)
                {
                    attackSequence++;
                    StartCoroutine(Attack2());
                }
                if (attackSequence == 1)
                {
                    attackSequence++;
                    StartCoroutine(Attack3());
                }
                if (attackSequence == 2)
                {
                    attackSequence++;
                    StartCoroutine(Attack4());
                }
                if (attackSequence == 3)
                {
                    attackSequence++;
                    StartCoroutine(Attack2());
                }
                if (attackSequence >= 4)
                {
                    attackSequence++;
                    int rollTheDice = UnityEngine.Random.Range(0, 3);
                    if(rollTheDice == 0)
                    {
                        StartCoroutine(Attack2());
                    }
                    if(rollTheDice == 1)
                    {
                        StartCoroutine(Attack3());
                    }
                    if(rollTheDice == 2)
                    {
                        StartCoroutine(Attack4());
                    }
                }

            }
        }

    }

    IEnumerator Attack1()
    {
        isAttacking = false;
        Debug.Log("Boss Attack1");
        // Code for boss attack
        StartSpawnersEvent?.Invoke();
        yield return new WaitForSeconds(attackTime);
        isDefending = true;
    }

    IEnumerator Attack2()
    {
        isAttacking = false;
        Debug.Log("Boss Attack2");

        attackCount = 3;
        for (int i = 0; i < attackCount; i++)
        {
            //Event to spawn enemies
            SpawnEventGroup1?.Invoke();
            yield return new WaitForSeconds(0.1f);
            SpawnEventGroup3?.Invoke();
            yield return new WaitForSeconds(0.4f);
            SpawnEventGroup2?.Invoke();
            yield return new WaitForSeconds(0.1f);
            SpawnEventGroup4?.Invoke();
        }
        yield return new WaitForSeconds(attackTime);
        isAttacking = true;
        if (isBoss2)
        {
            attackTime--;
        }
    }

    IEnumerator Attack3()
    {
        isAttacking = false;
        Debug.Log("Boss Attack3");

        attackCount = 2;
        for (int i = 0; i < attackCount; i++)
        {
            //Event to spawn enemies
            SpawnEventGroup5?.Invoke();
            yield return new WaitForSeconds(0.3f);
            SpawnEventGroup5?.Invoke();
        }
        yield return new WaitForSeconds(attackTime);
        isAttacking = true;
        if (isBoss2)
        {
            attackTime--;
        }
    }

    IEnumerator Attack4()
    {
        isAttacking = false;
        Debug.Log("Boss Attack4");

        attackCount = 8;

        int rollTheDice = UnityEngine.Random.Range(0, 3);
        for (int i = 0; i < attackCount; i++)
        {
            if (rollTheDice == 0)
            {
                SpawnEventGroup1?.Invoke();
            }
            if (rollTheDice == 1)
            {
                SpawnEventGroup2?.Invoke();
            }
            if (rollTheDice == 2)
            {
                SpawnEventGroup5?.Invoke();
            }

            yield return new WaitForSeconds(attackTime);
            isAttacking = true;
            if (isBoss2)
            {
                attackTime--;
            }
        }
    }

    IEnumerator Defend()
    {
        isDefending = false;
        Debug.Log("Boss1 Defend");
        // Code for boss defense
        StopSpawnersEvent?.Invoke();
        yield return new WaitForSeconds(defenceTime);
        isAttacking = true;
    }

    private void Destruction()
    {
        // SFX
        if (destructionSound != null)
        {
            sFXManager = FindAnyObjectByType<SFXManager>();
            if (sFXManager != null)
            {
                sFXManager.PlaySFX(destructionSound);
            }
        }
        // Instantiate particles
        if (destructionParticles != null)
        {
            ParticleSystem particles = Instantiate(destructionParticles, transform.position, Quaternion.identity);

            //coroutine to make the particles follow the obstacle's movement
            StartCoroutine(FollowParentMovement(particles.transform));
        }
        // Trigger play
        GameManager.Instance.SetState(GameManager.GameState.Playing);
        // Destroy boss
        Debug.Log("Boss1: Destroyed");
        Destroy(gameObject);
    }

    private System.Collections.IEnumerator FollowParentMovement(Transform particlesTransform)
    {
        // Store the initial offset between the obstacle and the particles
        Vector3 offset = particlesTransform.position - transform.position;

        // Update the particles' position every frame while the parent exists
        while (this != null) // Check if the parent still exists
        {
            // Update the particles' position to match the parent's position plus the offset
            particlesTransform.position = transform.position + offset;

            // Wait for the next frame
            yield return null;
        }
    }



}

