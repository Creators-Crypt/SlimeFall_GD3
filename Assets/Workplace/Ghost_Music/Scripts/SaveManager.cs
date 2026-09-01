using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [Header("Player")]
    public GameObject player;
    public HealthSystem healthSystem;

    private string savePath;

    void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        savePath = Path.Combine(
            Application.persistentDataPath,
            "savegame.json"
        ); 
    }

    public void saveGame()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if(healthSystem == null && player != null)
        {
            healthSystem = player.GetComponent<HealthSystem>();
        }

        if(player == null)
        {
            Debug.LogError("Save Failed: Player not found");
            return;
        }

        if(healthSystem == null)
        {
            Debug.LogError("Save Failed: Health not found");
            return;
        }

        SaveGame data = new SaveGame();

        data.playerPosX = player.transform.position.x;
        data.playerPosY = player.transform.position.y;
        data.playerPosZ = player.transform.position.z;

        data.playerHealth = healthSystem.CurrentHealth;

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(savePath, json);

        Debug.Log("Game Saved");
        Debug.Log("Save Location: " + savePath);
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("No save file found.");
            return;
        }

        string json = File.ReadAllText(savePath);
        SaveGame data = JsonUtility.FromJson<SaveGame>(json);

        SceneManager.sceneLoaded += OnSceneLoaded;

        SceneManager.LoadScene(data.sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        player = GameObject.FindGameObjectWithTag("Player"); 

        if(player == null)
        {
            Debug.LogError("Load Failed: Player not found");
            return;
        }

        healthSystem = player.GetComponent<HealthSystem>();

        if(healthSystem = null)
        {
            Debug.LogError("Load failed: Health not found");
            return;
        }

        string json = File.ReadAllText(savePath);
        SaveGame data = JsonUtility.FromJson<SaveGame>(json);

        player.transform.position = new Vector3(
            data.playerPosX,
            data.playerPosY,
            data.playerPosZ
        );

        healthSystem.SetHealth(data.playerHealth);
    }    
}
