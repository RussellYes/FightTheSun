using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Bullet : MonoBehaviour
{
    private SFXManager sFXManager;

    [SerializeField] private ParticleSystem collisionParticles;
    [SerializeField] private AudioClip[] bulletImpactSounds;

    private float countdown = 5f;

    private void Start()
    {
        sFXManager = FindAnyObjectByType<SFXManager>();
    }

    private void Update()
    {
        TimedSelfDestruct();
    }

    private void TimedSelfDestruct()
    {
        if (countdown > 0)
        {
            countdown -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object is the WorldLowerBarrier
        if (collision.CompareTag("WorldLowerBarrier"))
        {
            SelfDestruct();

            return;
        }
        StartCoroutine(SelfDestruct());
    }


    IEnumerator SelfDestruct()
    {
        Debug.Log("bullet hit");
        if (collisionParticles != null)
        {
            //Play bulletImpact SFX
            if (sFXManager != null && bulletImpactSounds.Length > 0)
            {
                sFXManager.PlaySFX(bulletImpactSounds[UnityEngine.Random.Range(0, bulletImpactSounds.Length)]);
            }

            ParticleSystem particles = Instantiate(collisionParticles, transform.position, Quaternion.identity);

            //coroutine to make the particles follow the obstacle's movement
            StartCoroutine(FollowParentMovement(particles.transform));
        }
        yield return new WaitForSeconds(0.01f);
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