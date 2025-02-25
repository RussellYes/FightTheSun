using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {

        StartCoroutine(SelfDestruct());
    }

    IEnumerator SelfDestruct()
    {
        Debug.Log("bullet hit");
        yield return new WaitForSeconds(0.01f);
        Destroy(gameObject);
    }
}