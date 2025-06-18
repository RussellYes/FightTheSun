using System;
using UnityEngine;

public class MiningClawLauncher : MonoBehaviour
{
    public static event Action<float> MiningClawTimeUpdateEvent;

    [SerializeField] private AudioClip miningClawSFX;
    [SerializeField] private LineRenderer miningClawCableLineRenderer;
    [SerializeField] private GameObject miningClawPrefab;
    [SerializeField] private float miningTime = 5f;
    [SerializeField] private Color cableColor;
    private float miningClawSpeed = 5f;
    private float cableWidth = 0.1f;

    private SFXManager sFXManager;
    private float miningCountdown;
    private bool miningClawActive = false;
    private MiningClaw currentClaw;

    private void Start()
    {
        sFXManager = FindFirstObjectByType<SFXManager>();
    }

    private void OnEnable()
    {
        ShipUIManager.LaunchMiningClawEvent += LaunchMiningClaw;
        ShipUIManager.StopMiningClawEvent += StopMiningClaw;
    }

    private void OnDisable()
    {
        ShipUIManager.LaunchMiningClawEvent -= LaunchMiningClaw;
        ShipUIManager.StopMiningClawEvent -= StopMiningClaw;
    }

    private void Update()
    {
        if (miningClawActive)
        {
            miningCountdown -= Time.deltaTime;
            MiningClawTimeUpdateEvent?.Invoke(miningCountdown);

            if (miningCountdown <= 0)
            {
                StopMiningClaw();
            }
        }
    }

    private void LaunchMiningClaw(Vector2 direction)
    {
        // Play sound effect
        sFXManager.PlaySFX(miningClawSFX);

        // Instantiate mining claw
        GameObject claw = Instantiate(miningClawPrefab, transform.position, Quaternion.identity);
        currentClaw = claw.GetComponent<MiningClaw>();

        // Configure cable line renderer
        if (miningClawCableLineRenderer != null)
        {
            miningClawCableLineRenderer.positionCount = 2;
            miningClawCableLineRenderer.SetPosition(0, transform.position); // Start at ship
            miningClawCableLineRenderer.SetPosition(1, transform.position); // Start with same position

            miningClawCableLineRenderer.startWidth = cableWidth;
            miningClawCableLineRenderer.endWidth = cableWidth * 0.66f; // Slightly tapered
            miningClawCableLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            miningClawCableLineRenderer.startColor = cableColor;
            miningClawCableLineRenderer.endColor = cableColor;
            miningClawCableLineRenderer.enabled = true;
        }

        // Initialize claw
        if (currentClaw != null)
        {
            currentClaw.Initialize(transform, miningClawCableLineRenderer, miningClawSpeed);
        }

        // Set velocity in the specified direction
        Rigidbody2D rb = currentClaw.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * miningClawSpeed;
        }

        // Start mining countdown
        miningClawActive = true;
        miningCountdown = miningTime;
    }

    private void StopMiningClaw()
    {
        if (currentClaw != null)
        {
            currentClaw.Retract();
        }
        miningClawActive = false;
        MiningClawTimeUpdateEvent?.Invoke(0f);
    }
}