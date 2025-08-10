using UnityEngine;
using com.Google.Play.AppUpdate;

public class UpdateScene : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log("UpdateScene OnEnable");
        FlexibleUpdateManager.OnUpdateProcessComplete += LoadNextScene;
        FlexibleUpdateManager.TestingWithoutUpdateManagerEvent += LoadNextScene;
    }

    private void OnDisable()
    {
        FlexibleUpdateManager.OnUpdateProcessComplete -= LoadNextScene;
        FlexibleUpdateManager.TestingWithoutUpdateManagerEvent -= LoadNextScene;
    }

    private void LoadNextScene()
    {
        Debug.Log("UpdateScene LoadNextScene - Loading MainMenuScene");
        Loader.Load(Loader.Scene.MainMenuScene);
    }
}