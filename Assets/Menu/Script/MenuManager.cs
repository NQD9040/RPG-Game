using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public TMP_Text startButtonText;
    public TMP_Text settingsButtonText;
    public TMP_Text exitButtonText;
    public Button exitSettingsButton;
    private Button startButton;
    private Button settingsButton;
    private Button exitButton;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    private string backgroundMusicName = "Background_Music";
    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Start()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.RegisterAudioSource(musicSource, AudioManager.AudioType.Music);
        }
        else
        {
            Debug.LogWarning("AudioManager not found in the scene!");
        }
        PlayBackgroundMusic();

        // Thiết lập AudioSource cho SFX
        sfxSource = gameObject.AddComponent<AudioSource>();
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.RegisterAudioSource(sfxSource, AudioManager.AudioType.SoundEffect);
        }
        else
        {
            Debug.LogWarning("AudioManager not found in the scene!");
        }

        if (fadeCanvasGroup == null)
        {
            Debug.LogWarning("Fade CanvasGroup is not assigned in the Inspector!");
        }

        if (startButtonText != null)
        {
            startButton = startButtonText.GetComponentInParent<Button>();
            if (startButton != null)
            {
                startButton.onClick.AddListener(() => StartCoroutine(FadeAndLoadScene()));
            }
        }

        if (settingsButtonText != null)
        {
            settingsButton = settingsButtonText.GetComponentInParent<Button>();
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(Setting);
            }
        }

        if (exitButtonText != null)
        {
            exitButton = exitButtonText.GetComponentInParent<Button>();
            if (exitButton != null)
            {
                exitButton.onClick.AddListener(ExitGame);
            }
        }

        if (exitSettingsButton != null)
        {
            exitSettingsButton.onClick.AddListener(ExitSetting);
        }

        if (musicSlider != null)
        {
            float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            musicSlider.value = savedMusicVolume;
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            OnMusicVolumeChanged(savedMusicVolume);
        }
        else
        {
            Debug.LogWarning("Music Slider is not assigned in the Inspector!");
        }

        if (sfxSlider != null)
        {
            float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxSlider.value = savedSFXVolume;
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            OnSFXVolumeChanged(savedSFXVolume);
        }
        else
        {
            Debug.LogWarning("SFX Slider is not assigned in the Inspector!");
        }
    }

    void OnDestroy()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.UnregisterAudioSource(musicSource);
            AudioManager.Instance.UnregisterAudioSource(sfxSource);
        }
    }

    private void PlayBackgroundMusic()
    {
        AudioClip musicClip = Resources.Load<AudioClip>("Sounds/" + backgroundMusicName);
        if (musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.Play();
        }
    }

    private IEnumerator FadeAndLoadScene()
    {
        PlaySound("button_click");
        Debug.Log("Starting game...");

        if (fadeCanvasGroup != null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                yield return null;
            }
            fadeCanvasGroup.alpha = 1f;
        }

        SceneManager.LoadScene(1);
    }

    private void Setting()
    {
        PlaySound("button_click");
        settingsCanvas.gameObject.SetActive(true);
    }

    private void ExitSetting()
    {
        PlaySound("button_click");
        settingsCanvas.gameObject.SetActive(false);
    }

    public void ExitGame()
    {
        PlaySound("button_click");
        Debug.Log("Exiting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void PlaySound(string soundName)
    {
        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + soundName);
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
            Debug.Log($"Playing sound: {soundName} on GameObject: {sfxSource.gameObject.name}");
        }
    }

    private void OnMusicVolumeChanged(float value)
    {
        Debug.Log($"Music Volume changed to: {value}");
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
    }

    private void OnSFXVolumeChanged(float value)
    {
        Debug.Log($"SFX Volume changed to: {value}");
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
        }
    }
}