using UnityEngine;
using UnityEngine.UI;

public class LootShip : MonoBehaviour
{
    [SerializeField] private SpriteRenderer shipColourStripe;
    [SerializeField] private GameObject dailyLootPrefab;

    private void Start()
    {
        ShipColour();
    }

    private void ShipColour()
    {
        // Load and apply the loot ship colour from saved game data
        shipColourStripe.color = DataPersister.Instance.CurrentGameData.randomLootShipCompanyColour;
    }

    public void CreateLoot()
    {
        // Spawn daily loot when the loot ship is destroyed
        Instantiate(dailyLootPrefab, transform.position, Quaternion.identity);
    }



}
