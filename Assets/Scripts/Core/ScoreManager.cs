using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score { get; private set; }
    public int Level { get; private set; } = 1;
    public int TotalLines { get; private set; }

    // 점수 변경 시 HUD에 알리는 이벤트
    public event System.Action OnScoreChanged;

    // 레벨당 필요한 줄 수
    private const int LinesPerLevel = 10;

    // 줄 수에 따른 기본 점수 (테트리스 공식)
    private static readonly int[] LineScores = { 0, 100, 300, 500, 800 };

    private void Awake()
    {
        Instance = this;
    }

    // Board에서 줄 제거 후 GameManager를 통해 호출
    public void AddScore(int linesCleared)
    {
        if (linesCleared <= 0) return;

        int gain = LineScores[Mathf.Clamp(linesCleared, 0, 4)] * Level;
        Score += gain;
        TotalLines += linesCleared;

        // 10줄마다 레벨 업 (최대 레벨 15)
        Level = Mathf.Min(1 + TotalLines / LinesPerLevel, 15);

        OnScoreChanged?.Invoke();
    }

    public void Reset()
    {
        Score = 0;
        Level = 1;
        TotalLines = 0;
        OnScoreChanged?.Invoke();
    }

    // Piece의 낙하 속도: 레벨이 높을수록 빨라짐
    public float GetStepDelay()
    {
        // 레벨 1 = 1.0초, 레벨 15 = 0.1초
        return Mathf.Max(1.0f - (Level - 1) * 0.065f, 0.1f);
    }
}
