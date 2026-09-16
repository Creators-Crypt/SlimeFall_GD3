using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class AudioSettingUI : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private TMP_Text masterValue;
    [SerializeField] private TMP_Text musicValue;
    [SerializeField] private TMP_Text sfxValue;

    private void FindUIReferences()
    {
        Slider[] sliders = GetComponentsInChildren<Slider>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        foreach (Slider slider in sliders)
        {
            switch (slider.name)
            {
                case "MasterSlider":
                    masterSlider = slider;
                    break;
                case "MusicSlider":
                    musicSlider = slider;
                    break;
                case "SFXSlider":
                    sfxSlider = slider;
                    break;
            }
        }

        foreach (TMP_Text text in texts)
        {
            switch (text.name)
            {
                case "MasterValue":
                    masterValue = text;
                    break;
                case "MusicValue":
                    musicValue = text;
                    break;
                case "SFXValue":
                    sfxValue = text;
                    break;
            }
        }
    }

    private void Start()
    {
        FindUIReferences();
        FindAudioMixer();

        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        masterSlider.onValueChanged.AddListener(UpdateMaster);
        musicSlider.onValueChanged.AddListener(UpdateMusic);
        sfxSlider.onValueChanged.AddListener(UpdateSFX);

        UpdateMaster(masterSlider.value);
        UpdateMusic(musicSlider.value);
        UpdateSFX(sfxSlider.value);

    }
    /*
     private void OnDisable() {
        
    Add a RemoveListener for each AddListenenr
    } 
    */
    private 
        void UpdateMaster(float value)
    {
        masterValue.text = Mathf.RoundToInt(value * 100f) + "%";
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    private 
        void UpdateMusic(float value)
    {
        value = Mathf.Clamp01(value);

        musicValue.text = Mathf.RoundToInt(value * 100f) + "%";

        float decibels = value <= 0f
            ? -80f
            : Mathf.Max(-80f, -14f + Mathf.Log10(value) * 20f);

        audioMixer.SetFloat("MusicVolume", decibels);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    private 
        void UpdateSFX(float value)
    {
        sfxValue.text = Mathf.RoundToInt(value * 100f) + "%";
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    private void FindAudioMixer()
    {
        if (audioMixer == null)
        {
            audioMixer = Resources.Load<AudioMixer>("MasterMixer");

            if (audioMixer == null)
            {
                Debug.LogError("MasterMixer not found in Resources folder.");
            }
        }
    }
}
