using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    [Header("Settings Panels")]
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject videoPanel;

    private void Awake()
    {
        FindUIReferences();
        AttachSettingsButtons();
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

    public void HideSettingsPanels()
    {
        if (audioPanel != null)
            audioPanel.SetActive(false);

        if (videoPanel != null)
            videoPanel.SetActive(false);
    }

    private void FindUIReferences()
    {
        Transform[] allChildren = transform.root.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in allChildren)
        {
            switch (child.name)
            {
                case "AudioPanel":
                    audioPanel = child.gameObject;
                    break;
                case "VideoPanel":
                    videoPanel = child.gameObject;
                    break;
            }
        }
    }

    private void AttachSettingsButtons()
    {
        Button[] buttons = transform.root.GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            switch (button.name)
            {
                case "AudioButton":
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(ShowAudio);
                    break;
                case "VideoButton":
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(ShowVideo);
                    break;
            }
        }
    }
}
