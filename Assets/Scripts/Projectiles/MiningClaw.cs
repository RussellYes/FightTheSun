using UnityEngine;

public class MiningClaw : MonoBehaviour
{
    private Transform originTransform; 
    private LineRenderer cableLineRenderer;
    private Rigidbody2D rb;
    private bool isReturning = false;
    private float returnSpeed = 5f;
    private float launchSpeed;

    public void Initialize(Transform origin, LineRenderer cableRenderer, float speed)
    {
        originTransform = origin;
        cableLineRenderer = cableRenderer;
        launchSpeed = speed;
        rb = GetComponent<Rigidbody2D>();
        cableLineRenderer.enabled = true;

        // Initial positions
        UpdateCable();
    }

    private void Update()
    {
        UpdateCable();

        if (isReturning)
        {
            ReturnToShip();
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

    public void Retract()
    {
        isReturning = true;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void ReturnToShip()
    {
        if (originTransform == null) return;

        Vector2 direction = (originTransform.position - transform.position).normalized;
        transform.position += (Vector3)direction * returnSpeed * Time.deltaTime;

        if (Vector2.Distance(transform.position, originTransform.position) < 0.5f)
        {
            Destroy(gameObject);
            if (cableLineRenderer != null)
            {
                cableLineRenderer.enabled = false;
            }
        }
    }

    private void OnDestroy()
    {
        if (cableLineRenderer != null)
        {
            cableLineRenderer.enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isReturning)
        {
            Retract();
        }
    }
}