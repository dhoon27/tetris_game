using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    public GameObject pausePanel;   // 반투명 오버레이 패널
    public Button pauseButton;      // HUD의 일시정지 버튼
    public Button resumeButton;     // 패널 안의 재개 버튼
    public Button mainMenuButton;   // 패널 안의 메인메뉴 버튼

    private void Start()
    {
        pauseButton.onClick.AddListener(OnPauseButton);
        resumeButton.onClick.AddListener(OnResumeButton);
        mainMenuButton.onClick.AddListener(OnMainMenuButton);

        pausePanel.SetActive(false);
        PauseManager.Instance.OnPauseChanged += OnPauseChanged;
    }

    private void OnDestroy()
    {
        if (PauseManager.Instance != null)
            PauseManager.Instance.OnPauseChanged -= OnPauseChanged;
    }

    private void OnPauseButton() => PauseManager.Instance.Pause();
    private void OnResumeButton() => PauseManager.Instance.Resume();

    private void OnMainMenuButton()
    {
        PauseManager.Instance.Resume(); // timeScale 복구
        SceneManager.LoadScene("MainMenu");
    }

    private void OnPauseChanged(bool isPaused)
    {
        pausePanel.SetActive(isPaused);
    }
}
