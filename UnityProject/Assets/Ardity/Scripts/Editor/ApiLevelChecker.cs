using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ApiLevelChecker
{
    static ApiLevelChecker()
    {
        CheckLevel();
    }

    public static void CheckLevel()
    {
        var currentApi = PlayerSettings.GetApiCompatibilityLevel(UnityEditor.Build.NamedBuildTarget.Standalone);
        var targetApi = ApiCompatibilityLevel.NET_Unity_4_8;

        int unityVersion = int.Parse(Application.unityVersion.Substring(0, 4));
        if (unityVersion <= 2018)
            targetApi = ApiCompatibilityLevel.NET_2_0;

        if (currentApi == targetApi) return;

        string title = "API Compatibility Level not supported";
        string message = $"Current API Compatibility Level ({currentApi}) is not supported for Ardity.\n" +
            $"Use .Net 4.x (or .Net 2.0 for Unity 2018 or earlier).\n" +
            $"Would you like to change it to {targetApi}?";
        string noButton = "I will change it later";

        if (EditorUtility.DisplayDialog(title, message, "Yes", noButton))
        {
            PlayerSettings.SetApiCompatibilityLevel(UnityEditor.Build.NamedBuildTarget.Standalone, targetApi);
            Debug.Log("API Compatibility Level changed to " + targetApi);
        }
    }
}
