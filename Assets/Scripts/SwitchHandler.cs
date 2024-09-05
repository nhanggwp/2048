using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SwitchHandler : MonoBehaviour
{
    private float _musicState;
    private float _soundEffectState;
    private float _vibrationState;

    public GameObject SwitchBtnMusic;
    public GameObject SwitchBtnEffect;
    public GameObject SwitchBtnVibration;
    public Image SwitchImageMusic;
    public Image SwitchImageEffect;
    public Image SwitchImageVibration;
    public Color OnColor = Color.green;
    public Color OffColor = new Color32(0x8F, 0x7A, 0x66, 255);
    public AudioManager AudioManager;

    private void Awake()
    {
        _musicState = PlayerPrefs.GetFloat("MusicState", -1f);
        _soundEffectState = PlayerPrefs.GetFloat("SoundEffectState", -1f);
        _vibrationState = PlayerPrefs.GetFloat("VibrationState", -1f);

        UpdateMusicUI();
        UpdateSoundEffectUI();
        UpdateVibrationEffectUI();

        TurnOnOffSoundEffect();
        TurnOnOffMusic();
        TurnOnOffVibration();
    }

    public void OnMusicSwitchButtonClicked()
    {
        _musicState = _musicState == 1 ? -1 : 1;
        PlayerPrefs.SetFloat("MusicState", _musicState);
        PlayerPrefs.Save();

        UpdateMusicUI();
        TurnOnOffMusic();
    }

    public void OnEffectSwitchButtonClicked()
    {
        _soundEffectState = _soundEffectState == 1 ? -1 : 1;
        PlayerPrefs.SetFloat("SoundEffectState", _soundEffectState);
        PlayerPrefs.Save();

        UpdateSoundEffectUI();
        TurnOnOffSoundEffect();
    }

    public void OnVibrationSwitchButtonClicked()
    {
        _vibrationState = _vibrationState == 1 ? -1 : 1;
        PlayerPrefs.SetFloat("VibrationState", _vibrationState);
        PlayerPrefs.Save();

        UpdateVibrationEffectUI();
        TurnOnOffVibration();
    }

    private void UpdateMusicUI()
    {
        if (_musicState == 1)
        {
            SwitchImageMusic.DOColor(OnColor, 0.2f);
            SwitchBtnMusic.transform.DOLocalMoveX(40f, 0.2f);
        }
        else
        {
            SwitchImageMusic.DOColor(OffColor, 0.2f);
            SwitchBtnMusic.transform.DOLocalMoveX(-40f, 0.2f);
        }
    }

    private void UpdateSoundEffectUI()
    {
        if (_soundEffectState == 1)
        {
            SwitchImageEffect.DOColor(OnColor, 0.2f);
            SwitchBtnEffect.transform.DOLocalMoveX(40f, 0.2f);
        }
        else
        {
            SwitchImageEffect.DOColor(OffColor, 0.2f);
            SwitchBtnEffect.transform.DOLocalMoveX(-40f, 0.2f);
        }
    }

    private void UpdateVibrationEffectUI()
    {
        if (_vibrationState == 1)
        {
            SwitchImageVibration.DOColor(OnColor, 0.2f);
            SwitchBtnVibration.transform.DOLocalMoveX(40f, 0.2f);
        }
        else
        {
            SwitchImageVibration.DOColor(OffColor, 0.2f);
            SwitchBtnVibration.transform.DOLocalMoveX(-40f, 0.2f);
        }
    }

    public void TurnOnOffSoundEffect()
    {
        bool isSoundOn = _soundEffectState == 1;

        if (AudioManager == null) return;

        AudioManager.ToggleSound(isSoundOn, AudioManager.AudioSource);
    }

    public void TurnOnOffMusic()
    {
        bool isMusicOn = _musicState == 1;

        if (AudioManager == null) return;

        AudioManager.ToggleSound(isMusicOn, AudioManager.AudioSourceMusic);

        if (isMusicOn)
        {
            AudioManager.PlayMusic();
        }
        else
        {
            AudioManager.StopMusic();
        }
    }

    public void TurnOnOffVibration()
    {
        bool isVibrationOn = _vibrationState == 1;

        if (isVibrationOn && SystemInfo.supportsVibration)
        {
            Handheld.Vibrate();
            Debug.Log("Vibration triggered.");
        }
        else if (!isVibrationOn)
        {
            Debug.Log("Vibration is disabled.");
        }
        else
        {
            Debug.LogWarning("Vibration is not supported on this device.");
        }
    }

    public void ResetSettings()
    {
        _soundEffectState = -1f;
        _musicState = -1f;
        _vibrationState = -1f;

        PlayerPrefs.SetFloat("MusicState", _musicState);
        PlayerPrefs.SetFloat("SoundEffectState", _soundEffectState);
        PlayerPrefs.SetFloat("VibrationState", _vibrationState);
        PlayerPrefs.Save();

        UpdateMusicUI();
        UpdateSoundEffectUI();
        UpdateVibrationEffectUI();

        TurnOnOffSoundEffect();
        TurnOnOffMusic();
        TurnOnOffVibration();
    }
}
