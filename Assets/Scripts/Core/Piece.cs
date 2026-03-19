using UnityEngine;

public class Piece : MonoBehaviour
{
    private Board _board;
    private TetrominoData _data;

    public Vector2Int Position { get; private set; }

    // 현재 회전 상태의 셀 좌표 (Initialize 시 data.cells를 복사, 회전할 때마다 갱신)
    private Vector2Int[] _cells;

    // 고스트 피스가 셀 배열을 읽을 수 있도록 공개
    public Vector2Int[] Cells => _cells;

    // 고스트 피스 참조 (회전 시 셀 동기화용)
    private GhostPiece _ghost;

    private float _stepTimer;
    public float StepDelay = 1f;

    // Lock Delay: 착지 후 바로 잠기지 않고 일정 시간 여유를 줌
    // 이동/회전하면 타이머 리셋 (최대 횟수 제한으로 무한 방지)
    private const float LockDelay = 0.5f;
    private const int MaxLockResets = 15;
    private float _lockTimer;
    private int _lockResetCount;
    private bool _isLanding; // 바닥에 닿아있는 상태

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
        if (_isLanding)
        {
            // 착지 상태: lock delay 타이머 진행
            _lockTimer += Time.deltaTime;
            if (_lockTimer >= LockDelay)
            {
                LockPiece();
                return;
            }

            // 착지 상태에서도 아래가 빈 공간이면 (옆으로 이동해서 빠진 경우) 착지 해제
            if (IsValidPosition(Position + Vector2Int.down, _cells))
            {
                _isLanding = false;
            }
        }
        else
        {
            // 낙하 중: 일반 중력 타이머
            _stepTimer += Time.deltaTime;
            if (_stepTimer >= StepDelay)
            {
                _stepTimer = 0f;
                MoveDown();
            }
        }
    }

    private void MoveDown()
    {
        if (!TryMove(Vector2Int.down))
        {
            // 바닥에 닿음 → 착지 상태 시작
            _isLanding = true;
            _lockTimer = 0f;
        }
    }

    public bool TryMove(Vector2Int direction)
    {
        Vector2Int newPos = Position + direction;
        if (IsValidPosition(newPos, _cells))
        {
            Position = newPos;
            UpdateWorldPosition();
            _ghost?.Refresh();
            ResetLockTimer();
            return true;
        }
        return false;
    }

    public void SetGhost(GhostPiece ghost)
    {
        _ghost = ghost;
    }

    // 90도 시계 방향 회전
    // 회전 공식: (x, y) -> (y, -x)
    // Wall Kick으로 여러 위치를 시도해 하나라도 유효하면 회전 적용
    public bool TryRotate()
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
                // 고스트 피스에 회전된 셀 동기화 + 위치 갱신
                if (_ghost != null)
                {
                    _ghost.SyncCells(_cells);
                    _ghost.Refresh();
                }
                ResetLockTimer();
                return true;
            }
        }
        // 모든 kick 실패 → 회전 취소
        return false;
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

    // 착지 상태에서 이동/회전 성공 시 lock timer 리셋
    // 무한 리셋 방지를 위해 최대 횟수 제한
    private void ResetLockTimer()
    {
        if (_isLanding && _lockResetCount < MaxLockResets)
        {
            _lockTimer = 0f;
            _lockResetCount++;
        }
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
