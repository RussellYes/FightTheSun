using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ImageGradientSpin : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image achievementBackingImage;

    [Header("Gradient Settings")]
    [SerializeField] private Color achievementColour1 = Color.red;
    [SerializeField] private Color achievementColour2 = Color.blue;
    [SerializeField] private float colorCycleSpeed = 4f;

    private Material gradientMaterial;

    private void Awake()
    {
        // Load the shader
        Shader gradientShader = Shader.Find("Custom/RadialGradientWithMask");
        if (gradientShader == null)
        {
            Debug.LogError("Shader not found! Make sure it's named correctly.");
            return;
        }

        // Create material with the shader
        gradientMaterial = new Material(gradientShader)
        {
            mainTexture = achievementBackingImage.mainTexture
        };

        achievementBackingImage.material = gradientMaterial;
        achievementBackingImage.material.SetFloat("_RotationSpeed", colorCycleSpeed);
        UpdateGradientColors();
    }

    private void UpdateGradientColors()
    {
        if (gradientMaterial != null)
        {
            gradientMaterial.SetColor("_Color1", achievementColour1);
            gradientMaterial.SetColor("_Color2", achievementColour2);
        }
    }



}
