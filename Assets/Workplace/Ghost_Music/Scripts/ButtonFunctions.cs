using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    public void Continue() {
        Resume();
    }

    public void NewGame() {
        Restart(); 
    }

    public void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); 
#endif
    }

    public void Resume() {
        GameManager.Instance.ResumeGame();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.Instance.ResumeGame();
    }

    public void Save()
    {
        
    }
}