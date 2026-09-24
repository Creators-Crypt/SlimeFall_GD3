using System;
using Unity.VisualScripting;
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

    private bool isGamePlaySceneReady = false;

    [Header("UI")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject lossMenu;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject settingsMenu;

    [SerializeField] private CameraController cameraController;
    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoad;
    }
    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }
    private void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        if (mode == LoadSceneMode.Additive) return;

        Debug.Log($"<color=cyan>[GameManager] OnSceneLoad starting for scene: '{scene.name}'</color>");

        isGamePlaySceneReady = false;

        // Trace references right before clearing them
        Debug.Log($"[GameManager] UI State BEFORE ClearReferences:\n" +
                  $"- pauseMenu: {(pauseMenu != null ? "ASSIGNED" : "NULL/DESTROYED")}\n" +
                  $"- hud: {(hud != null ? "ASSIGNED" : "NULL/DESTROYED")}");

        ClearReferences();

        Debug.Log($"[GameManager] UI State AFTER ClearReferences:\n" +
                  $"- pauseMenu: {(pauseMenu != null ? "ASSIGNED" : "NULL/DESTROYED")}\n" +
                  $"- hud: {(hud != null ? "ASSIGNED" : "NULL/DESTROYED")}");

        Time.timeScale = 1f;
        currentState = GameState.Playing;

        if (scene.name == "Menus_1") {
            ShowCursor();
        } else {
            HideCursor();
        }
    }
    protected override void Awake() { base.Awake(); }
    private void Update()
    {
        /*        // TEMPORARY UI TEST KEYS
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    SetWin();
                }

                if (Input.GetKeyDown(KeyCode.F2))
                {
                    SetLose();
                }*/

        if (isGamePlaySceneReady && pauseMenu == null) {
            Debug.LogWarning("[GameManager] isGameplaySceneReady was true, but UI references are missing! Forcing ready state to false.");
            isGamePlaySceneReady = false;
        }

        if (!isGamePlaySceneReady) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LogCurrentUIStatus();

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
    public void RegisterCameraController(CameraController camera) {
        cameraController = camera;

        if (cameraController != null) {

            cameraController.enabled = true;
            Debug.Log("<color=green>CameraController registered successfully to the active GameManager instance!</color>");
        }
        
    }
    public void ConfigureGameplayState() {

        Time.timeScale = 1f;
        currentState = GameState.Playing;
        SetStage(GameStage.HomeBase_Tut_Spawn);

        if (hud != null) hud.SetActive(true);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (settingsMenu != null) settingsMenu.SetActive(false);
        if (winMenu != null) winMenu.SetActive(false);
        if (lossMenu != null) lossMenu.SetActive(false);

        HideCursor();

        isGamePlaySceneReady = true;
    }
    public void SetWin()
    {
        currentState = GameState.Won;

        if (hud != null) hud.SetActive(false);
        if (winMenu != null) winMenu.SetActive(true);

        if (cameraController != null)
            cameraController.enabled = false;

        ShowCursor();
        Time.timeScale = 0f;
    }

    public void SetLose()
    {
        currentState = GameState.Lost;

        if (hud != null) hud.SetActive(false);
        if (lossMenu != null) lossMenu.SetActive(true);

        if (cameraController != null)
            cameraController.enabled = false;

        ShowCursor();
        Time.timeScale = 0f;
    }

    public void PauseGame()
    {
        if (pauseMenu == null) {
            Debug.LogError("[GameManager] Cannot Pause! pauseMenu reference is physically missing/destroyed.");
            return;
        }

        currentState = GameState.Paused;

        if (hud != null) hud.SetActive(false);
        pauseMenu.SetActive(true);

        if (cameraController != null)
            cameraController.enabled = false;

        ShowCursor();
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        currentState = GameState.Playing;

        if (hud != null) hud.SetActive(true);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (settingsMenu != null) settingsMenu.SetActive(false);
        if (winMenu != null) winMenu.SetActive(false);
        if (lossMenu != null) lossMenu.SetActive(false);

        if (cameraController != null)
            cameraController.enabled = true;

        HideCursor();
        Time.timeScale = 1f;
    }

    public void QuitToMain()
    {

        ResetUIForSceneChanges();
        SceneManager.LoadScene("Menus_1");
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
        SceneManager.LoadScene("Showcase_MainArea");
    }
    private void ClearReferences() {

        cameraController = null;
        pauseMenu = null;
        winMenu = null;
        lossMenu = null;
        hud = null;
        settingsMenu = null;

        OnStageChanged = null;
        OnPlayerAction = null;
    }
    public void InitializeSceneUI(SceneUIConnection connector, CameraController camera) {

        Debug.Log("<color=orange>[GameManager] InitializeSceneUI invoked by GameInitializer. Attempting reassignment...</color>");
        cameraController = camera;
        if (cameraController != null) {
            cameraController.enabled = true;
        }

        if (connector == null) {
            Debug.LogError("[GameManager] InitializeSceneUI failed: The connector parameter is null.");
            return;
        }

        // Assign references
        pauseMenu = connector.pauseMenu;
        winMenu = connector.winMenu;
        lossMenu = connector.lossMenu;
        hud = connector.hud;
        settingsMenu = connector.settingsMenu;

        // Log exactly what was found inside the connector at assignment time
        Debug.Log($"[GameManager] UI Assigned from {connector.gameObject.name}:\n" +
                  $"- pauseMenu: {(pauseMenu != null ? pauseMenu.name : "NULL")}\n" +
                  $"- winMenu: {(winMenu != null ? winMenu.name : "NULL")}\n" +
                  $"- lossMenu: {(lossMenu != null ? lossMenu.name : "NULL")}\n" +
                  $"- hud: {(hud != null ? hud.name : "NULL")}\n" +
                  $"- settingsMenu: {(settingsMenu != null ? settingsMenu.name : "NULL")}");

        AttachGamePlayButtonsFromContext(connector);
    }
    public void LogCurrentUIStatus() {
        Debug.Log($"[GameManager] UI Status Check (Escape Pressed):\n" +
                  $"- pauseMenu: {(pauseMenu != null ? "ALIVE (" + pauseMenu.name + ")" : "DESTROYED / NULL")}\n" +
                  $"- winMenu: {(winMenu != null ? "ALIVE (" + winMenu.name + ")" : "DESTROYED / NULL")}\n" +
                  $"- lossMenu: {(lossMenu != null ? "ALIVE (" + lossMenu.name + ")" : "DESTROYED / NULL")}\n" +
                  $"- hud: {(hud != null ? "ALIVE (" + hud.name + ")" : "DESTROYED / NULL")}\n" +
                  $"- settingsMenu: {(settingsMenu != null ? "ALIVE (" + settingsMenu.name + ")" : "DESTROYED / NULL")}");
    }
    private void AttachGamePlayButtonsFromContext(SceneUIConnection connector) { 
        
        Button[] buttons = connector.GetComponentsInChildren<Button>(true); 
        
        foreach (Button button in buttons) { 
            if (pauseMenu != null && button.transform.IsChildOf(pauseMenu.transform)) { 
                switch (button.name) { 
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
            if (settingsMenu != null && button.transform.IsChildOf(settingsMenu.transform)) { 
                if (button.name == "BackButton") { 
                    button.onClick.RemoveAllListeners(); 
                    button.onClick.AddListener(ReturnFromSettings); } } 
            if (winMenu != null && button.transform.IsChildOf(winMenu.transform)) { 
                switch (button.name) { 
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
                        button.onClick.AddListener(QuitToMain); break; } } 
            if (lossMenu != null && button.transform.IsChildOf(lossMenu.transform)) { 
                switch (button.name) { 
                    case "RetryButton": 
                        button.onClick.RemoveAllListeners(); 
                        button.onClick.AddListener(RespawnGame); 
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
/*    private void FindUIReferences()
    {
        if (SceneManager.GetActiveScene().name == "Menus_1") return;

        Canvas UICanvas = FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);
        Debug.Log($"I have found the following Canvas: {UICanvas.name}");
        Transform[] allChildren = UICanvas.GetComponentsInChildren<Transform>(true);

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
        if (SceneManager.GetActiveScene().name == "Menus_1") return;

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
    }*/

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
        if (cameraController != null && cameraController.Equals(null)) cameraController = null;
        if (cameraController == null) {
            cameraController = FindFirstObjectByType<CameraController>(FindObjectsInactive.Include);
        }
        if (cameraController == null)
        {
            Debug.LogWarning("GameManager could not find a CameraController in this scene.");
        }
    }
}