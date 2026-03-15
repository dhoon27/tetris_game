using UnityEngine;

public class Piece : MonoBehaviour
{
    private Board _board;
    private TetrominoData _data;

    public Vector2Int Position { get; private set; }

    private float _stepTimer;
    public float StepDelay = 1f;

    public void Initialize(Board board, TetrominoData data, Vector2Int spawnPos)
    {
        _board = board;
        _data = data;
        Position = spawnPos;

        // 조명 영향을 받지 않는 Unlit 머티리얼 로드
        var unlitMat = Resources.Load<Material>("Sprite-Unlit-Default")
                       ?? new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default"));

        foreach (var cell in data.cells)
        {
            var blockObj = new GameObject("Block");
            blockObj.transform.SetParent(transform);

            var sr = blockObj.AddComponent<SpriteRenderer>();
            sr.sprite = data.sprite;
            sr.sortingOrder = 1;
            sr.material = unlitMat;

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
        if (IsValidPosition(newPos))
        {
            Position = newPos;
            UpdateWorldPosition();
            return true;
        }
        return false;
    }

    private bool IsValidPosition(Vector2Int pivot)
    {
        foreach (var cell in _data.cells)
        {
            if (!_board.IsValidPosition(pivot + cell))
                return false;
        }
        return true;
    }

    // 회전 (Phase 1-6에서 구현)
    public void TryRotate()
    {
        // TODO: Phase 1-6
    }

    // 하드 드롭: 바닥까지 즉시 이동
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

    private void UpdateWorldPosition()
    {
        transform.position = _board.BoardToWorld(Position);
    }
}
