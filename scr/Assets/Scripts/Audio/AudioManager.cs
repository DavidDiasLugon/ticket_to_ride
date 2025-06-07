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
        Play("Music");
    }

    public void Play(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        s.source.Play();
    }

    public void SetMusicVolume(float sliderValue)
    {
        string exposedParameterName = "Music";
        if (sliderValue <= 0.0001f)
        {
            mainMixer.SetFloat(exposedParameterName, -80f);
        }
        else
        {
            float dbValue = Mathf.Log10(sliderValue) * 20;
            mainMixer.SetFloat(exposedParameterName, dbValue);
        }
    }

    public void SetSFXVolume(float sliderValue)
    {

        string exposedParameterName = "SoundEffects";
        if (sliderValue <= 0.0001f)
        {
            mainMixer.SetFloat(exposedParameterName, -80f);
        }
        else
        {
            float dbValue = Mathf.Log10(sliderValue) * 20;
            mainMixer.SetFloat(exposedParameterName, dbValue);
        }
    }
    
    public void SetMasterVolume(float sliderValue)
    {
        string exposedParameterName = "Master";
        if (sliderValue <= 0.0001f)
        {
            mainMixer.SetFloat(exposedParameterName, -80f);
        }
        else
        {
            float dbValue = Mathf.Log10(sliderValue) * 20;
            mainMixer.SetFloat(exposedParameterName, dbValue);
        }
    }
}