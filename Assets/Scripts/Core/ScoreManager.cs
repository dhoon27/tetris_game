using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score { get; private set; }
    public int Level { get; private set; } = 1;
    public int TotalLines { get; private set; }
    public int BestScore { get; private set; }

    // 이번 게임에서 신기록을 세웠는지
    public bool IsNewBest { get; private set; }

    // 점수 변경 시 HUD에 알리는 이벤트
    public event System.Action OnScoreChanged;

    private const string BestScoreKey = "BestScore";
    private const int LinesPerLevel = 10;
    private static readonly int[] LineScores = { 0, 100, 300, 500, 800 };

    private void Awake()
    {
        Instance = this;
        BestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
    }

    public void AddScore(int linesCleared)
    {
        if (linesCleared <= 0) return;

        int gain = LineScores[Mathf.Clamp(linesCleared, 0, 4)] * Level;
        Score += gain;
        TotalLines += linesCleared;

        Level = Mathf.Min(1 + TotalLines / LinesPerLevel, 15);

        // 최고 점수 갱신
        if (Score > BestScore)
        {
            BestScore = Score;
            IsNewBest = true;
            PlayerPrefs.SetInt(BestScoreKey, BestScore);
            PlayerPrefs.Save();
        }

        OnScoreChanged?.Invoke();
    }

    public void Reset()
    {
        Score = 0;
        Level = 1;
        TotalLines = 0;
        IsNewBest = false;
        OnScoreChanged?.Invoke();
    }

    public float GetStepDelay()
    {
        return Mathf.Max(1.0f - (Level - 1) * 0.065f, 0.1f);
    }
}
