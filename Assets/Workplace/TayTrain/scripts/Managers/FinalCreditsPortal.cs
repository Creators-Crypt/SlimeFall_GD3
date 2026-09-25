using UnityEngine;
//using UnityEngine.SceneManagement;

public class FinalCreditsPortal : MonoBehaviour
{
    private bool activated;

    private void OnTriggerEnter(Collider other)
    {
        if (activated || !other.CompareTag("Player"))
            return;

        activated = true;
        GameManager.Instance.SetWin();
    }
    //[SerializeField] private string menuSceneName = "Menus 1";

    //private bool activated;

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (activated || !other.CompareTag("Player"))
    //        return;

    //    activated = true;

    //    PlayerPrefs.SetInt("OpenCreditsOnLoad", 1);
    //    PlayerPrefs.Save();

    //    SceneManager.LoadScene(menuSceneName);
    //}
}
