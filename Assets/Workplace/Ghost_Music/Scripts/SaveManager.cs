using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [Header("Player")]
    public GameObject player;
    public HealthSystem healthSystem;
    public StaminaController staminaController;

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

        FindPlayer(); 

        if (player == null)
        {
            Debug.LogError("Save Failed: Player not found.");
            return;
        }


        // Create save data object
        SaveGame data = new SaveGame();

        data.sceneName =
            SceneManager.GetActiveScene().name;

        data.playerPosX =
           player.transform.position.x;

        data.playerPosY =
            player.transform.position.y;

        data.playerPosZ =
            player.transform.position.z;

        if (healthSystem != null)
        {
            data.playerHealth =
                healthSystem.CurrentHealth;
        }
        else
        {
            Debug.LogWarning(
                "Save Warning: HealthSystem not found."
            );
        }

        if (staminaController != null)
        {
            data.playerStamina =
                staminaController.Current;
        }
        else
        {
            Debug.LogWarning(
                "Save Warning: StaminaController not found."
            );
        }



        string json =
           JsonUtility.ToJson(data, true);

        File.WriteAllText(
          savePath,
          json
      );


        Debug.Log("=================================");
        Debug.Log("GAME SAVED");
        Debug.Log("=================================");
        Debug.Log("Scene: " + data.sceneName);
        Debug.Log("Health: " + data.playerHealth);
        Debug.Log("Stamina: " + data.playerStamina);
        Debug.Log("Save Location: " + savePath);
        Debug.Log("=================================");
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("No save file found.");
            return;
        }


        // Read save file
        string json =
            File.ReadAllText(savePath);


        // Convert JSON back into SaveGame
        SaveGame data =
            JsonUtility.FromJson<SaveGame>(json);


        if (data == null)
        {
            Debug.LogError(
                "Load Failed: Save data is invalid."
            );

            return;
        }


        // Make sure the scene name exists
        if (string.IsNullOrEmpty(data.sceneName))
        {
            Debug.LogError(
                "Load Failed: No scene name in save file."
            );

            return;
        }


        // Wait until the new scene finishes loading
        SceneManager.sceneLoaded += OnSceneLoaded;


        // Load saved scene
        SceneManager.LoadScene(data.sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Stop listening for scene loads
        SceneManager.sceneLoaded -= OnSceneLoaded;


        // Find player in the newly loaded scene
        FindPlayer();

        if (player == null)
        {
            Debug.LogError(
                "Load Failed: Player not found."
            );

            return;
        }


        // Read save file again
        string json =
            File.ReadAllText(savePath);


        SaveGame data =
            JsonUtility.FromJson<SaveGame>(json);


        if (data == null)
        {
            Debug.LogError(
                "Load Failed: Save data is invalid."
            );

            return;
        }

        player.transform.position =
            new Vector3(
                data.playerPosX,
                data.playerPosY,
                data.playerPosZ
            );


        if (healthSystem != null)
        {
            healthSystem.SetHealth(
                data.playerHealth
            );
        }
        else
        {
            Debug.LogWarning(
                "Load Warning: HealthSystem not found."
            );
        }

        if (staminaController != null)
        {
            staminaController.SetStamina(
                data.playerStamina
            );
        }
        else
        {
            Debug.LogWarning(
                "Load Warning: StaminaController not found."
            );
        }



        Debug.Log("=================================");
        Debug.Log("GAME LOADED");
        Debug.Log("=================================");
        Debug.Log("Scene: " + data.sceneName);
        Debug.Log("Health: " + data.playerHealth);
        Debug.Log("Stamina: " + data.playerStamina);
        Debug.Log("=================================");
    }

    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if(player != null)
        {
            healthSystem = player.GetComponent<HealthSystem>();
            staminaController = player.GetComponent<StaminaController>(); 
        }
        else
        {
            healthSystem = null;
            staminaController = null; 
        }
    }
}
