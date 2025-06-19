using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mainMixer;
    public Sound[] sounds;
    public static AudioManager instance;

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.mixerGroup;
        }
    }

    void Start()
    {
        float master = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        float music = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        SetMasterVolume(master);
        SetMusicVolume(music);
        SetSFXVolume(sfx);
        
        Play("Music");
    }

    public void Play(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        s.source.Play();
    }

    public void Stop(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        s.source.Stop();
    }

    public void SetMusicVolume(float sliderValue)
    {
        string exposedParameterName = "Music";
        float dbValue = (sliderValue <= 0.0001f) ? -80f : Mathf.Log10(sliderValue) * 20;
        mainMixer.SetFloat(exposedParameterName, dbValue);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {

        string exposedParameterName = "SoundEffects";
        float dbValue = (sliderValue <= 0.0001f) ? -80f : Mathf.Log10(sliderValue) * 20;
        mainMixer.SetFloat(exposedParameterName, dbValue);
        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
    }

    public void SetMasterVolume(float sliderValue)
    {
        string exposedParameterName = "Master";
        float dbValue = (sliderValue <= 0.0001f) ? -80f : Mathf.Log10(sliderValue) * 20;
        mainMixer.SetFloat(exposedParameterName, dbValue);
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
    }
}