using System;
using UnityEngine;

public class Cockpit : MonoBehaviour
{
    [SerializeField] private float mass;
    [SerializeField] private float thrust;

    // Define the event
    public static event Action<float> OnCockpitMassChanged;
    public static event Action<float> OnCockpitThrustChanged;


    private void Start()
    {
        // Invoke the event to add mass
        OnCockpitMassChanged?.Invoke(mass);
        // Invoke the event to add thrust
        OnCockpitThrustChanged?.Invoke(thrust);
    }

    private void OnDestroy()
    {
        // Invoke the event to subtract mass
        OnCockpitMassChanged?.Invoke(-mass);
        // Invoke the event to subtract thrust
        OnCockpitThrustChanged?.Invoke(-thrust);
    }
}