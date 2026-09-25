using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private const string GameSceneName = "Showcase_MainArea";

    private GameObject mainPanel;
    private GameObject settingsPanel;
    private GameObject creditsPanel;
    private GameObject loadingPanel;
    private GameObject audioPanel;
    private GameObject videoPanel;

    private bool isLoading;

    private void OnEnable() {
        GameInitializer.OnSceneSetupComplete += HandleSceneReady;
    }
    private void OnDisable() {
        GameInitializer.OnSceneSetupComplete -= HandleSceneReady;
    }
    private void Awake()
    {
        mainPanel = transform.Find("Canvas/MainMenu").gameObject;
        settingsPanel = transform.Find("Canvas/SettingsMenu").gameObject;
        creditsPanel = transform.Find("Canvas/CreditsMenu").gameObject;
        loadingPanel = transform.Find("Canvas/LoadingScreen").gameObject;

        const string settingsLayout =
            "Canvas/SettingsMenu/SettingsLayout/";

        audioPanel = transform.Find(
            settingsLayout + "ContentPanel/AudioPanel").gameObject;

        videoPanel = transform.Find(
            settingsLayout + "ContentPanel/VideoPanel").gameObject;

        const string menuButtons = "Canvas/MainMenu/MenuButtons/";
        const string sidebarButtons =
            settingsLayout + "Sidebar/SidebarButtons/";

        BindButton(menuButtons + "NewGameButton", NewGame);
        BindButton(menuButtons + "SettingsButton", ShowSettings);
        BindButton(menuButtons + "CreditsButton", ShowCredits);
        BindButton(menuButtons + "QuitButton", QuitGame);

        BindButton(sidebarButtons + "AudioButton", ShowAudio);
        BindButton(sidebarButtons + "VideoButton", ShowVideo);
        BindButton(sidebarButtons + "BackButton", ShowMainMenu);

        BindButton("Canvas/CreditsMenu/BackButton", ShowMainMenu);
    }

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowAudio();
        ShowMainMenu();
    }
    private void HandleSceneReady() {
        Debug.Log("<color=green>[MainMenu] Level setup finished. Deactivating loading screen!</color>");
        isLoading = false;

        if (loadingPanel != null) {
            loadingPanel.SetActive(false);
        }
    }

    private void BindButton(string path, UnityAction action)
    {
        Button button = transform.Find(path).GetComponent<Button>();
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(action);
    }

    private void ShowScreen(GameObject screen)
    {
        mainPanel.SetActive(screen == mainPanel);
        settingsPanel.SetActive(screen == settingsPanel);
        creditsPanel.SetActive(screen == creditsPanel);
        loadingPanel.SetActive(screen == loadingPanel);
    }

    public void ShowMainMenu()
    {
        if (isLoading) return;

        ShowScreen(mainPanel);
    }

    public void ShowSettings()
    {
        if (isLoading) return;

        ShowScreen(settingsPanel);
        ShowAudio();
    }

    public void ShowCredits()
    {
        if (isLoading) return;

        ShowScreen(creditsPanel);
    }

    public void ShowAudio()
    {
        if (isLoading) return;

        audioPanel.SetActive(true);
        videoPanel.SetActive(false);
    }

    public void ShowVideo()
    {
        if (isLoading) return;

        audioPanel.SetActive(false);
        videoPanel.SetActive(true);
    }

    public void NewGame()
    {
        if (isLoading) return;

        isLoading = true;
        ShowScreen(loadingPanel);
        Time.timeScale = 1f;

        SceneManager.LoadScene(GameSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}