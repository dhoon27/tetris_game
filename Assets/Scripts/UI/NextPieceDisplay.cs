using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Canvas UI 기반 다음 블록 미리보기.
/// SafeAreaPanel 아래의 컨테이너에 부착하여 사용.
/// </summary>
public class NextPieceDisplay : MonoBehaviour
{
    [SerializeField] private float cellSize = 45f; // UI 셀 크기(픽셀)

    private Image[] _blocks;

    private void Start()
    {
        // UI Image 블록 4개 생성
        _blocks = new Image[4];
        for (int i = 0; i < 4; i++)
        {
            var go = new GameObject("PreviewBlock_" + i);
            go.transform.SetParent(transform, false);

            var img = go.AddComponent<Image>();
            img.raycastTarget = false;

            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(cellSize, cellSize);

            go.SetActive(false);
            _blocks[i] = img;
        }

        GameManager.Instance.OnNextPieceChanged += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnNextPieceChanged -= Refresh;
    }

    private void Refresh()
    {
        var data = GameManager.Instance.NextData;
        if (data == null) return;

        // cells의 중심을 (0,0)으로 맞추기 위해 평균 오프셋 계산
        Vector2 center = Vector2.zero;
        foreach (var cell in data.cells)
            center += new Vector2(cell.x, cell.y);
        center /= data.cells.Length;

        for (int i = 0; i < _blocks.Length; i++)
        {
            if (i < data.cells.Length)
            {
                var cell = data.cells[i];
                _blocks[i].sprite = data.sprite;
                _blocks[i].gameObject.SetActive(true);

                var rt = _blocks[i].GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(
                    (cell.x - center.x) * cellSize,
                    (cell.y - center.y) * cellSize);
            }
            else
            {
                _blocks[i].gameObject.SetActive(false);
            }
        }
    }
}
