using UnityEngine;

public class ObstaclePopUpText : MonoBehaviour
{
    [SerializeField] private GameObject popUpTextPrefab;
    private Color damageNegitiveColour = Color.white;

    public void DamageText(float damageAmt)
    {
        if (popUpTextPrefab)
        {
            GameObject prefab = Instantiate(popUpTextPrefab, transform.position, Quaternion.identity);
            TextMesh textComponent = prefab.GetComponentInChildren<TextMesh>();
            if (!textComponent) return;

            textComponent.text = damageAmt >= 0 ? damageAmt.ToString("F0") : damageAmt.ToString("F0");

            Renderer textRenderer = prefab.GetComponentInChildren<Renderer>();
            if (textRenderer)
            {
                textRenderer.material.color = damageNegitiveColour;
            }
        }
    }
}

