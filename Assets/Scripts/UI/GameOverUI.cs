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

        // GameOver 패널이 뜰 때 SafeAreaPanel의 raycastTarget을 꺼서
        // RETRY/MAIN MENU 버튼이 터치를 받을 수 있게 함
        if (TouchInputHandler.Instance != null)
            TouchInputHandler.Instance.SetInputActive(false);
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
