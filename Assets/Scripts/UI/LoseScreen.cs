using UnityEngine;
using UnityEngine.UI;

public class LoseScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject sun;    // UI element with Transform
    [SerializeField] private GameObject aura;   // UI element with Image
    [SerializeField] private float lerpDuration = 2f;

    private Image auraImage;
    private float timer = 0f;
    private bool fadeInComplete = false;
    private bool sunShrunk = false;

    private void Start()
    {
        // Initialize aura to 5% alpha
        if (aura != null)
        {
            auraImage = aura.GetComponent<Image>();
            if (auraImage != null)
            {
                Color c = auraImage.color;
                c.a = 0.05f;
                auraImage.color = c;
            }
        }

        // Initialize sun to full scale
        if (sun != null)
        {
            sun.transform.localScale = Vector3.one;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Handle Sun shrinking (first 5 seconds)
        if (!sunShrunk && sun != null)
        {
            float sunProgress = Mathf.Clamp01(timer / lerpDuration);
            float sunScale = Mathf.Lerp(1f, 0f, sunProgress);
            sun.transform.localScale = new Vector3(sunScale, sunScale, sunScale);

            if (sunProgress >= 1f)
            {
                Destroy(sun);
                sunShrunk = true;
            }
        }

        // Handle Aura fading
        if (auraImage != null)
        {
            if (!fadeInComplete)
            {
                // First 5 seconds: fade from 0.05 to 1
                float fadeProgress = Mathf.Clamp01(timer / lerpDuration);
                float alpha = Mathf.Lerp(0.00f, 1f, fadeProgress);

                Color c = auraImage.color;
                c.a = alpha;
                auraImage.color = c;

                if (fadeProgress >= 1f)
                {
                    fadeInComplete = true;
                    timer = 0f; // Reset timer for fade out
                }
            }
            else
            {
                // Next 5 seconds: fade from 1 to 0
                float fadeProgress = Mathf.Clamp01(timer / lerpDuration);
                float alpha = Mathf.Lerp(1f, 0f, fadeProgress);

                Color c = auraImage.color;
                c.a = alpha;
                auraImage.color = c;
            }
        }
    }
}