using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// This script moves a planet visual when a level is completed.

public class EndPlanet : MonoBehaviour
{
    [Header("Movement")]
    private float movementSpeed = 2;
    private PlanetEndPosition planetEndPosition;
    private Vector3 startPosition; // Store the starting position of the planet
    private float journeyLength; // Total distance between start and end positions
    private float startTime; 
    private bool isMoving = true;

    [Header("Visuals")]
    [SerializeField] private GameObject planetVisual;
    [SerializeField] private float planetRotationSpeedMin;
    [SerializeField] private float planetRotationSpeedMax;
    private float planetRotationSpeed;
    

    private void Start()
    {
        planetEndPosition = FindAnyObjectByType<PlanetEndPosition>();

        // Initialize rotation speed with a random direction (-1 or +1)
        int randomDirection = Random.Range(0, 2) * 2 - 1; // Generates -1 or +1
        planetRotationSpeed = Random.Range(planetRotationSpeedMin, planetRotationSpeedMax) * randomDirection;

        // Store the starting position of the planet
        startPosition = transform.position;

        // Calculate the total distance between the start and end positions
        journeyLength = Vector3.Distance(startPosition, planetEndPosition.transform.position);

        // Record the start time
        startTime = Time.time;
    }

    private void Update()
    {
        planetVisual.transform.Rotate(Vector3.forward * planetRotationSpeed * Time.deltaTime, Space.World);

        MovePlanet();
    }

    private void MovePlanet()
    {
        if (isMoving)
        {
            // Calculate the fraction of the journey completed
            float distanceCovered = (Time.time - startTime) * movementSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            // Smoothly interpolate between the start and end positions
            transform.position = Vector3.Lerp(startPosition, planetEndPosition.transform.position, fractionOfJourney);

            // Optional: Stop moving once the planet reaches the end position
            if (fractionOfJourney >= 1f)
            {
                // Disable further movment
                isMoving = false;
            }
        }
        
    }
}
