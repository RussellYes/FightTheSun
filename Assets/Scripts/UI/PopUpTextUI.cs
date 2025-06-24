using TMPro;
using UnityEngine;

public class PopUpTextUI : MonoBehaviour
{
    [SerializeField] private GameObject popUpTextPrefab;

    [SerializeField] private Color damagePositiveColour;
    [SerializeField] private Color damageNegitiveColour;
    [SerializeField] private Color moneyColour;
    [SerializeField] private Color metalColour;
    [SerializeField] private Color rareMetalColour;
    [SerializeField] private Color missileColour;

    private void OnEnable()
    {
        Hull.PopUpTextEvent += DamageText;
        Obstacle.ObstacleExitsSceneEvent += MoneyText;
        Loot.PlayerGainsLootEvent += LootText;
        ObstacleMovement.MissilePickupEvent += MissileText;
    }

    private void OnDisable()
    {
        Hull.PopUpTextEvent -= DamageText;
        Obstacle.ObstacleExitsSceneEvent -= MoneyText;
        Loot.PlayerGainsLootEvent -= LootText;
        ObstacleMovement.MissilePickupEvent -= MissileText;
    }
    private void DamageText(float damageAmt)
    {
        if (popUpTextPrefab)
        {
            GameObject prefab = Instantiate(popUpTextPrefab, transform.position, Quaternion.identity);
            TextMesh textComponent = prefab.GetComponentInChildren<TextMesh>();
            if (!textComponent) return;

            textComponent.text = damageAmt >= 0 ? damageAmt.ToString("F1") : damageAmt.ToString("F1");

            Renderer textRenderer = prefab.GetComponentInChildren<Renderer>();
            if (textRenderer)
            {
                textRenderer.material.color = damageAmt >= 0 ? damagePositiveColour : damageNegitiveColour;
            }
        }
    }

    private void MoneyText(bool isKilledByPlayer, int moneyAmt)
    {
        if (!isKilledByPlayer) return;

        if (popUpTextPrefab)
        {
            GameObject prefab = Instantiate(popUpTextPrefab, transform.position, Quaternion.identity);
            TextMesh textComponent = prefab.GetComponentInChildren<TextMesh>();
            if (!textComponent) return;

            textComponent.text = moneyAmt.ToString();

            Renderer textRenderer = prefab.GetComponentInChildren<Renderer>();
            if (textRenderer)
            {
                textRenderer.material.color = moneyColour;
            }
        }
    }

    private void LootText(float metalAmt, float rareMetalAmt)
    {
    Debug.Log($"PopUpTextUI LootText metalAmt: {metalAmt}, rareMetalAmt: {rareMetalAmt}");

        if (metalAmt <= 0 && rareMetalAmt <= 0) return;

        GameObject prefab = Instantiate(popUpTextPrefab, transform.position, Quaternion.identity);
        TextMesh textComponent = prefab.GetComponentInChildren<TextMesh>();
        if (!textComponent) return;

        textComponent.text = metalAmt >= rareMetalAmt ? metalAmt.ToString("F1") : rareMetalAmt.ToString("F1");

        Renderer textRenderer = prefab.GetComponentInChildren<Renderer>();
        if (textRenderer)
        {
            textRenderer.material.color = metalAmt >= rareMetalAmt ? metalColour : rareMetalColour;
        }
    }

    private void MissileText(int missileAmt)
    {
        GameObject prefab = Instantiate(popUpTextPrefab, transform.position, Quaternion.identity);
        TextMesh textComponent = prefab.GetComponentInChildren<TextMesh>();
        if (!textComponent) return;

        textComponent.text = missileAmt.ToString();

        Renderer textRenderer = prefab.GetComponentInChildren<Renderer>();
        if (textRenderer)
        {
            textRenderer.material.color = missileColour;
        }
    }

}
