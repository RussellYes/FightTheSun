using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Scroller from Tarodev https://www.youtube.com/watch?v=-6H-uYh80vc
/// </summary>

public class Scroller : MonoBehaviour
{
    private PlayerStatsManager playerStatsManager;
    [SerializeField] private float backgroundSpeedMultiplier; // Speed multiplier for the background

    private void Start()
    {
        playerStatsManager = FindAnyObjectByType<PlayerStatsManager>();
    }

    [SerializeField] private RawImage _img;
    private float _x, _y;

    private void Update()
    {
        _y = (playerStatsManager.PlayerThrust / (playerStatsManager.PlayerMass + 0.1f)) * backgroundSpeedMultiplier;
        _x = 0;
        _img.uvRect = new Rect(_img.uvRect.position + new Vector2(_x, _y) * Time.deltaTime, _img.uvRect.size);
    }
}
