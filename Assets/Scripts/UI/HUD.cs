using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [Header("텍스트 참조")]
    public TMP_Text scoreText;
    public TMP_Text levelText;
    public TMP_Text bestScoreText;

    private void Start()
    {
        ScoreManager.Instance.OnScoreChanged += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged -= Refresh;
    }

    private void Refresh()
    {
        scoreText.text = $"SCORE\n{ScoreManager.Instance.Score:N0}";
        levelText.text = $"LEVEL\n{ScoreManager.Instance.Level}";
        bestScoreText.text = $"BEST\n{ScoreManager.Instance.BestScore:N0}";
    }
}
