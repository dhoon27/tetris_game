using UnityEngine;

// 보드 우측 상단에 다음 블록을 SpriteRenderer로 미리 보여주는 컴포넌트
public class NextPieceDisplay : MonoBehaviour
{
    // 미리보기에 사용할 블록 4개 (Init에서 자동 생성)
    private SpriteRenderer[] _blocks;

    // Unlit 머티리얼 (Board/Piece와 동일)
    private Material _unlitMat;

    private void Start()
    {
        _unlitMat = Resources.Load<Material>("Sprite-Unlit-Default")
                    ?? new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default"));

        // 블록 4개 미리 생성 (비활성화 상태)
        _blocks = new SpriteRenderer[4];
        for (int i = 0; i < 4; i++)
        {
            var go = new GameObject("PreviewBlock_" + i);
            go.transform.SetParent(transform);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 2;
            sr.material = _unlitMat;
            go.SetActive(false);
            _blocks[i] = sr;
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
                // 중심 기준으로 위치 설정
                _blocks[i].transform.localPosition = new Vector3(
                    cell.x - center.x,
                    cell.y - center.y,
                    0f);
            }
            else
            {
                _blocks[i].gameObject.SetActive(false);
            }
        }
    }
}
