using UnityEngine;

public class Piece : MonoBehaviour
{
    private Board _board;
    private TetrominoData _data;

    public Vector2Int Position { get; private set; }

    // 현재 회전 상태의 셀 좌표 (Initialize 시 data.cells를 복사, 회전할 때마다 갱신)
    private Vector2Int[] _cells;

    private float _stepTimer;
    public float StepDelay = 1f;

    // Wall Kick 시도 오프셋 목록
    // 회전 후 겹칠 때 순서대로 시도해서 유효한 위치를 찾음
    private static readonly Vector2Int[] WallKickOffsets =
    {
        new Vector2Int( 0,  0), // 원래 위치
        new Vector2Int(-1,  0), // 왼쪽으로 1칸
        new Vector2Int( 1,  0), // 오른쪽으로 1칸
        new Vector2Int(-2,  0), // 왼쪽으로 2칸 (I피스 벽 근처)
        new Vector2Int( 2,  0), // 오른쪽으로 2칸
        new Vector2Int( 0,  1), // 위로 1칸 (천장 근처)
    };

    public void Initialize(Board board, TetrominoData data, Vector2Int spawnPos)
    {
        _board = board;
        _data = data;
        Position = spawnPos;

        // data.cells를 복사해 독립적인 회전 상태로 관리
        _cells = (Vector2Int[])data.cells.Clone();

        foreach (var cell in _cells)
        {
            var blockObj = new GameObject("Block");
            blockObj.transform.SetParent(transform);

            var sr = blockObj.AddComponent<SpriteRenderer>();
            sr.sprite = data.sprite;
            sr.sortingOrder = 1;

            blockObj.transform.localPosition = new Vector3(cell.x, cell.y, 0);
        }

        UpdateWorldPosition();
    }

    private void Update()
    {
        _stepTimer += Time.deltaTime;
        if (_stepTimer >= StepDelay)
        {
            _stepTimer = 0f;
            MoveDown();
        }
    }

    private void MoveDown()
    {
        if (!TryMove(Vector2Int.down))
            LockPiece();
    }

    public bool TryMove(Vector2Int direction)
    {
        Vector2Int newPos = Position + direction;
        if (IsValidPosition(newPos, _cells))
        {
            Position = newPos;
            UpdateWorldPosition();
            return true;
        }
        return false;
    }

    // 90도 시계 방향 회전
    // 회전 공식: (x, y) -> (y, -x)
    // Wall Kick으로 여러 위치를 시도해 하나라도 유효하면 회전 적용
    public void TryRotate()
    {
        var rotated = new Vector2Int[_cells.Length];
        for (int i = 0; i < _cells.Length; i++)
            rotated[i] = new Vector2Int(_cells[i].y, -_cells[i].x);

        foreach (var kick in WallKickOffsets)
        {
            Vector2Int kickedPos = Position + kick;
            if (IsValidPosition(kickedPos, rotated))
            {
                _cells = rotated;
                Position = kickedPos;
                UpdateWorldPosition();
                ApplyCellsToBlocks();
                return;
            }
        }
        // 모든 kick 실패 → 회전 취소
    }

    // 하드 드롭: 더 이상 내려갈 수 없을 때까지 즉시 낙하
    public void HardDrop()
    {
        while (TryMove(Vector2Int.down)) { }
        LockPiece();
    }

    private void LockPiece()
    {
        _board.PlacePiece(transform);
        int linesCleared = _board.ClearLines();
        GameManager.Instance.OnPieceLocked(linesCleared);
        Destroy(gameObject);
    }

    // 주어진 pivot 위치와 셀 배열로 유효성 검사
    private bool IsValidPosition(Vector2Int pivot, Vector2Int[] cells)
    {
        foreach (var cell in cells)
        {
            if (!_board.IsValidPosition(pivot + cell))
                return false;
        }
        return true;
    }

    // 회전 후 자식 블록들의 localPosition을 _cells에 맞게 갱신
    private void ApplyCellsToBlocks()
    {
        int i = 0;
        foreach (Transform block in transform)
        {
            block.localPosition = new Vector3(_cells[i].x, _cells[i].y, 0);
            i++;
        }
    }

    private void UpdateWorldPosition()
    {
        transform.position = _board.BoardToWorld(Position);
    }
}
