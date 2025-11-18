using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Nuke : MonoBehaviour
{
    private SFXManager sFXManager;

    public static event Action <float> NukeDamageEvent;

    [SerializeField] private float selfDestructTime = 2f;
    [SerializeField] private AudioClip nukeSound;
    [SerializeField] private GameObject nukeExplosionParticlePrefab;

    void Start()
    {
        sFXManager = FindAnyObjectByType<SFXManager>();

        NukeObstacles();
        StartCoroutine(SelfDestruct());
        PlayNukeSFX();
    }

    private void NukeObstacles()
    {
        Damage damage = GetComponent<Damage>();
        float damageAmt = damage.GetDamage();
        Instantiate(nukeExplosionParticlePrefab, transform.position, Quaternion.identity);
        NukeDamageEvent?.Invoke(damageAmt);
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(selfDestructTime);
        Destroy(gameObject);
    }

    private void PlayNukeSFX()
    {
        Debug.Log($"Nuke PlayNukeSFX");
        if (sFXManager != null && nukeSound != null)
        {
            sFXManager.PlaySFX(nukeSound);
        }
    }
}
