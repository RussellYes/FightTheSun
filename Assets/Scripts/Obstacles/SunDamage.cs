using UnityEngine;

public class SunDamage : MonoBehaviour
{
    private float sunDamageTime = 3f;
    private float sunDamageCountdown;
    private GameObject heatWave;

    private void Start()
    {
        sunDamageCountdown = sunDamageTime;
    }

    private void Update()
    {
        if (sunDamageCountdown > 0)
        {
            heatWave.SetActive(false);
            sunDamageCountdown -= Time.deltaTime;
        }
        else
        {
            heatWave.SetActive(true);
            sunDamageCountdown = sunDamageTime; // Reset the countdown
        }
    }
}
