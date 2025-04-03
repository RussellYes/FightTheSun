using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// This script switches between scenes in the game after loading is complete.

public static class Loader
{
    public enum Scene
    {
        MainMenuScene,
        LoadingScene,
        Mission1Scene,
        Mission2Scene,
        Mission3Scene,
        Mission4Scene,
        Mission5Scene,
        Mission6Scene,
        Mission7Scene,
        Mission8Scene,
        Mission9Scene,
        Mission10Scene,
    }

    private static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());

    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }

}
