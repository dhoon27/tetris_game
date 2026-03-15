using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
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
        finalScoreText.text = $"SCORE\n{ScoreManager.Instance.Score:N0}";
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
