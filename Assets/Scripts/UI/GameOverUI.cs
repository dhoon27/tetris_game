using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
    public TMP_Text newBestText;   // "NEW BEST!" 텍스트 (신기록 아니면 숨김)
    public Button retryButton;
    public Button mainMenuButton;

    private void Start()
    {
        retryButton.onClick.AddListener(OnRetry);
        mainMenuButton.onClick.AddListener(OnMainMenu);

        gameOverPanel.SetActive(false);
        GameManager.Instance.OnGameOver += ShowGameOver;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameOver -= ShowGameOver;
    }

    private void ShowGameOver()
    {
        var sm = ScoreManager.Instance;
        finalScoreText.text = $"SCORE\n{sm.Score:N0}\n\nBEST\n{sm.BestScore:N0}";
        newBestText.gameObject.SetActive(sm.IsNewBest);
        gameOverPanel.SetActive(true);
    }

    private void OnRetry()
    {
        SceneManager.LoadScene("Game");
    }

    private void OnMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
