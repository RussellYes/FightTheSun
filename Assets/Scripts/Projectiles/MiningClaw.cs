using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class MiningClaw : MonoBehaviour
{
    SFXManager SFXManager => SFXManager.Instance;
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
    private Loot carriedLoot;

    [Header("Collision Settings")]
    [SerializeField] private float colliderCheckRadius = 0.5f;
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private AudioClip[] miningClawGrabSFX;

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
        UpdateRotation();

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

    private void UpdateRotation()
    {
        if (originTransform != null)
        {
            // Calculate direction from claw to origin (launcher)
            Vector2 directionToOrigin = originTransform.position - transform.position;

            // Flip the direction to face away from origin
            Vector2 directionAwayFromOrigin = -directionToOrigin;

            // Calculate the angle in degrees
            float angle = Mathf.Atan2(directionAwayFromOrigin.y, directionAwayFromOrigin.x) * Mathf.Rad2Deg + 90;

            // Apply the rotation
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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

        if (collider.gameObject.layer == LayerMask.NameToLayer("WorldBarrier"))
        {
            Debug.Log($"Claw collided: {name}");
            Retract();
            return;
        }

        Loot loot = collider.GetComponent<Loot>();
        if (loot != null)
        {
            Debug.Log($"Claw grabbed loot: {loot.name}");
            GrabLoot(loot);
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
        if (collider.CompareTag("Player"))
            return true;

        // Ignore solar flares
        if (collider.CompareTag("SolarFlare"))
            return true;

        return false;
    }
    public void StartMining(float miningDuration, CreateLoot lootTarget)
    {
        isMining = true;
        SFXManager.PlaySFX(miningClawGrabSFX[UnityEngine.Random.Range(0, miningClawGrabSFX.Length)]);
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
            // Spawn loot (this version doesn't return anything)
            currentLootTarget.SpawnLoot();

            // Try to find any Loot objects near the mining target to grab
            Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, 1f);
            foreach (Collider2D col in nearbyColliders)
            {
                Loot loot = col.GetComponent<Loot>();
                if (loot != null && loot.transform.parent == null) // Only grab loot that isn't already parented
                {
                    GrabLoot(loot);
                    break; // Just grab the first one found
                }
            }

            // Handle obstacle destruction
            Obstacle obstacle = currentLootTarget.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                obstacle.Die(true);
            }
            else
            {
                Destroy(currentLootTarget.gameObject);
            }

            miningTarget = null;
            Retract();
        }
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

    private void GrabLoot(Loot loot)
    {
        carriedLoot = loot;

        // Parent the loot to the claw
        loot.transform.SetParent(transform);

        // Disable loot's collider to prevent further collisions
        Collider2D lootCollider = loot.GetComponent<Collider2D>();
        if (lootCollider != null) lootCollider.enabled = false;

        // Start returning to ship
        Retract();
    }

    private void ReturnToShip()
    {
        if (originTransform == null) return;

        Vector2 direction = (originTransform.position - transform.position).normalized;
        transform.position += (Vector3)direction * returnSpeed * Time.deltaTime;

        if (Vector2.Distance(transform.position, originTransform.position) < 0.5f)
        {
            // Drop any carried loot
            if (carriedLoot != null)
            {
                carriedLoot.transform.SetParent(null);
                carriedLoot.transform.position = originTransform.position;

                // Re-enable collider if it exists
                Collider2D lootCollider = carriedLoot.GetComponent<Collider2D>();
                if (lootCollider != null) lootCollider.enabled = true;

                carriedLoot = null;
            }

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