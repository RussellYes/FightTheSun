using UnityEngine;
using System.IO;
using System;

public class SimpleAdLogger : MonoBehaviour
{
    public static SimpleAdLogger Instance { get; private set; }
    private string logPath;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        logPath = Path.Combine(Application.persistentDataPath, "ad_debug.log");
        Log("SYSTEM", "Ad Logger Initialized");
    }

    public void Log(string eventType, string data)
    {
        try
        {
            File.AppendAllText(logPath, $"{DateTime.Now:HH:mm:ss} - {eventType}: {data}\n");
        }
        catch (Exception e)
        {
            Debug.LogError($"AdLogger Error: {e.Message}");
        }
    }
}