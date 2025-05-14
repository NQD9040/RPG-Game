using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private Button menuButton;
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject statsCanvas;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button openStatsButton;
    [SerializeField] private Button closeStatsButton;
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    [SerializeField] private GameObject settingCanvas;
    [SerializeField] private GameObject selectCanvas;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button exitSettingButton;
    [SerializeField] private string backgroundMusicName = "Background_Music_Main";
    [SerializeField] private TextMeshProUGUI timeText;
    private SelectManager select;
    private bool isOpenStats = false;
    private bool isOpenSetting = false;
    private AudioSource musicSource;
    private AudioSource sfxSource;
    private float time = 0f;
    void Start()
    {
        selectCanvas.gameObject.SetActive(true);
        select = GameObject.FindGameObjectWithTag("Select").GetComponent<SelectManager>();
        selectCanvas.gameObject.SetActive(false);
        if (select == null)
        {
            Debug.Log("Select not found");
        }
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

        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
        }
        else
        {
            Debug.LogWarning("MenuCanvas is not assigned in the Inspector!");
        }

        if (settingCanvas != null)
        {
            settingCanvas.SetActive(false);
        }
        else
        {
            Debug.LogWarning("SettingCanvas is not assigned in the Inspector!");
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(ToggleMenu);
        }
        else
        {
            Debug.LogWarning("Menu Button is not assigned in the Inspector!");
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }
        else
        {
            Debug.LogWarning("Resume Button is not assigned in the Inspector!");
        }

        if (settingButton != null)
        {
            settingButton.onClick.AddListener(OpenSettings);
        }
        else
        {
            Debug.LogWarning("Setting Button is not assigned in the Inspector!");
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(() => StartCoroutine(FadeAndLoadMenu()));
        }
        else
        {
            Debug.LogWarning("Exit Button is not assigned in the Inspector!");
        }

        if (openStatsButton != null)
        {
            openStatsButton.onClick.AddListener(OpenStats);
        }
        else
        {
            Debug.LogWarning("Open Stats Button is not assigned in the Inspector!");
        }

        if (closeStatsButton != null)
        {
            closeStatsButton.onClick.AddListener(CloseStats);
        }
        else
        {
            Debug.LogWarning("Close Stats Button is not assigned in the Inspector!");
        }

        if (exitSettingButton != null)
        {
            exitSettingButton.onClick.AddListener(ExitSetting);
        }
        else
        {
            Debug.LogWarning("Exit Setting Button is not assigned in the Inspector!");
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
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isOpenSetting)
                ExitSetting();
            else
                ToggleMenu();
        }
            
        if (Input.GetKeyDown("tab"))
        {
            if (!isOpenStats)
                OpenStats();
            else
                CloseStats();
        }
        time += Time.deltaTime;
        timeText.text = "" + Mathf.Round(time);
    }
    void OnDestroy()
    {

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.UnregisterAudioSource(musicSource);
            AudioManager.Instance.UnregisterAudioSource(sfxSource);
        }
    }

    private void ToggleMenu()
    {
        PlaySound("button_click");
        if (menuCanvas != null)
        {
            bool isActive = menuCanvas.activeSelf;
            menuCanvas.SetActive(!isActive);
            Time.timeScale = isActive ? 1f : 0f;

            if (!isActive && settingCanvas != null)
            {
                settingCanvas.SetActive(false);
            }
        }
    }

    private void ResumeGame()
    {
        PlaySound("button_click");
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
            if (!select.IsOpenSelectTrue()) Time.timeScale = 1f;

            if (settingCanvas != null)
            {
                settingCanvas.SetActive(false);
            }
        }
    }

    private void OpenSettings()
    {
        PlaySound("button_click");
        if (settingCanvas != null)
        {
            settingCanvas.SetActive(true);
            Debug.Log("Settings menu opened!");
            isOpenSetting = true;
        }
    }

    private void ExitSetting()
    {
        PlaySound("button_click");
        if (settingCanvas != null)
        {
            settingCanvas.SetActive(false);
            isOpenSetting = false;
        }
    }

    private IEnumerator FadeAndLoadMenu()
    {
        PlaySound("button_click");

        if (fadeCanvasGroup != null)
        {
            if (menuCanvas != null)
            {
                menuCanvas.SetActive(false);
                Time.timeScale = 1f;
            }
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                yield return null;
            }
            fadeCanvasGroup.alpha = 1f;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    private void PlaySound(string soundName)
    {
        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + soundName);
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
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

    private void OpenStats()
    {
        PlaySound("button_click");
        if (statsCanvas != null)
        {
            bool isActive = statsCanvas.activeSelf;
            statsCanvas.SetActive(!isActive);
            openStatsButton.gameObject.SetActive(false);
        }
    }

    private void CloseStats()
    {
        PlaySound("button_click");
        if (statsCanvas != null)
        {
            statsCanvas.SetActive(false);
            openStatsButton.gameObject.SetActive(true);
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