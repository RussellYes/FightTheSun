using UnityEngine;

public class LevelDailyLootShipGlow : MonoBehaviour
{
    [SerializeField] private int levelNumber;
    [SerializeField] private GameObject glowObject;

    private void Update()
    {
        if (levelNumber == DataPersister.Instance.CurrentGameData.randomLootShipLevel)
        {
            glowObject.SetActive(true);
        }
        else
        {
            glowObject.SetActive(false);
        }
    }
}
