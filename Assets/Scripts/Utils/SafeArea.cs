using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    private RectTransform _rect;
    private Rect _lastSafeArea;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        Refresh();
    }

    private void Update()
    {
        if (_lastSafeArea != Screen.safeArea)
            Refresh();
    }

    private void Refresh()
    {
        _lastSafeArea = Screen.safeArea;

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 anchorMin = _lastSafeArea.position / screenSize;
        Vector2 anchorMax = (_lastSafeArea.position + _lastSafeArea.size) / screenSize;

        _rect.anchorMin = anchorMin;
        _rect.anchorMax = anchorMax;
        _rect.offsetMin = Vector2.zero;
        _rect.offsetMax = Vector2.zero;
    }
}
