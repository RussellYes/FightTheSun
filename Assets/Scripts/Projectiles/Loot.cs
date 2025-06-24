using System;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public static event Action <float, float> PlayerGainsLootEvent;

    PlayerStatsManager playerStatsManager;

    private float metal;
    private float rareMetal;

    [SerializeField] private Sprite metalImage;
    [SerializeField] private Sprite rareMetalImage;
    [SerializeField] private SpriteRenderer lootImageRenderer;

    // Start is called before the first frame update
    void Start()
    {
        playerStatsManager = FindFirstObjectByType<PlayerStatsManager>();

        SetLootValues();


    }

    private void SetLootValues()
    {
        int lootCount = 1;
        Boss boss = FindFirstObjectByType<Boss>();
        if (boss != null)
        {
            lootCount = 5;
        }

        for (int i = 0; i < lootCount; i++)
        {
            float rollTheDice = UnityEngine.Random.Range(0, 101) + playerStatsManager.MiningSkill;
            if (rollTheDice > 95)
            {
                rareMetal = playerStatsManager.MiningSkill / UnityEngine.Random.Range(1f, 11f);
                lootImageRenderer.sprite = rareMetalImage;
            }
            else if (rollTheDice <= 95)
            {
                metal = playerStatsManager.MiningSkill / UnityEngine.Random.Range(1f, 6f);
                lootImageRenderer.sprite = metalImage;
            }
        }
    }


    private void OnDestroy()
    {
        PlayerGainsLootEvent?.Invoke((float)metal, (float)rareMetal);
    }
}
