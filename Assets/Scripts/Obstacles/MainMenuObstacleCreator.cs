using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuObstacleCreator : MonoBehaviour
{
    private MainMenuUI mainMenuUI;

    [SerializeField] private GameObject mainMenuBackground;
    [SerializeField] private GameObject mainMenuObstacle;
    [SerializeField] private float ascendingSpawnTime;
    private float ascendingSpawnCountdown;
    [SerializeField] private float descendingSpawnTime;
    private float descendingSpawnCountdown;

    private void Start()
    {
        mainMenuUI = FindFirstObjectByType<MainMenuUI>();

        ascendingSpawnCountdown = ascendingSpawnTime;
        descendingSpawnCountdown = descendingSpawnTime;
    }

    private void Update()
    {
        AscendingTimer();
        DescendingTimer();
    }

    private void AscendingTimer()
    {
        ascendingSpawnCountdown -= Time.deltaTime;
        if (ascendingSpawnCountdown <= 0)
        {
            SpawnObstacle(true);
            ascendingSpawnCountdown = ascendingSpawnTime;
        }
    }

    private void DescendingTimer()
    {
        descendingSpawnCountdown -= Time.deltaTime;
        if (descendingSpawnCountdown <= 0)
        {
            SpawnObstacle(false);
            descendingSpawnCountdown = descendingSpawnTime;
        }
    }

    private void SpawnObstacle(bool isAscending)
    {
        // X position is always 0, y position depends on ascending/descending
        float yPosition = isAscending ? mainMenuUI.MinYPosition - 1 : mainMenuUI.MaxYPosition + 1;
        float xPosition = Random.Range(-2, 3);
        Vector3 spawnPosition = new Vector3(xPosition, yPosition, 0);

        // Instantiate as child of background
        GameObject newObstacle = Instantiate(
            mainMenuObstacle,
            spawnPosition,
            Quaternion.identity,
            mainMenuBackground.transform);

        // Set the isAscending value on the obstacle component
        MainMenuObstacle obstacleComponent = newObstacle.GetComponent<MainMenuObstacle>();
        if (obstacleComponent != null)
        {
            obstacleComponent.SetAscending(isAscending);
        }
    }
}