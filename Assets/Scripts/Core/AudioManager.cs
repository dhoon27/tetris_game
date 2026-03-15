using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("BGM")]
    public AudioClip bgmClip;

    private AudioSource _bgmSource;

    private void Awake()
    {
        // 씬을 넘어도 파괴되지 않도록 (메인메뉴 → 게임 씬 이동해도 음악 유지하고 싶을 때 사용)
        // 지금은 메인메뉴 전용이므로 일단 단순하게 유지
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.clip = bgmClip;
        _bgmSource.loop = true;
        _bgmSource.playOnAwake = false;
        _bgmSource.volume = 0.7f;
    }

    private void Start()
    {
        PlayBGM();
    }

    public void PlayBGM()
    {
        if (bgmClip == null) return;
        if (!_bgmSource.isPlaying)
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
}
