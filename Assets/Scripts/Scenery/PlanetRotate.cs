using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotate : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private GameObject planetVisual;
    [SerializeField] private float planetRotationSpeedMin;
    [SerializeField] private float planetRotationSpeedMax;
    private float planetRotationSpeed;


    private void Start()
    {       
        // Initialize rotation speed with a random direction (-1 or +1)
        int randomDirection = Random.Range(0, 2) * 2 - 1; // Generates -1 or +1
        planetRotationSpeed = Random.Range(planetRotationSpeedMin, planetRotationSpeedMax) * randomDirection;
    }

    private void Update()
    {
        planetVisual.transform.Rotate(Vector3.forward * planetRotationSpeed * Time.deltaTime, Space.World);
    }
}

