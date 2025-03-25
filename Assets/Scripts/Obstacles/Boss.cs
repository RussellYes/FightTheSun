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
    private Vector3 startPosition;
    private float journeyLength;
    private float startTime;
    private bool isMovingAtStart = true;

    private bool isAttackTrigger = false;
    private bool isDefenceTrigger = false;
    private bool isAttacking = false; // New flag to prevent overlapping attacks

    [Header("Attack")]
    [SerializeField] private float attackTime = 10f;
    [SerializeField] private float defenceTime = 6f;
    [SerializeField] private int attackCount;
    [SerializeField] private int attackSequence = 0;
    [SerializeField] private GameObject turret1;
    [SerializeField] private GameObject turret2;

    [Header("Destruction")]
    [SerializeField] private AudioClip destructionSound;
    [SerializeField] private ParticleSystem destructionParticles;

    private void Start()
    {
        planetEndPosition = FindAnyObjectByType<PlanetEndPosition>();
        startPosition = transform.position;
        journeyLength = Vector3.Distance(startPosition, planetEndPosition.transform.position);
        startTime = Time.time;

        if (turret1) turret1.SetActive(false);
        if (turret2) turret2.SetActive(false);
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

        if (!isAttacking) // Only check battle mode if not currently attacking
        {
            BattleMode();
        }
    }

    private void MovePlanetAtStart()
    {
        if (isMovingAtStart)
        {
            float distanceCovered = (Time.time - startTime) * startingMovementSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, planetEndPosition.transform.position, fractionOfJourney);

            if (fractionOfJourney >= 1f)
            {
                isMovingAtStart = false;
                isAttackTrigger = true;
            }
        }
    }

    private void BattleMode()
    {
        if (isBoss1)
        {
            if (!isMovingAtStart && isAttackTrigger)
            {
                StartCoroutine(Attack1());
            }
            if (!isMovingAtStart && isDefenceTrigger)
            {
                StartCoroutine(Defend());
            }
        }

        if (isBoss2 && !isMovingAtStart)
        {
            if (isDefenceTrigger)
            {
                Debug.Log("Boss2: Starting Defence");
                StartCoroutine(Defend());
            }
            else if (isAttackTrigger && !isAttacking)
            {
                switch (attackSequence)
                {
                    case 0:
                        StartCoroutine(ExecuteAttackSequence(Attack2()));
                        break;
                    case 1:
                        StartCoroutine(ExecuteAttackSequence(Attack3()));
                        break;
                    case 2:
                        StartCoroutine(ExecuteAttackSequence(Attack4()));
                        break;
                    case 3:
                        StartCoroutine(ExecuteAttackSequence(Attack5()));
                        break;
                }
            }
        }
    }

    // New wrapper coroutine to manage attack sequence state
    IEnumerator ExecuteAttackSequence(IEnumerator attackCoroutine)
    {
        isAttacking = true;
        yield return StartCoroutine(attackCoroutine);
        isAttacking = false;
    }

    IEnumerator Attack1()
    {
        isAttackTrigger = false;
        Debug.Log("Boss1: Attack1 Started");
        StartSpawnersEvent?.Invoke();
        yield return new WaitForSeconds(attackTime);
        isDefenceTrigger = true;
        Debug.Log("Boss1: Attack1 Complete");
    }

    IEnumerator Attack2()
    {
        isAttackTrigger = false;
        Debug.Log("Boss2: Attack2 Started");

        attackCount = 2;
        for (int i = 0; i < attackCount; i++)
        {
            SpawnEventGroup1?.Invoke();
            yield return new WaitForSeconds(0.5f);
            SpawnEventGroup3?.Invoke();
            yield return new WaitForSeconds(3f);
            SpawnEventGroup2?.Invoke();
            yield return new WaitForSeconds(0.5f);
            SpawnEventGroup4?.Invoke();
        }
        yield return new WaitForSeconds(attackTime);

        attackSequence = 1; // Next will be Attack3
        isAttackTrigger = true;
        Debug.Log("Boss2: Attack2 Complete");
    }

    IEnumerator Attack3()
    {
        isAttackTrigger = false;
        Debug.Log("Boss2: Attack3 Started");

        attackCount = 2;
        for (int i = 0; i < attackCount; i++)
        {
            SpawnEventGroup5?.Invoke();
            yield return new WaitForSeconds(1f);
            SpawnEventGroup5?.Invoke();
        }
        yield return new WaitForSeconds(attackTime);

        attackSequence = 2; // Next will be Attack4
        isAttackTrigger = true;
        Debug.Log("Boss2: Attack3 Complete");
    }

    IEnumerator Attack4()
    {
        isAttackTrigger = false;
        Debug.Log("Boss2: Attack4 Started");
        attackCount = 2;
        for (int i = 0; i < attackCount; i++)
        {
            if (turret1) turret1.SetActive(true);
            if (turret2) turret2.SetActive(true);
            yield return new WaitForSeconds(6);
            if (turret1) turret1.SetActive(false);
            if (turret2) turret2.SetActive(false);
        }
        yield return new WaitForSeconds(attackTime);

        attackSequence = 3; // Next will be Attack5
        isAttackTrigger = true;
        Debug.Log("Boss2: Attack4 Complete");
    }

    IEnumerator Attack5()
    {
        isAttackTrigger = false;
        Debug.Log("Boss2: Attack5 Started");

        attackCount = 2;
        int rollTheDice = UnityEngine.Random.Range(0, 3);

        for (int i = 0; i < attackCount; i++)
        {
            yield return new WaitForSeconds(1f);
            if (rollTheDice == 0)
            {
                SpawnEventGroup1?.Invoke();
            }
            else if (rollTheDice == 1)
            {
                SpawnEventGroup2?.Invoke();
            }
            else
            {
                SpawnEventGroup5?.Invoke();
            }
        }
        yield return new WaitForSeconds(attackTime);

        attackSequence = 0; // Reset sequence for next cycle
        isDefenceTrigger = true; // Next will be Defence
        Debug.Log("Boss2: Attack5 Complete");
    }

    IEnumerator Defend()
    {
        isDefenceTrigger = false;
        Debug.Log("Boss: Defence Started");
        StopSpawnersEvent?.Invoke();
        yield return new WaitForSeconds(defenceTime);

        attackSequence = 0;
        isAttackTrigger = true;
        Debug.Log("Boss: Defence Complete");
    }

    private void Destruction()
    {
        if (destructionSound != null)
        {
            sFXManager = FindAnyObjectByType<SFXManager>();
            if (sFXManager != null)
            {
                sFXManager.PlaySFX(destructionSound);
            }
        }

        if (destructionParticles != null)
        {
            ParticleSystem particles = Instantiate(destructionParticles, transform.position, Quaternion.identity);
            StartCoroutine(FollowParentMovement(particles.transform));
        }

        GameManager.Instance.SetState(GameManager.GameState.Playing);
        Debug.Log("Boss: Destroyed");
        Destroy(gameObject);
    }

    private IEnumerator FollowParentMovement(Transform particlesTransform)
    {
        Vector3 offset = particlesTransform.position - transform.position;
        while (this != null)
        {
            particlesTransform.position = transform.position + offset;
            yield return null;
        }
    }
}