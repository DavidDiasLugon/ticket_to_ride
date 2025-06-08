using UnityEngine;
using UnityEngine.Audio;
using System;

public class GameAudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static GameAudioManager instance;

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
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
}