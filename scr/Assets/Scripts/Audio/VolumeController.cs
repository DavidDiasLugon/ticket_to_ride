using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeController : MonoBehaviour
{
    public Slider sliderVolumeMusica;
    public Slider sliderVolumeEfeitos;
    public Slider sliderVolumeGeral;

    private AudioManager audioManager;

    void Start()
    {
        audioManager = FindAnyObjectByType<AudioManager>();

        float music = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        float master = PlayerPrefs.GetFloat("MasterVolume", 0.75f);

        sliderVolumeMusica.value = music;
        sliderVolumeEfeitos.value = sfx;
        sliderVolumeGeral.value = master;

        sliderVolumeMusica.onValueChanged.AddListener(audioManager.SetMusicVolume);
        sliderVolumeEfeitos.onValueChanged.AddListener(audioManager.SetSFXVolume);
        sliderVolumeGeral.onValueChanged.AddListener(audioManager.SetMasterVolume);
    }
}
