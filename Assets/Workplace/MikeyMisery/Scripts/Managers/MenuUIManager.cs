using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject videoPanel;
    [SerializeField] private GameObject interfacePanel;
    [SerializeField] private GameObject accessibilityPanel;

    [Header("Loading")]
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TMP_Text loadingProgress;

    private bool settingsOpenFromPause = false;

    public void ShowMainMenu()
    {
        HideAllMenus();
        mainMenuPanel.SetActive(true);
    }

    public void ShowSettings()
    {
        settingsOpenFromPause = false;
        HideAllMenus();
        settingsPanel.SetActive(true);
    }

    public void ShowSettingsFromPause()
    {
        settingsOpenFromPause = true;
        HideAllMenus();
        settingsPanel.SetActive(true);
    }

    public void BackFromSettings()
    {
        if (settingsOpenFromPause)
        {
            HideAllMenus();

        }
        else
        {
            ShowMainMenu();
        }
    }

    public void ShowCredits()
    {
        HideAllMenus();
        creditsPanel.SetActive(true);
    }

    public void ShowLoading()
    {
        HideAllMenus();
        loadingPanel.SetActive(true);
    }

    public void ShowAudio()
    {
        HideSettingsPanels();
        audioPanel.SetActive(true);
    }

    public void ShowVideo()
    {
        HideSettingsPanels();
        videoPanel.SetActive(true);
    }

    public void ShowInterface()
    {
        HideSettingsPanels();
        interfacePanel.SetActive(true);
    }

    public void ShowAccessibility()
    {
        HideSettingsPanels();
        accessibilityPanel.SetActive(true);
    }

    public void HideAllMenus()
    {
        mainMenuPanel.SetActive(false);
        loadingPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void HideSettingsPanels()
    {
        audioPanel.SetActive(false);
        videoPanel.SetActive(false);
        interfacePanel.SetActive(false);
        accessibilityPanel.SetActive(false);
    }

    public void StartGame()
    {
        ShowLoading();
        StartCoroutine(LoadGameAsync());
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private IEnumerator LoadGameAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("TheOriginalDeveloper");

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.98f);

            loadingBar.value = progress;
            loadingProgress.text = Mathf.RoundToInt(progress * 100f) + "%";

            yield return null;
        }
    }
}
