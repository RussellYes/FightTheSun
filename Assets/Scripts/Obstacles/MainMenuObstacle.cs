using UnityEngine;
using UnityEngine.UI;

public class MainMenuObstacle : MonoBehaviour
{
    [SerializeField] private bool isAscending; // True for ascending, false for descending
    [SerializeField] private float movementAngle;
    [SerializeField] private float movementAngleVariation;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float movementSpeedVariation;
    private float rotationDirection;
    private float rotationSpeed;
    [SerializeField] private float timeUntilDestruction;
    [SerializeField] private Image mainMenuObstacleImage;
    [SerializeField] private Sprite[] ascendingObstacleSprites;
    [SerializeField] private Sprite[] descendingObstacleSprites;

    public void SetAscending(bool ascending)
    {
        isAscending = ascending;
    }
    private void Start()
    {
        AssignSprite();
        AssignSpeed();
        AssignAngle();
        AssignRotation();
    }
    private void Update()
    {
        SelfDestructTimer();
        MoveObstacle();
        RotateObstacle();
    }

    private void AssignSprite()
    {
        if (isAscending)
        {
            // Ascending obstacle
            mainMenuObstacleImage.sprite = ascendingObstacleSprites[Random.Range(0, ascendingObstacleSprites.Length)];
        }
        if (!isAscending)
        {
            // Descending obstacle
            mainMenuObstacleImage.sprite = descendingObstacleSprites[Random.Range(0, descendingObstacleSprites.Length)];
        }
    }

    private void AssignSpeed()
    {
        movementSpeed = Random.Range(movementSpeed - movementSpeedVariation, movementSpeed + movementSpeedVariation);
    }
    private void AssignAngle()
    {
        float baseAngle = isAscending ? 90f : 270f; // 90° for ascending, 270° for descending
        movementAngle = baseAngle + Random.Range(-movementAngleVariation, movementAngleVariation);
    }

    private void AssignRotation()
    {
        if (!isAscending)
        {
            // Randomize rotation direction clockwise or counterclockwise
            rotationDirection = Random.value < 0.5f ? -1f : 1f;
            rotationSpeed = Random.Range(10f, 30f); // Random rotation speed
        }
    }
    private void MoveObstacle()
    {
        Vector3 direction = new Vector3(
            Mathf.Cos(movementAngle * Mathf.Deg2Rad),
            Mathf.Sin(movementAngle * Mathf.Deg2Rad),
            0);
        transform.Translate(direction * movementSpeed * Time.deltaTime, Space.World);
    }

    private void RotateObstacle()
    {
        if (rotationSpeed != 0)
        {
            transform.Rotate(0, 0, rotationDirection * rotationSpeed * Time.deltaTime);
        }
    }
    private void SelfDestructTimer()
    {
        timeUntilDestruction -= Time.deltaTime;
        if (timeUntilDestruction <= 0)
        {
            Destroy(gameObject);
        }
    }
}
