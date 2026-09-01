using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public GameState currentState = GameState.Playing;

    [Header("UI")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject lossMenu;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject settingsMenu;

    [SerializeField] private CameraController cameraController;

    protected override void Awake() { 
        base.Awake();   
    }

    private void Start()
    {

        cameraController = Camera.main.GetComponent<CameraController>();
        Time.timeScale = 1f;
        currentState = GameState.Playing;
        currentStage = GameStage.Intro_Spawn;
        hud.SetActive(true);
        HideCursor();
    }

    private void Update()
    {
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
        cameraController.enabled = false;

        ShowCursor();
        Time.timeScale = 0f;
    }

    public void SetLose()
    {
        currentState = GameState.Lost;
        hud.SetActive(false);
        lossMenu.SetActive(true);
        cameraController.enabled = false;

        ShowCursor();
        Time.timeScale = 0f;
    }

    public void PauseGame()
    {
        currentState = GameState.Paused;
        hud.SetActive(false);
        pauseMenu.SetActive(true);
        cameraController.enabled = false;

        ShowCursor();
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        currentState = GameState.Playing;
        hud.SetActive(true);
        pauseMenu.SetActive(false);
        cameraController.enabled = true;

        HideCursor();
        Time.timeScale = 1f;
    }

    public void QuitToMain()
    {
        Time.timeScale = 1f;
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
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void ReturnToPauseFromSettings()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
    public void SetStage(GameStage newState) {

        currentStage = newState;
        OnStageChanged?.Invoke(currentStage);
    }
    public void PlayerPerformAction(string actionKey) {

        OnPlayerAction?.Invoke(actionKey);
    }
    public void RespawnGame()
    {
        /*currentState = GameState.Playing;

        lossMenu.SetActive(false);*/

        hud.SetActive(true);
        cameraController.enabled = true;

        HideCursor();
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void PlayAgain() {
        hud.SetActive(true);
        cameraController.enabled = true;

        HideCursor();
        Time.timeScale = 1f;
        SceneManager.LoadScene("TheOriginalDeveloper");
    }
}