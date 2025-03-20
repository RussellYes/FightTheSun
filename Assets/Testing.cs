using System;
using System.Collections;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public static event Action StartGameCountdownEvent;

    /*
    private void Start()
    {
        StartGameCountdownEvent?.Invoke();
    }*/


    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f); // Wait for one frame to ensure all scripts are enabled and subscribed
        StartGameCountdownEvent?.Invoke();
    }
}