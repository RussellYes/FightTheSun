using UnityEngine;

public class MiningClawLauncher : MonoBehaviour
{
    [SerializeField] private AudioClip miningClawSFX;
    [SerializeField] private LineRenderer miningClawCableLineRenderer;
    [SerializeField] private GameObject miningClawPrefab;
    [SerializeField] private Color cableColor;
    [SerializeField] private float miningClawSpeed = 5f;
    [SerializeField] private float cableWidth = 0.1f;

    private SFXManager sFXManager;
    private MiningClaw currentClaw;


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

    private void Start()
    {
        sFXManager = FindFirstObjectByType<SFXManager>();
    }

    private void LaunchMiningClaw(Vector2 direction)
    {
        if (currentClaw != null) return;

        if (sFXManager == null)
        {
            sFXManager = FindFirstObjectByType<SFXManager>();
        }
        if (miningClawSFX != null || sFXManager != null)
        {
            sFXManager.PlaySFX(miningClawSFX);
        }

        GameObject claw = Instantiate(miningClawPrefab, transform.position, Quaternion.identity);
        currentClaw = claw.GetComponent<MiningClaw>();

        InitializeCable();
        currentClaw.Initialize(transform, miningClawCableLineRenderer, miningClawSpeed);

        Rigidbody2D rb = currentClaw.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; 
            rb.linearVelocity = direction * miningClawSpeed; 
        }

        currentClaw.OnDestroyed += HandleClawDestroyed;
    }

    private void HandleClawDestroyed()
    {
        miningClawCableLineRenderer.enabled = false;
        currentClaw = null;
    }

    private void InitializeCable()
    {
        if (miningClawCableLineRenderer == null) return;

        miningClawCableLineRenderer.positionCount = 2;
        miningClawCableLineRenderer.SetPositions(new Vector3[] { transform.position, transform.position });
        miningClawCableLineRenderer.startWidth = cableWidth;
        miningClawCableLineRenderer.endWidth = cableWidth * 0.66f;
        miningClawCableLineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = cableColor };
        miningClawCableLineRenderer.enabled = true;
    }

    private void StopMiningClaw()
    {
        if (currentClaw != null)
        {
            currentClaw.Retract();
        }
    }


}