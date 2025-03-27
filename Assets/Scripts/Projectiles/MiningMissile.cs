using UnityEngine;
using System.Collections; // Required for IEnumerator

public class MiningMissile : MonoBehaviour
{
    private SFXManager sFXManager;
    private Rigidbody2D rb;
    private float accelerationTimer = 0f;
    private float initialSpeed;
    private float targetSpeed;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem collisionParticles;

    [Header("Audio")]
    [SerializeField] private AudioClip[] miningMissleStandardImpactSounds;
    [SerializeField] private AudioClip[] miningMissleAsteroidImpactSounds;

    [Header("Timing")]
    [SerializeField] private float countdown = 20f;
    [SerializeField] private float accelerationDuration = 1f;

    private void Start()
    {
        sFXManager = FindObjectOfType<SFXManager>();
        rb = GetComponent<Rigidbody2D>();

        // Initialize speed parameters
        targetSpeed = rb.velocity.magnitude;
        initialSpeed = targetSpeed * 0.25f;
        rb.velocity = rb.velocity.normalized * initialSpeed;
    }

    private void Update()
    {
        TimedSelfDestruct();
        AccelerateMissile();
    }

    private void AccelerateMissile()
    {
        if (accelerationTimer < accelerationDuration)
        {
            accelerationTimer += Time.deltaTime;
            float currentSpeed = Mathf.Lerp(initialSpeed, targetSpeed, accelerationTimer / accelerationDuration);
            rb.velocity = rb.velocity.normalized * currentSpeed;
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
            StartCoroutine(SelfDestruct());
        }
    }

    private IEnumerator SelfDestruct()
    {
        // Play impact effects
        if (collisionParticles != null)
        {
            PlayImpactSound();
            ParticleSystem particles = Instantiate(collisionParticles, transform.position, Quaternion.identity);
            yield return StartCoroutine(FollowParentMovement(particles.transform));
        }

        yield return new WaitForSeconds(0.01f);
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
        while (this != null && particlesTransform != null)
        {
            particlesTransform.position = transform.position + offset;
            yield return null;
        }
    }
}