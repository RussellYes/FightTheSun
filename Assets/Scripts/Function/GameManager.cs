using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void ProjectileAction();
    public static event ProjectileAction StopProjectiles;
    public static event ProjectileAction StartProjectiles;

    public delegate void SpawningAction();
    public static event SpawningAction StopSpawning;
    public static event SpawningAction StartSpawning;

    [SerializeField] private float goal; // Distance to the goal

    // Public property to access the goal
    public float Goal => goal;

    public void StartSpawningTrigger()
    {
        StartSpawning?.Invoke();
    }

    public void StopSpawningTrigger()
    {
        StopSpawning?.Invoke();
    }
    
    private void StartProjectilesTrigger()
    {
        StartProjectiles?.Invoke();
    }

    private void StopProjectilesTrigger()
    {
        StopProjectiles?.Invoke();
    }

}