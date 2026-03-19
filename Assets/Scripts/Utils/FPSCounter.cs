using UnityEngine;

/// <summary>
/// 화면 좌측 하단에 FPS를 표시하는 디버그용 카운터
/// Debug.isDebugBuild가 true일 때만 동작 (에디터 + Development Build)
/// 릴리스 빌드에서는 자동으로 비활성화됨
/// </summary>
public class FPSCounter : MonoBehaviour
{
    private float _fps;
    private float _updateInterval = 0.5f;
    private float _timer;
    private int _frames;

    private void Awake()
    {
        if (!Debug.isDebugBuild)
        {
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        _timer += Time.unscaledDeltaTime;
        _frames++;

        if (_timer >= _updateInterval)
        {
            _fps = _frames / _timer;
            _timer = 0f;
            _frames = 0;
        }
    }

    private void OnGUI()
    {
        int size = Mathf.RoundToInt(Screen.height * 0.025f);
        size = Mathf.Max(size, 14);

        var style = new GUIStyle();
        style.fontSize = size;
        style.fontStyle = FontStyle.Bold;

        // FPS에 따라 색상 변경: 50+ 초록, 30+ 노랑, 30미만 빨강
        if (_fps >= 50f)
            style.normal.textColor = Color.green;
        else if (_fps >= 30f)
            style.normal.textColor = Color.yellow;
        else
            style.normal.textColor = Color.red;

        float x = 10f;
        float y = Screen.height - size - 10f;
        GUI.Label(new Rect(x, y, 200, size + 4), $"FPS: {_fps:F0}", style);
    }
}
