#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

[InitializeOnLoad]
public static class FlexibleUpdateValidator
{
    private const string AAR_PATH = "Assets/Plugins/Android/play-core-1.10.3.aar";

    static FlexibleUpdateValidator()
    {
        EditorApplication.delayCall += ValidateSetup;
    }

    private static void ValidateSetup()
    {
        CheckAARFile();
        CheckScriptingBackend();
    }

    private static void CheckAARFile()
    {
        if (!File.Exists(AAR_PATH))
        {
            Debug.LogWarning(
                $"[FlexibleUpdateManager] ❌ Missing required AAR file:\n" +
                $"Expected: `{AAR_PATH}`\n" +
                $"Download from: https://maven.google.com/web/index.html#com.google.android.play:core:1.10.3\n" +
                $"Then place it in `Assets/Plugins/Android/`"
            );
        }
        else
        {
            Debug.Log($"[FlexibleUpdateManager] ✅ `play-core-1.10.3.aar` found. Update system ready.");
        }
    }

    private static void CheckScriptingBackend()
    {
        var namedTarget = NamedBuildTarget.FromBuildTargetGroup(BuildTargetGroup.Android);
        var backend = PlayerSettings.GetScriptingBackend(namedTarget);

        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android && backend != ScriptingImplementation.IL2CPP)
        {
            Debug.LogWarning(
                $"[FlexibleUpdateManager] ⚠️ Scripting backend is set to *Mono*, but IL2CPP is recommended and required for Play Core features to work properly on Android.\n" +
                $"Go to: Edit > Project Settings > Player > Android > Other Settings > Scripting Backend"
            );
        }
    }
}
#endif
