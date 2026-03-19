using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("BGM")]
    public AudioClip bgmClip;        // 메인메뉴 BGM
    public AudioClip bgmIngameClip;   // 인게임 BGM

    [Header("SFX")]
    public AudioClip sfxLanding;      // 블록 착지
    public AudioClip sfxLineClear;    // 줄 제거
    public AudioClip sfxGameOver;     // 게임 오버

    [Header("SFX 재생 제한 (초)")]
    public float sfxLandingDuration = 1.0f;
    public float sfxLineClearDuration = 2.0f;
    public float sfxGameOverDuration = 5.0f;

    private AudioSource _bgmSource;
    private AudioSource _sfxSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // BGM용 AudioSource
        _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.loop = true;
        _bgmSource.playOnAwake = false;
        _bgmSource.volume = 0.7f;

        // SFX용 AudioSource
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake = false;
        _sfxSource.volume = 1.0f;
    }

    private void Start()
    {
        // 메인메뉴 씬에서만 메뉴 BGM 자동 재생
        // Game 씬에서는 GameManager.Start()가 PlayIngameBGM()을 호출함
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
            PlayBGM();
    }

    // --- BGM ---

    public void PlayBGM()
    {
        if (bgmClip == null) return;
        _bgmSource.clip = bgmClip;
        if (!_bgmSource.isPlaying)
            _bgmSource.Play();
    }

    public void PlayIngameBGM()
    {
        _bgmSource.Stop();  // 기존 BGM 확실히 멈추고
        if (bgmIngameClip == null) return;
        _bgmSource.clip = bgmIngameClip;
        _bgmSource.volume = 0.4f;  // 인게임 BGM은 조용하게
        _bgmSource.Play();
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
    }

    public void SetVolume(float volume)
    {
        _bgmSource.volume = Mathf.Clamp01(volume);
    }

    // --- SFX ---
    // PlaySFX: 이전 효과음을 멈추고 새 효과음을 재생 (겹침 방지)
    // maxDuration: 이 시간 후 자동 정지 (긴 mp3 대응)

    private void PlaySFX(AudioClip clip, float maxDuration)
    {
        if (clip == null) return;
        _sfxSource.Stop();
        _sfxSource.clip = clip;
        _sfxSource.Play();
        CancelInvoke(nameof(StopSFX));
        Invoke(nameof(StopSFX), maxDuration);
    }

    private void StopSFX()
    {
        _sfxSource.Stop();
    }

    public void PlayLanding()
    {
        PlaySFX(sfxLanding, sfxLandingDuration);
    }

    public void PlayLineClear()
    {
        PlaySFX(sfxLineClear, sfxLineClearDuration);
    }

    public void PlayGameOver()
    {
        PlaySFX(sfxGameOver, sfxGameOverDuration);
    }
}
