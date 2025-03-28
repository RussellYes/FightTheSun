using UnityEngine;
using System.Collections;

public class MiningMissile : MonoBehaviour
{
    private SFXManager sFXManager;
    private Rigidbody2D rb;
    private float accelerationTimer = 0f;
    private bool hasAccelerated = false;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem collisionParticles;
    [SerializeField] private ParticleSystem flightParticles;

    [Header("Audio")]
    [SerializeField] private AudioClip[] miningMissleStandardImpactSounds;
    [SerializeField] private AudioClip[] miningMissleAsteroidImpactSounds;

    [Header("Timing")]
    [SerializeField] private float countdown;
    [SerializeField] private float accelerationDelay;
    [SerializeField] private float startSpeedPercent;
    [SerializeField] private float smokeTime;
    private float smokeCountdown;

    private void Start()
    {
        sFXManager = FindObjectOfType<SFXManager>();
        rb = GetComponent<Rigidbody2D>();
        smokeCountdown = smokeTime;

        // Apply initial slow speed (10%)
        rb.velocity = rb.velocity * startSpeedPercent;
    }

    private void Update()
    {
        TimedSelfDestruct();
        HandleAcceleration();
        FlightParticles();
    }

    private void HandleAcceleration()
    {
        if (!hasAccelerated && accelerationTimer < accelerationDelay)
        {
            accelerationTimer += Time.deltaTime;

            if (accelerationTimer >= accelerationDelay)
            {
                rb.velocity = rb.velocity / startSpeedPercent;
                hasAccelerated = true;
                Debug.Log("Missile accelerated to full speed!", this);
            }
        }
    }

    private void FlightParticles()
    {
        smokeCountdown -= Time.deltaTime;

        if (smokeCountdown <= 0)
        {
            if (flightParticles != null)
            {
                Instantiate(flightParticles, transform.position, Quaternion.identity);
            }
            smokeCountdown = smokeTime;
        }
    }

    private void TimedSelfDestruct()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            Debug.Log("Missile collided with an obstacle!", this);
            StartCoroutine(SelfDestruct());
        }
    }

    private IEnumerator SelfDestruct()
    {
        Debug.Log("Missile self destructing");
        // Play effects immediately
        PlayImpactSound();

        if (collisionParticles != null)
        {
            ParticleSystem particles = Instantiate(collisionParticles, transform.position, Quaternion.identity);
            StartCoroutine(FollowParentMovement(particles.transform));
        }

        // Wait one frame to ensure effects play
        yield return null;

        Destroy(gameObject);
    }

    private void PlayImpactSound()
    {
        if (sFXManager != null && miningMissleStandardImpactSounds.Length > 0)
        {
            AudioClip clip = miningMissleStandardImpactSounds[Random.Range(0, miningMissleStandardImpactSounds.Length)];
            sFXManager.PlaySFX(clip);
        }
    }

    private IEnumerator FollowParentMovement(Transform particlesTransform)
    {
        Vector3 offset = particlesTransform.position - transform.position;
        while (particlesTransform != null)
        {
            particlesTransform.position = transform.position + offset;
            yield return null;
        }
    }
}