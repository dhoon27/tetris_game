using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("참조")]
    public Board board;
    public TetrominoData[] tetrominoes; // 인스펙터에서 7개 할당

    // 블록 스폰 위치 (보드 좌표)
    // x=4: 10칸 보드의 중앙, y=18: 최상단 두 줄 위
    private readonly Vector2Int _spawnPosition = new Vector2Int(4, 18);

    private Piece _activePiece;

    // 다음에 나올 블록 데이터
    public TetrominoData NextData { get; private set; }

    // 다음 블록이 바뀔 때 NextPieceDisplay에 알리는 이벤트
    public event System.Action OnNextPieceChanged;

    // 게임 오버 시 GameOverUI에 알리는 이벤트
    public event System.Action OnGameOver;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 게임 시작 시 next를 미리 뽑아두고 첫 스폰
        NextData = PickRandom();
        OnNextPieceChanged?.Invoke();
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        var data = NextData;

        // 다음 블록 미리 뽑기
        NextData = PickRandom();
        OnNextPieceChanged?.Invoke();

        var go = new GameObject("ActivePiece");
        _activePiece = go.AddComponent<Piece>();
        _activePiece.StepDelay = ScoreManager.Instance.GetStepDelay();
        _activePiece.Initialize(board, data, _spawnPosition);

        // 스폰 직후 보드 최상단이 차있으면 게임 오버
        if (board.IsGameOver())
        {
            Destroy(go);
            TriggerGameOver();
        }
    }

    // Piece가 고정된 후 호출됨
    public void OnPieceLocked(int linesCleared)
    {
        ScoreManager.Instance.AddScore(linesCleared);

        if (board.IsGameOver())
        {
            TriggerGameOver();
            return;
        }

        SpawnPiece();
    }

    // TouchInputHandler에서 호출되는 조작 메서드들
    public void TryMovePiece(Vector2Int direction)
    {
        _activePiece?.TryMove(direction);
    }

    public void RotatePiece()
    {
        _activePiece?.TryRotate();
    }

    public void SoftDropPiece()
    {
        _activePiece?.TryMove(Vector2Int.down);
    }

    public void HardDropPiece()
    {
        _activePiece?.HardDrop();
    }

    private void TriggerGameOver()
    {
        Debug.Log("Game Over!");
        OnGameOver?.Invoke();
    }

    private TetrominoData PickRandom()
    {
        return tetrominoes[Random.Range(0, tetrominoes.Length)];
    }
}
