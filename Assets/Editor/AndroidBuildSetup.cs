using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

public class AndroidBuildSetup
{
    [MenuItem("Tools/Setup Android Build (APK)")]
    public static void Setup()
    {
        // APK 빌드 (Google Play용 AAB 아님)
        EditorUserBuildSettings.buildAppBundle = false;

        // 패키지 이름 — Google Play 등록 시 유일해야 함
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.dh.tetrisgame");

        // 앱 이름 & 버전
        PlayerSettings.productName = "TetrisGame";
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.Android.bundleVersionCode = 1;

        // API 레벨
        // 최소 Android 6.0 (API 23) — 시장 점유율 99%+ 커버
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
        // 타겟은 최신 (Unity가 권장하는 기본값 사용)
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;

        // 세로 고정
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.allowedAutorotateToPortrait = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = false;

        // 스크립팅 백엔드: IL2CPP (Android 12+ 64비트 요구사항 대응)
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);

        // ARM 아키텍처: ARM64만 (최신 기기 64비트 전용)
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

        Debug.Log("Android 빌드 세팅 완료! (APK / IL2CPP / ARM64)");
        EditorUtility.DisplayDialog("완료", "Android 빌드 세팅이 적용되었습니다.\n\nFile > Build Settings에서 Android 플랫폼으로 Switch 후 Build 하세요.", "확인");
    }
}
