using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script ensures a 1 frame delay for the loading scene to render before calling the LoaderCallback method.

public class LoaderCallback : MonoBehaviour
{


    private bool isFirstUpdate = true;

    private void Update()
    {
        
        if (isFirstUpdate)
        {
            isFirstUpdate = false;

            Loader.LoaderCallback();
        }
        
    }
}
