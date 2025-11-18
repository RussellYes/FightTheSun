using UnityEngine;
using System.Collections;

public class DestroyInSeconds : MonoBehaviour
{
    [SerializeField] private float selfDestructTime = 1.5f;

    private void Start()
    {
        StartCoroutine(DestroyAfterTime());
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSecondsRealtime(selfDestructTime);
        Destroy(gameObject);
    }
}