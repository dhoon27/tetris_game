using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("BGM")]
    public AudioClip bgmClip;        // 메인메뉴 BGM
    public AudioClip bgmIngameClip;   // 인게임 BGM

    [Header("SFX")]
    public AudioClip sfxMove;         // 블록 이동/회전
    public AudioClip sfxLanding;      // 블록 착지
    public AudioClip sfxLineClear;    // 줄 제거
    public AudioClip sfxGameOver;     // 게임 오버

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

        // SFX용 AudioSource (PlayOneShot으로 겹쳐 재생 가능)
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake = false;
        _sfxSource.volume = 1.0f;
    }

    private void Start()
    {
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
        if (bgmIngameClip == null) return;
        _bgmSource.clip = bgmIngameClip;
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

    public void PlayMove()
    {
        if (sfxMove != null)
            _sfxSource.PlayOneShot(sfxMove);
    }

    public void PlayLanding()
    {
        if (sfxLanding != null)
            _sfxSource.PlayOneShot(sfxLanding);
    }

    public void PlayLineClear()
    {
        if (sfxLineClear != null)
            _sfxSource.PlayOneShot(sfxLineClear);
    }

    public void PlayGameOver()
    {
        if (sfxGameOver != null)
            _sfxSource.PlayOneShot(sfxGameOver);
    }
}
