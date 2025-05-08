using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioClip menuMusicClip; // Nhạc nền MainMenu
    [SerializeField] private AudioClip gameMusicClip; // Nhạc nền Game/Boss
    [SerializeField] private AudioClip buttonClickSFX; // Hiệu ứng nhấn nút
    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Thêm AudioSource
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        // Gán AudioMixer nhóm
        musicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
        sfxSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];

        // Cấu hình musicSource
        musicSource.loop = true;
        musicSource.playOnAwake = false;
    }

    void Start()
    {
        // Phát nhạc MainMenu
        PlayMusic(menuMusicClip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip != null && musicSource.clip != clip)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // Chuyển nhạc khi vào Game/Boss scene
    public void SwitchToGameMusic()
    {
        PlayMusic(gameMusicClip);
    }

    // Hiệu ứng nút
    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSFX);
    }
}