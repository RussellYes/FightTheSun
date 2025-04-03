using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script detroys the particle system after a countdown.

public class DestroyParticles : MonoBehaviour
{
    private float countdown = 5f;

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;

        if (countdown <= 0)
        {
            Destroy(gameObject);
        }
    }
}
