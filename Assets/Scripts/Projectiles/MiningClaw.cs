using UnityEngine;

public class MiningClaw : MonoBehaviour
{
    private Vector2 originPoint;
    private LineRenderer cableLineRenderer;
    private Rigidbody2D rb;
    private bool isReturning = false;
    private float returnSpeed = 15f;

    [SerializeField] private float maxDistance = 20f;

    public void Initialize(Vector2 origin, LineRenderer cableRenderer)
    {
        originPoint = origin;
        cableLineRenderer = cableRenderer;
        rb = GetComponent<Rigidbody2D>();
        cableLineRenderer.enabled = true;
    }

    private void Update()
    {
        UpdateCable();
        CheckDistance();

        if (isReturning)
        {
            ReturnToShip();
        }
    }

    private void UpdateCable()
    {
        if (cableLineRenderer != null)
        {
            cableLineRenderer.SetPosition(0, originPoint);
            cableLineRenderer.SetPosition(1, transform.position);
        }
    }

    private void CheckDistance()
    {
        float distance = Vector2.Distance(originPoint, transform.position);
        if (distance > maxDistance && !isReturning)
        {
            isReturning = true;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void ReturnToShip()
    {
        Vector2 direction = (originPoint - (Vector2)transform.position).normalized;
        transform.position += (Vector3)direction * returnSpeed * Time.deltaTime;

        if (Vector2.Distance(transform.position, originPoint) < 0.5f)
        {
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