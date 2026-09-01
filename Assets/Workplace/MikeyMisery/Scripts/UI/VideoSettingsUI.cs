using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoSettingsUI : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown displayModeDropdown;
    [SerializeField] private Toggle vSyncToggle;
    [SerializeField] private TMP_Dropdown frameRateDropdown;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private TMP_Text brightnessValue;

    private Resolution[] resolutions;

    private void Start()
    {
        SetupResolution();
        int savedResolution = PlayerPrefs.GetInt("Resolution", resolutionDropdown.value);

        if (savedResolution >= 0 && savedResolution < resolutions.Length)
        {
            resolutionDropdown.value = savedResolution;
            resolutionDropdown.RefreshShownValue();
        }
        
        vSyncToggle.isOn = PlayerPrefs.GetInt("VSync", 1) == 1; // Default to VSync enabled
        frameRateDropdown.value = PlayerPrefs.GetInt("FrameRate", -1); // Default to platform's default frame rate
        frameRateDropdown.RefreshShownValue();
        displayModeDropdown.value = PlayerPrefs.GetInt("DisplayMode", 1); // Default to Borderless Fullscreen
        displayModeDropdown.RefreshShownValue();
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 100f); // Default to 100% brightness
        brightnessValue.text = Mathf.RoundToInt(brightnessSlider.value) + "%";

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        displayModeDropdown.onValueChanged.AddListener(SetDisplayMode);
        vSyncToggle.onValueChanged.AddListener(SetVSync);
        frameRateDropdown.onValueChanged.AddListener(SetFrameRate);
        brightnessSlider.onValueChanged.AddListener(SetBrightness);
    }

    private void SetupResolution()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];

        Screen.SetResolution(
            resolution.width,
            resolution.height,
            Screen.fullScreenMode);

        PlayerPrefs.SetInt("Resolution", index);
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
    }

    public void SetVSync(bool enabled)
    {
        QualitySettings.vSyncCount = enabled ? 1 : 0;
        PlayerPrefs.SetInt("VSync", enabled ? 1 : 0);
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
    }

    public void SetBrightness(float value)
    {
        brightnessValue.text = Mathf.RoundToInt(value) + "%";
        PlayerPrefs.SetFloat("Brightness", value);
    }   
}
