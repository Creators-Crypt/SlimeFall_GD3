using UnityEngine;

public class CreditsAutoOpen : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject creditsMenu;

    private void Start()
    {
        if (PlayerPrefs.GetInt("OpenCreditsOnLoad", 0) != 1)
            return;

        PlayerPrefs.DeleteKey("OpenCreditsOnLoad");
        PlayerPrefs.Save();

        if (mainMenu != null)
            mainMenu.SetActive(false);

        if (creditsMenu != null)
            creditsMenu.SetActive(true);
    }
}
