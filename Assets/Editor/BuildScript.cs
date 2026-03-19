using UnityEditor;
using UnityEngine;

public class BuildScript
{
    [MenuItem("Tools/Build Android APK")]
    public static void BuildAndroid()
    {
        string outputPath = "tetris_game.apk";

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/Game.unity" },
            locationPathName = outputPath,
            target = BuildTarget.Android,
            options = EditorUserBuildSettings.development
                ? BuildOptions.Development
                : BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            Debug.Log($"빌드 성공! 경로: {outputPath}");
        else
            Debug.LogError($"빌드 실패: {report.summary.result}");
    }
}
