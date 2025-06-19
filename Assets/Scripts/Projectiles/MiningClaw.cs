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
    private Transform miningTarget;

    [Header("Collision Settings")]
    [SerializeField] private float colliderCheckRadius = 0.5f;
    [SerializeField] private bool showDebugGizmos = true;

    private CircleCollider2D clawCollider;
    private void Start()
    {
        Debug.Log("MiningClaw spawned", this);

        // Ensure we have all required components
        clawCollider = GetComponent<CircleCollider2D>();
        if (clawCollider == null)
        {
            clawCollider = gameObject.AddComponent<CircleCollider2D>();
            Debug.Log("Added CircleCollider2D to MiningClaw", this);
        }
        clawCollider.isTrigger = true;
        clawCollider.radius = colliderCheckRadius;

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            Debug.Log("Added Rigidbody2D to MiningClaw", this);
        }
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
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
            // Stay centered on the moving target
            if (miningTarget != null)
            {
                transform.position = miningTarget.position;
            }

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
        if (isReturning || isMining) return;

        Debug.Log($"Claw trigger entered with: {collider.name} (Layer: {collider.gameObject.layer})", collider.gameObject);

        // Debug: Print all components on the collided object
        Debug.Log($"Components on {collider.name}:");
        foreach (Component comp in collider.GetComponents<Component>())
        {
            Debug.Log($"- {comp.GetType()}");
        }

        if (ShouldIgnoreCollision(collider))
        {
            Debug.Log("Ignoring player collision");
            return;
        }

        CreateLoot lootTarget = collider.GetComponentInParent<CreateLoot>();
        if (lootTarget != null)
        {
            Debug.Log($"Mining started with {lootTarget.name} (MiningTime: {lootTarget.MiningTime})");
            StartMining(lootTarget.MiningTime, lootTarget);
            return;
        }

        Debug.Log($"No CreateLoot found on {collider.name}, retracting...");
        Retract();
    }

    private bool ShouldIgnoreCollision(Collider2D collider)
    {
        // Ignore player
        return collider.CompareTag("Player");
    }
    public void StartMining(float miningDuration, CreateLoot lootTarget)
    {
        isMining = true;
        totalMiningTime = miningDuration;
        currentMiningTime = miningDuration;
        currentLootTarget = lootTarget;
        miningTarget = lootTarget.transform; // Store the target transform

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
        miningTarget = null; // Clear the target reference
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

    private void OnDrawGizmos()
    {
        if (showDebugGizmos)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, colliderCheckRadius);

            if (originTransform != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(originTransform.position, transform.position);
            }
        }
    }

}