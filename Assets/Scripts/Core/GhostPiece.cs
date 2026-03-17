using UnityEngine;

/// <summary>
/// 현재 활성 피스가 하드드롭 시 착지할 위치를 반투명 회색 블록으로 미리 보여줌
/// GameManager가 피스 스폰 시 함께 생성하고, 피스 잠금 시 파괴함
/// </summary>
public class GhostPiece : MonoBehaviour
{
    private Board _board;
    private Piece _activePiece;

    // 고스트 블록 색상: 반투명 회색
    private static readonly Color GhostColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);

    private GameObject[] _blocks;
    private SpriteRenderer[] _renderers;

    public void Initialize(Board board, Piece activePiece, Sprite sprite, Vector2Int[] cells)
    {
        _board = board;
        _activePiece = activePiece;

        _blocks = new GameObject[cells.Length];
        _renderers = new SpriteRenderer[cells.Length];

        for (int i = 0; i < cells.Length; i++)
        {
            var blockObj = new GameObject("GhostBlock");
            blockObj.transform.SetParent(transform);

            var sr = blockObj.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = GhostColor;
            sr.sortingOrder = 0; // 실제 블록(1)보다 뒤에 표시

            blockObj.transform.localPosition = new Vector3(cells[i].x, cells[i].y, 0);

            _blocks[i] = blockObj;
            _renderers[i] = sr;
        }
    }

    private void LateUpdate()
    {
        if (_activePiece == null)
        {
            // 활성 피스가 파괴되면 고스트도 숨김
            gameObject.SetActive(false);
            return;
        }

        UpdateGhostPosition();
    }

    /// <summary>
    /// 활성 피스의 현재 셀 배열을 고스트 블록에 반영 (회전 시 모양 동기화)
    /// </summary>
    public void SyncCells(Vector2Int[] cells)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            _blocks[i].transform.localPosition = new Vector3(cells[i].x, cells[i].y, 0);
        }
    }

    private void UpdateGhostPosition()
    {
        // 활성 피스 위치에서 아래로 한 칸씩 내려가며 유효한 최저 위치를 찾음
        Vector2Int ghostPos = _activePiece.Position;

        while (IsValidPosition(ghostPos + Vector2Int.down))
        {
            ghostPos += Vector2Int.down;
        }

        transform.position = _board.BoardToWorld(ghostPos);
    }

    private bool IsValidPosition(Vector2Int pivot)
    {
        // 고스트 블록의 localPosition에서 셀 좌표를 역산
        for (int i = 0; i < _blocks.Length; i++)
        {
            var local = _blocks[i].transform.localPosition;
            var cell = new Vector2Int(Mathf.RoundToInt(local.x), Mathf.RoundToInt(local.y));
            if (!_board.IsValidPosition(pivot + cell))
                return false;
        }
        return true;
    }
}
