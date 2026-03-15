using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    public bool IsPaused { get; private set; }

    public event System.Action<bool> OnPauseChanged; // true=일시정지, false=재개

    private void Awake()
    {
        Instance = this;
    }

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        OnPauseChanged?.Invoke(true);
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        OnPauseChanged?.Invoke(false);
    }

    public void TogglePause()
    {
        if (IsPaused) Resume();
        else Pause();
    }

    // 씬 전환 전 timeScale 복구
    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
