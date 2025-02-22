using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WhaleBot : MonoBehaviour

{
    public TextMeshProUGUI whaleText; // Assign in Inspector
    // public TextMeshProUGUI whaleText; // Uncomment if using TextMeshPro

    private void Start()
    {
        string whaleArt = @"
       oooooooooooBlowHOLEoooooooooo
             .'|
           .'  |
         .'    |
       .'______|
       |  __  |
       | |  | |
       | |  | |
       | |__| |
       |______|
         |  |
         |  |
         |  |
         |__|";

        whaleText.text = whaleArt; // Display the whale art
    }
}   
