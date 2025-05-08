using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public AudioMixer audioMixer; // Gán MainAudioMixer
    public Slider masterSlider; // Slider cho Master Volume
    public Slider musicSlider; // Slider cho Music Volume
    public Slider sfxSlider; // Slider cho SFX Volume
    public GameObject settingsPanel; // Gán SettingsPanel
    public Button settingsButton; // Gán SettingsButton
    public Button closeButton; // Gán CloseButton

    void Start()
    {
        // Load giá trị lưu trước
        masterSlider.value = PlayerPrefs.GetFloat("MasterVol", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVol", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", 1f);

        // Gán sự kiện slider
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        // Gán sự kiện nút
        settingsButton.onClick.AddListener(() => {
            settingsPanel.SetActive(true);
            AudioManager.Instance?.PlayButtonClick();
        });
        closeButton.onClick.AddListener(() => {
            settingsPanel.SetActive(false);
            AudioManager.Instance?.PlayButtonClick();
        });

        // Tắt panel ban đầu
        settingsPanel.SetActive(false);

        // Áp dụng âm lượng ban đầu
        SetMasterVolume(masterSlider.value);
        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);
    }

    public void SetMasterVolume(float value)
    {
        float dB = value > 0 ? Mathf.Log10(value) * 20 : -80f;
        audioMixer.SetFloat("MasterVol", dB);
        PlayerPrefs.SetFloat("MasterVol", value);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float value)
    {
        float dB = value > 0 ? Mathf.Log10(value) * 20 : -80f;
        audioMixer.SetFloat("MusicVol", dB);
        PlayerPrefs.SetFloat("MusicVol", value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        float dB = value > 0 ? Mathf.Log10(value) * 20 : -80f;
        audioMixer.SetFloat("SFXVol", dB);
        PlayerPrefs.SetFloat("SFXVol", value);
        PlayerPrefs.Save();
    }
}