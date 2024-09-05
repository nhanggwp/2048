using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource _audioSourceMusic;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _mergeClip;
    [SerializeField] private AudioClip _moveClip;
    [SerializeField] private AudioClip _gameOver;
    [SerializeField] private AudioClip _backgroundMusic;

    public float musicState;
    public float soundEffectState;
    private bool isSound;

    public AudioSource AudioSourceMusic => _audioSourceMusic;
    public AudioSource AudioSource => _audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        musicState = PlayerPrefs.GetFloat("MusicState", -1f);
        soundEffectState = PlayerPrefs.GetFloat("SoundEffectState", -1f);

        ApplyAudioStates();
    }

    private void ApplyAudioStates()
    {
        bool isSoundOn = soundEffectState == 1;
        bool isMusicOn = musicState == 1;

        ToggleSound(isSoundOn, _audioSource);
        ToggleSound(isMusicOn, _audioSourceMusic);

        if (isMusicOn)
        {
            PlayMusic();
        }
        else
        {
            StopMusic();
        }
    }

    public void PlayMergeSound()
    {
        if (_mergeClip != null)
        {
            _audioSource.PlayOneShot(_mergeClip);
        }
    }

    public void PlayMoveSound()
    {
        if (_moveClip != null)
        {
            _audioSource.PlayOneShot(_moveClip);
        }
    }

    public void PlayGameOverSound()
    {
        if (_gameOver != null)
        {
            _audioSource.PlayOneShot(_gameOver);
        }
    }

    public void PlayMusic()
    {
        if (_backgroundMusic != null && _audioSourceMusic != null)
        {
            _audioSourceMusic.clip = _backgroundMusic;
            _audioSourceMusic.loop = true;
            _audioSourceMusic.Play();
        }
    }

    public void StopSoundEffect()
    {
        if (_audioSource != null)
        {
            _audioSource.Stop();
        }
    }

    public void StopMusic()
    {
        if (_audioSourceMusic != null)
        {
            _audioSourceMusic.Stop();
        }
    }

    public void ToggleSound(bool isSound, AudioSource audioSource)
    {
        if (audioSource != null)
        {
            audioSource.mute = !isSound;
        }
    }
}
