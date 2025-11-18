using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is currently unused, but was created to generate a line of gameObjects with exponential scaling and offset to give 2D art a 3D depth look.

public class CreatePeopleInALine : MonoBehaviour
{
    [SerializeField] private GameObject personPrefab;
    [SerializeField] private GameObject frontOfTheLinePosition;
    [SerializeField] private int numberOfPeople;
    private float xOffset = -0.12f;
    private float yOffset = -0.1f;
    [SerializeField] private float offsetExponentialFactor; // Controls how fast the offset grows exponentially
    [SerializeField] private float scaleExponentialFactor; // Controls how fast the scale grows exponentially

    // Start is called before the first frame update
    void Start()
    {
        GenerateLineOfPeople();
    }

    private void GenerateLineOfPeople()
    {
        // Create a person at the front of the line, then create a person behind them with an offset and larger scale. Repeat until the line is full.
        for (int i = 0; i < numberOfPeople; i++)
        {
            // Calculate the position offset exponentially
            float exponentialXOffset = xOffset * Mathf.Pow(offsetExponentialFactor, i);
            float exponentialYOffset = yOffset * Mathf.Pow(offsetExponentialFactor, i);
            Vector3 positionOffset = new Vector3(exponentialXOffset, exponentialYOffset, 0);

            // Instantiate the person as a child of THIS GameObject (the one this script is attached to)
            GameObject person = Instantiate(personPrefab, frontOfTheLinePosition.transform.position + positionOffset, Quaternion.identity, this.transform);

            // Scale the person exponentially
            float scale = Mathf.Pow(scaleExponentialFactor, i); // Exponential scaling
            person.transform.localScale = new Vector3(scale, scale, 1); // Apply the scale
        }
    }
}