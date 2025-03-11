using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{

    [SerializeField] private Button playMission1Button;
    [SerializeField] private Button playMission2Button;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        playMission1Button.onClick.AddListener(() =>{
            Loader.Load(Loader.Scene.MissionAlphaScene);
            Debug.Log("Loading Scene");
        });  
        
        playMission2Button.onClick.AddListener(() =>{
            Loader.Load(Loader.Scene.MissionBravoScene);
            Debug.Log("Loading Scene");
        });        
        
        quitButton.onClick.AddListener(() =>{
            Debug.Log("Quit");
            Application.Quit();
        });

        Time.timeScale = 1f;
    }

}
