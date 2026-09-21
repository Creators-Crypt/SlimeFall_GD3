using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{

    public static event Action<GameStage> OnStageChanged;
    public static event Action<string> OnPlayerAction;

    [SerializeField] private GameStage currentStage;
    public GameStage GameStage { get { return currentStage; } }
    public enum GameState
    {
        Playing,
        Paused,
        Won,
        Lost
    }

    public enum SettingsReturnmenu
    {
        Pause,
        Win,
        Lose
    }

    private SettingsReturnmenu settingsReturnMenu;

    public GameState currentState = GameState.Playing;

    [Header("UI")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject lossMenu;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject settingsMenu;

    [SerializeField] private CameraController cameraController;

    protected override void Awake()
    {
        base.Awake();
    }
    private void OnEnable() {
        FindCameraController();
    }
    private void OnDisable() {
        cameraController = null;
    }
    private void Start()
    {
        FindUIReferences();
        AttachGamePlaybuttons();
        FindCameraController();

        Time.timeScale = 1f;
        currentState = GameState.Playing;
        SetStage(GameStage.HomeBase_Tut_Spawn);

        hud.SetActive(true);
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        winMenu.SetActive(false);
        lossMenu.SetActive(false);

        HideCursor();
    }

    private void Update()
    {
        // TEMPORARY UI TEST KEYS
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SetWin();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            SetLose();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing)
            {
                PauseGame();
            }
            else if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
        }
    }

    public void SetWin()
    {
        currentState = GameState.Won;

        hud.SetActive(false);
        winMenu.SetActive(true);

        if (cameraController != null)
            cameraController.enabled = false;

        ShowCursor();
        Time.timeScale = 0f;
    }

    public void SetLose()
    {
        currentState = GameState.Lost;

        hud.SetActive(false);
        lossMenu.SetActive(true);

        if (cameraController != null)
            cameraController.enabled = false;

        ShowCursor();
        Time.timeScale = 0f;
    }

    public void PauseGame()
    {
        currentState = GameState.Paused;

        hud.SetActive(false);
        pauseMenu.SetActive(true);

        if (cameraController != null)
            cameraController.enabled = false;

        ShowCursor();
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        currentState = GameState.Playing;

        hud.SetActive(true);
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        winMenu.SetActive(false);
        lossMenu.SetActive(false);

        if (cameraController != null)
            cameraController.enabled = true;

        HideCursor();
        Time.timeScale = 1f;
    }

    public void QuitToMain()
    {
        ResetUIForSceneChanges();
        SceneManager.LoadScene("Menus");
    }

    private void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenSettingsFromPause()
    {
        settingsReturnMenu = SettingsReturnmenu.Pause;
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void OpenSettingsFromWin()
    {
        settingsReturnMenu = SettingsReturnmenu.Win;
        winMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void OpenSettingsFromLose()
    {
        settingsReturnMenu = SettingsReturnmenu.Lose;
        lossMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void ReturnFromSettings()
    {
        settingsMenu.SetActive(false);

        switch (settingsReturnMenu)
        {
            case SettingsReturnmenu.Pause:
                pauseMenu.SetActive(true);
                break;
            case SettingsReturnmenu.Win:
                winMenu.SetActive(true);
                break;
            case SettingsReturnmenu.Lose:
                lossMenu.SetActive(true);
                break;
        }
    }

    public void SetStage(GameStage newState)
    {

        currentStage = newState;
        OnStageChanged?.Invoke(currentStage);
    }

    public void PlayerPerformAction(string actionKey)
    {

        OnPlayerAction?.Invoke(actionKey);
    }

    public void RespawnGame()
    {        
        ResetUIForSceneChanges();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlayAgain()
    {        
        ResetUIForSceneChanges();
        SceneManager.LoadScene("Showcase_Homebase");
    }

    private void FindUIReferences()
    {
        Transform[] allChildren = transform.root.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in allChildren)
        {
            switch (child.name)
            {
                case "Pause":
                    pauseMenu = child.gameObject;
                    break;
                case "Win":
                    winMenu = child.gameObject;
                    break;
                case "Lose":
                    lossMenu = child.gameObject;
                    break;
                case "HUD":
                    hud = child.gameObject;
                    break;
                case "SettingsMenu":
                    settingsMenu = child.gameObject;
                    break;
            }
        }
    }

    private void AttachGamePlaybuttons()
    {
        Button[] buttons = transform.root.GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            if (pauseMenu!= null && button.transform.IsChildOf(pauseMenu.transform))
            {
                switch (button.name)
                {
                    case "ResumeButton":
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(ResumeGame);
                        break;
                    case "SettingsButton":
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(OpenSettingsFromPause);
                        break;
                    case "QuitToMain":
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(QuitToMain);
                        break;
                }

                continue;
            }

            if (settingsMenu != null && button.transform.IsChildOf(settingsMenu.transform))
            {
                if (button.name == "BackButton")
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(ReturnFromSettings);
                }
            }

            if (winMenu != null && button.transform.IsChildOf(winMenu.transform))
            {
                switch (button.name)
                {
                    case "ResumeButton":
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(ResumeGame);
                        break;

                    case "SettingsButton":
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(OpenSettingsFromWin);
                        break;

                    case "QuitToMain":
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(QuitToMain);
                        break;
                }
            }

            if (lossMenu != null && button.transform.IsChildOf(lossMenu.transform))
            {
                switch (button.name)
                {
                    case "RetryButton":
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(RespawnGame);
                        Debug.Log("Testing button");
                        break;

                    case "SettingsButton":
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(OpenSettingsFromLose);
                        break;

                    case "QuitToMain":
                        button.onClick.RemoveAllListeners();
                        button.onClick.AddListener(QuitToMain);
                        break;
                }
            }
        }
    }

    private void ResetUIForSceneChanges()
    {
        Time.timeScale = 1f;
        currentState = GameState.Playing;

        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        if (settingsMenu != null)
            settingsMenu.SetActive(false);

        if(winMenu != null)
            winMenu.SetActive(false);

        if (lossMenu != null)
            lossMenu.SetActive(false);

        if (hud != null)
            hud.SetActive(true);

        HideCursor();
    }

    private void FindCameraController()
    {
        cameraController = FindFirstObjectByType<CameraController>(FindObjectsInactive.Include);

        if (cameraController == null)
        {
            Debug.LogWarning("GameManager could not find a CameraController in this scene.");
        }
    }
}