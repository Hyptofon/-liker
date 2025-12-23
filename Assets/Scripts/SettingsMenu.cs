using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button openButton;
    [SerializeField] private Button closeButton;
    
    [Header("Audio Controls")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle muteToggle;
    
    private const string PREF_MASTER = "Pref_MasterVol";
    private const string PREF_MUSIC = "Pref_MusicVol";
    private const string PREF_SFX = "Pref_SFXVol";
    private const string PREF_MUTE = "Pref_IsMuted";

    private void Start()
    {
        settingsPanel.SetActive(false);
        masterSlider.value = PlayerPrefs.GetFloat(PREF_MASTER, 1f);
        musicSlider.value = PlayerPrefs.GetFloat(PREF_MUSIC, 1f);
        sfxSlider.value = PlayerPrefs.GetFloat(PREF_SFX, 1f);
        muteToggle.isOn = PlayerPrefs.GetInt(PREF_MUTE, 0) == 1;
        openButton.onClick.AddListener(OpenSettings);
        closeButton.onClick.AddListener(CloseSettings);
        masterSlider.onValueChanged.AddListener((v) => AudioManager.Instance.SetMasterVolume(v));
        musicSlider.onValueChanged.AddListener((v) => AudioManager.Instance.SetMusicVolume(v));
        sfxSlider.onValueChanged.AddListener((v) => AudioManager.Instance.SetSFXVolume(v));
        muteToggle.onValueChanged.AddListener((ismuted) => AudioManager.Instance.ToggleMute(ismuted));
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f; 
    }
}