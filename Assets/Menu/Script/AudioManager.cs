using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    private List<AudioSource> musicSources = new List<AudioSource>();
    private List<AudioSource> sfxSources = new List<AudioSource>();

    public enum AudioType
    {
        Music,
        SoundEffect
    }

    public static AudioManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        SetMusicVolume(savedMusicVolume);
        SetSFXVolume(savedSFXVolume);
    }

    public void RegisterAudioSource(AudioSource source, AudioType type)
    {
        if (source == null) return;

        if (type == AudioType.Music)
        {
            if (!musicSources.Contains(source))
            {
                musicSources.Add(source);
                source.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            }
        }
        else
        {
            if (!sfxSources.Contains(source))
            {
                sfxSources.Add(source);
                source.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            }
        }
    }

    public void UnregisterAudioSource(AudioSource source)
    {
        if (source == null) return;
        musicSources.Remove(source);
        sfxSources.Remove(source);
    }

    public void SetMusicVolume(float volume)
    {
        Debug.Log($"Setting Music Volume to: {volume}");
        foreach (var source in musicSources)
        {
            if (source != null)
            {
                source.volume = volume;
            }
        }
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        Debug.Log($"Setting SFX Volume to: {volume}");
        foreach (var source in sfxSources)
        {
            if (source != null)
            {
                source.volume = volume;
            }
        }
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
}