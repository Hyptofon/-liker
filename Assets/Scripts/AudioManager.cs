using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer Setup")]
    [SerializeField] private AudioMixer mainMixer;
    private const string PREF_MASTER = "Pref_MasterVol";
    private const string PREF_MUSIC = "Pref_MusicVol";
    private const string PREF_SFX = "Pref_SFXVol";
    private const string PREF_MUTE = "Pref_IsMuted";
    private const string PREF_TRACK_INDEX = "Pref_TrackIndex";
    private const string MIXER_MASTER = "MasterVol";
    private const string MIXER_MUSIC = "MusicVol";
    private const string MIXER_SFX = "SFXVol";

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Assets")]
    [SerializeField] private List<AudioClip> backgroundTracks;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip cookieClickSound;
    [SerializeField] private AudioClip upgradeBuySound;

    private int _currentTrackIndex = 0;
    private bool _isMuted = false;

    void Awake()
    {
        if (Instance == null) 
        { 
            Instance = this; 
            DontDestroyOnLoad(gameObject); 
        }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        LoadVolumeSettings();
        _currentTrackIndex = PlayerPrefs.GetInt(PREF_TRACK_INDEX, 0);
        PlayTrack(_currentTrackIndex);
    }
    
    void Update()
    {
        if (!musicSource.isPlaying && backgroundTracks.Count > 0) 
        {
            PlayNextTrack();
        }
    }
    public void PlayNextTrack()
    {
        if (backgroundTracks.Count == 0) return;
        
        _currentTrackIndex = (_currentTrackIndex + 1) % backgroundTracks.Count;
        PlayTrack(_currentTrackIndex);
    }

    private void PlayTrack(int index)
    {
        if (backgroundTracks.Count <= index) return;
        
        musicSource.clip = backgroundTracks[index];
        musicSource.Play();
        PlayerPrefs.SetInt(PREF_TRACK_INDEX, index);
        PlayerPrefs.Save();
    }

    public void PlayButtonClick() => PlaySFX(buttonClickSound);
    public void PlayCookieClick() => PlaySFX(cookieClickSound);
    public void PlayUpgradeBuy() => PlaySFX(upgradeBuySound);

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null) sfxSource.PlayOneShot(clip);
    }
    
    public void SetMasterVolume(float value)
    {
        SetMixerVolume(MIXER_MASTER, value);
        PlayerPrefs.SetFloat(PREF_MASTER, value); 
    }
    
    public void SetMusicVolume(float value)
    {
        SetMixerVolume(MIXER_MUSIC, value);
        PlayerPrefs.SetFloat(PREF_MUSIC, value); 
    }
    
    public void SetSFXVolume(float value)
    {
        SetMixerVolume(MIXER_SFX, value);
        PlayerPrefs.SetFloat(PREF_SFX, value);
    }

    private void SetMixerVolume(string parameterName, float sliderValue)
    {
        if (_isMuted && parameterName == MIXER_MASTER) return; 

        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20;
        mainMixer.SetFloat(parameterName, dB);
    }
    
    public void ToggleMute(bool isMuted)
    {
        _isMuted = isMuted;
        float volume = isMuted ? -80f : 0f; 
        mainMixer.SetFloat(MIXER_MASTER, volume);
        

        if (!isMuted)
        {
             float savedMaster = PlayerPrefs.GetFloat(PREF_MASTER, 1f);
             SetMasterVolume(savedMaster);
        }

        PlayerPrefs.SetInt(PREF_MUTE, isMuted ? 1 : 0);
    }

    private void LoadVolumeSettings()
    {
        float masterVol = PlayerPrefs.GetFloat(PREF_MASTER, 1f);
        float musicVol = PlayerPrefs.GetFloat(PREF_MUSIC, 1f);
        float sfxVol = PlayerPrefs.GetFloat(PREF_SFX, 1f);
        bool isMuted = PlayerPrefs.GetInt(PREF_MUTE, 0) == 1;
        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);
        if (isMuted) ToggleMute(true);
        else SetMasterVolume(masterVol);
    }
}