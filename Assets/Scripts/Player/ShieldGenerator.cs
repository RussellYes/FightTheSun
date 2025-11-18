using Unity.VisualScripting;
using UnityEngine;

public class ShieldGenerator : MonoBehaviour
{
    [SerializeField] private GameObject shieldPrefab;

    private void OnEnable()
    {
        ShipUIManager.CreateShieldEvent += GenerateShield;
    }

    private void OnDisable()
    {
        ShipUIManager.CreateShieldEvent -= GenerateShield;
    }

    private void GenerateShield()
    {
        Instantiate(shieldPrefab, transform.position, Quaternion.identity, transform);
    }

}
