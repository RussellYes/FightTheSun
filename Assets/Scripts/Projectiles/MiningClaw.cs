using Unity.VisualScripting;
using UnityEngine;

public class MiningClaw : MonoBehaviour
{
    public class OnClawTimerChangedEventArgs : System.EventArgs
    {
        public float progressNormalized;
    }
    public static event System.EventHandler<OnClawTimerChangedEventArgs> OnClawTimerChanged;
    public event System.Action OnDestroyed;

    private Transform originTransform;
    private LineRenderer cableLineRenderer;
    private Rigidbody2D rb;
    private bool isReturning = false;
    private bool isMining = false;
    private float returnSpeed = 5f;
    private float launchSpeed;
    private float currentMiningTime;
    private float totalMiningTime;
    private CreateLoot currentLootTarget;
    private float maxFlightTime = 2f;
    private float flightTimer;

    private void Start()
    {
        Debug.Log("MiningClaw spawned", this);

        if (TryGetComponent<Rigidbody2D>(out var rigid))
        {
            rigid.bodyType = RigidbodyType2D.Dynamic;
            rigid.gravityScale = 0;
            rigid.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    public void Initialize(Transform origin, LineRenderer cableRenderer, float speed)
    {
        originTransform = origin;
        cableLineRenderer = cableRenderer;
        launchSpeed = speed;
        rb = GetComponent<Rigidbody2D>();
        cableLineRenderer.enabled = true;
        flightTimer = maxFlightTime;
        UpdateCable();
    }

    private void Update()
    {
        UpdateCable();

        if (!isMining && !isReturning)
        {
            flightTimer -= Time.deltaTime;
            if (flightTimer <= 0)
            {
                Retract();
                return;
            }
        }

        if (isMining)
        {
            currentMiningTime -= Time.deltaTime;
            OnClawTimerChanged?.Invoke(this, new OnClawTimerChangedEventArgs
            {
                progressNormalized = currentMiningTime / totalMiningTime
            });

            if (currentMiningTime <= 0)
            {
                CompleteMining();
            }
        }
        else if (isReturning)
        {
            ReturnToShip();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (isReturning || isMining || collider.CompareTag("Player")) return;

        Debug.Log($"Claw hit: {collider.name}", collider.gameObject);

        CreateLoot lootTarget = collider.GetComponent<CreateLoot>();
        if (lootTarget != null)
        {
            StartMining(lootTarget.MiningTime, lootTarget);
            return;
        }

        // Hit anything else? Then retract
        Retract();
    }

    public void StartMining(float miningDuration, CreateLoot lootTarget)
    {
        isMining = true;
        totalMiningTime = miningDuration;
        currentMiningTime = miningDuration;
        currentLootTarget = lootTarget;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void CompleteMining()
    {
        if (currentLootTarget != null)
        {
            currentLootTarget.SpawnLoot();
            Destroy(currentLootTarget.gameObject);
        }
        Retract();
    }

    public void Retract()
    {
        isMining = false;
        isReturning = true;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }





    private void UpdateCable()
    {
        if (cableLineRenderer != null && originTransform != null)
        {
            // Get a new position from the origin transform
            cableLineRenderer.SetPosition(0, originTransform.position);
            cableLineRenderer.SetPosition(1, transform.position);
        }
    }


    private void ReturnToShip()
    {
        if (originTransform == null) return;

        Vector2 direction = (originTransform.position - transform.position).normalized;
        transform.position += (Vector3)direction * returnSpeed * Time.deltaTime;

        if (Vector2.Distance(transform.position, originTransform.position) < 0.5f)
        {
            OnDestroyed?.Invoke();
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (cableLineRenderer != null)
        {
            cableLineRenderer.enabled = false;
        }
    }


}