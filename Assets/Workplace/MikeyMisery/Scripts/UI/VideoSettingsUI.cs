using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VideoSettingsUI : MonoBehaviour
{
    private TMP_Dropdown resolutionDropdown;
    private TMP_Dropdown displayModeDropdown;
    private Toggle vSyncToggle;
    private TMP_Dropdown frameRateDropdown;
    private Slider brightnessSlider;
    private TMP_Text brightnessValue;

    private Resolution[] resolutions;
    private Volume brightnessVolume;
    private ColorAdjustments colorAdjustments;

    private void FindUIReferences()
    {
        TMP_Dropdown[] dropdowns = GetComponentsInChildren<TMP_Dropdown>(true);
        Toggle[] toggles = GetComponentsInChildren<Toggle>(true);
        Slider[] sliders = GetComponentsInChildren<Slider>(true);
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);

        foreach (TMP_Dropdown dropdown in dropdowns)
        {
            switch (dropdown.name)
            {
                case "ResolutionDropdown":
                    resolutionDropdown = dropdown;
                    break;
                case "DisplayModeDropdown":
                    displayModeDropdown = dropdown;
                    break;
                case "FrameRateDropdown":
                    frameRateDropdown = dropdown;
                    break;
            }
        }
        foreach (Toggle toggle in toggles)
        {
            if (toggle.name == "VSyncToggle")
            {
                vSyncToggle = toggle;
                break;
            }
        }
        foreach (Slider slider in sliders)
        {
            if (slider.name == "BrightnessSlider")
            {
                brightnessSlider = slider;
                break;
            }
        }
        foreach (TMP_Text text in texts)
        {
            if (text.name == "BrightnessValue")
            {
                brightnessValue = text;
                break;
            }
        }
    }

    private void Start()
    {
        FindUIReferences();

        SetupResolution();
        SetupDisplayMode();
        SetupFrameRate();
        SetupBrightness();

        int savedResolution = PlayerPrefs.GetInt("Resolution", resolutionDropdown.value);

        if (savedResolution >= 0 && savedResolution < resolutions.Length)
        {
            resolutionDropdown.value = savedResolution;
            resolutionDropdown.RefreshShownValue();
        }
        
        bool savedVSync = PlayerPrefs.GetInt("VSync", 1) == 1; // Default to VSync enabled
        int savedFrameRate = PlayerPrefs.GetInt("FrameRate", 1); // Default to 60 FPS
        int savedDisplayMode = PlayerPrefs.GetInt("DisplayMode", 1); // Default to Borderless

        vSyncToggle.isOn = savedVSync;

        frameRateDropdown.value = savedFrameRate;
        frameRateDropdown.RefreshShownValue();

        displayModeDropdown.value = savedDisplayMode;
        displayModeDropdown.RefreshShownValue();

        SetDisplayMode(savedDisplayMode);
        SetVSync(savedVSync);
        SetFrameRate(savedFrameRate);

        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 50f); // Default to 50% brightness
        brightnessValue.text = Mathf.RoundToInt(brightnessSlider.value) + "%";
        SetBrightness(brightnessSlider.value);

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        displayModeDropdown.onValueChanged.AddListener(SetDisplayMode);
        vSyncToggle.onValueChanged.AddListener(SetVSync);
        frameRateDropdown.onValueChanged.AddListener(SetFrameRate);
        brightnessSlider.onValueChanged.AddListener(SetBrightness);
    }

    private void SetupResolution()
    {
        Resolution[] availableResolutions = Screen.resolutions;

        var uniqueResolutions = new System.Collections.Generic.List<Resolution>();
        var options = new System.Collections.Generic.List<string>();

        int currentResolutionIndex = 0;

        foreach (Resolution resolution in availableResolutions)
        {
            bool alreadyAdded = uniqueResolutions.Exists(r => 
            r.width == resolution.width && 
            r.height == resolution.height);

            if (alreadyAdded)
                continue;

            uniqueResolutions.Add(resolution);

            options.Add(resolution.width + " x " + resolution.height);

            if (resolution.width == Screen.width && 
                resolution.height == Screen.height)
            {
                currentResolutionIndex = uniqueResolutions.Count - 1;
            }
        }

        resolutions = uniqueResolutions.ToArray();

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);

        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void SetupDisplayMode()
    {
        displayModeDropdown.ClearOptions();
        var options = new System.Collections.Generic.List<string>
        {
            "Fullscreen",
            "Borderless",
            "Windowed"
        };

        displayModeDropdown.AddOptions(options);
        displayModeDropdown.RefreshShownValue();
    }

    private void SetupFrameRate()
    {
        frameRateDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>
        {
            "30 FPS",
            "60 FPS",
            "120 FPS",
            "144 FPS",
            "240 FPS"
        };

        frameRateDropdown.AddOptions(options);
        frameRateDropdown.RefreshShownValue();
    }

    private void SetupBrightness()
    {
        GameObject brightnessObject =
        GameObject.Find("RuntimeBrightnessVolume");

        if (brightnessObject == null)
        {
            brightnessObject =
                new GameObject("RuntimeBrightnessVolume");
        }

        brightnessVolume =
            brightnessObject.GetComponent<Volume>();

        if (brightnessVolume == null)
        {
            brightnessVolume =
                brightnessObject.AddComponent<Volume>();
        }

        brightnessVolume.isGlobal = true;
        brightnessVolume.priority = 100f;

        if (brightnessVolume.profile == null)
        {
            brightnessVolume.profile =
                ScriptableObject.CreateInstance<VolumeProfile>();
        }

        if (!brightnessVolume.profile.TryGet(out colorAdjustments))
        {
            colorAdjustments =
                brightnessVolume.profile.Add<ColorAdjustments>(true);
        }

        colorAdjustments.active = true;
        colorAdjustments.postExposure.overrideState = true;
    }

    public void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];

        Screen.SetResolution(
            resolution.width,
            resolution.height,
            Screen.fullScreenMode);

        PlayerPrefs.SetInt("Resolution", index);
        PlayerPrefs.Save();
    }

    public void SetDisplayMode(int index)
    {
        switch (index)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }

        PlayerPrefs.SetInt("DisplayMode", index);
        PlayerPrefs.Save();
    }

    public void SetVSync(bool enabled)
    {
        QualitySettings.vSyncCount = enabled ? 1 : 0;

        if (frameRateDropdown != null)
            frameRateDropdown.interactable = !enabled;

        PlayerPrefs.SetInt("VSync", enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetFrameRate(int index)
    {
        switch (index)
        {
            case 0:
                Application.targetFrameRate = 30;
                break;
            case 1:
                Application.targetFrameRate = 60;
                break;
            case 2:
                Application.targetFrameRate = 120;
                break;
            case 3:
                Application.targetFrameRate = 144;
                break;
            case 4:
                Application.targetFrameRate = 240;
                break;
            default:
                Application.targetFrameRate = -1; // Default to platform's default frame rate
                break;
        }

        PlayerPrefs.SetInt("FrameRate", index);
        PlayerPrefs.Save();
    }

    public void SetBrightness(float value)
    {
        brightnessValue.text = Mathf.RoundToInt(value) + "%";

        if (colorAdjustments != null)
        {
            float exposure = Mathf.Lerp(-2f, 2f, value / 100f);
            colorAdjustments.postExposure.value = exposure;
        }

        PlayerPrefs.SetFloat("Brightness", value);
        PlayerPrefs.Save();
    }   
}
