using UnityEditor;
using UnityEngine;

// 메뉴 Tools > Setup Build Scenes 를 실행하면
// MainMenu(index 0) → Game(index 1) 순서로 빌드 씬을 등록합니다.
public class BuildSettingsSetup
{
    [MenuItem("Tools/Setup Build Scenes")]
    public static void SetupBuildScenes()
    {
        var scenes = new[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Game.unity",     true),
        };

        EditorBuildSettings.scenes = scenes;
        Debug.Log("Build Scenes 등록 완료: MainMenu(0), Game(1)");
    }
}
