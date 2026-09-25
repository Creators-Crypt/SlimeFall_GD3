using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : Singleton<GameInitializer> {

    public static event Action OnSceneSetupComplete;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerTransform;

    public Transform GetPlayerTransform => playerTransform;

    protected override void Awake() {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        
        if (mode == LoadSceneMode.Additive) return;
        CleanAudioListeners();
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Menus_1") return;

        string prevStatus = playerTransform == null ? "null" : (playerTransform ? "Valid" : "Missing (Destroyed but not nullified)");
        Debug.Log($"[GameInitializer] OnSceneLoaded triggered for '{scene.name}'. Previous playerTransform state: {prevStatus}");

        Debug.Log($"<color=yellow>[GameInitializer] OnSceneLoaded began executing for scene: {scene.name}</color>");

        playerTransform = null;

        SpawnPoint spawn = FindFirstObjectByType<SpawnPoint>();
        Debug.Log($"[GameInitializer] SpawnPoint search result: {(spawn != null ? $"FOUND at {spawn.transform.position}" : "NOT FOUND / NULL")}");
        PlayerController activePlayer = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Exclude);

        if (activePlayer != null) {
            playerTransform = activePlayer.transform;
            Debug.Log("[GameInitializer] Found and linked an existing PlayerController from the scene hierarchy.");
        }

        // 2. UNCONDITIONAL FALLBACK: If playerTransform is still null, force instantiate the prefab!
        if (playerTransform == null) {
            if (spawn != null && playerPrefab != null) {
                Debug.Log("[GameInitializer] No player found. Instantiating a fresh player prefab asset at the spawn point.");
                GameObject newPlayer = Instantiate(playerPrefab, spawn.transform.position, spawn.transform.rotation);
                playerTransform = newPlayer.transform;
            } else {
                // This will tell us instantly if an asset is unassigned in your inspector panel
                Debug.LogError($"[GameInitializer] CRITICAL: Cannot spawn player! SpawnPoint is {(spawn != null ? "VALID" : "NULL")}. PlayerPrefab asset is {(playerPrefab != null ? "VALID" : "NULL")}.");
            }
        }

        Debug.Log($"[GameInitializer] Post-Find Check. activePlayer found: {activePlayer != null}. playerTransform assigned: {playerTransform != null}");

        if (spawn != null && playerTransform != null) {
            SpawnPlayer(spawn.transform.position, spawn.transform.rotation);

            var enemySpawner = FindFirstObjectByType<Spawner>(FindObjectsInactive.Include);
            if (enemySpawner != null) {
                enemySpawner.InitializeSpawner(playerTransform);
            } else {
                Debug.Log("[GameInitializer] No EnemySpawner found in this scene to initialize.");
            }

            CameraController playerCamera = playerTransform.GetComponentInChildren<CameraController>();
            if (playerCamera != null) {
                playerCamera.InitializeAndRegister();

                SceneUIConnection sceneUI = FindFirstObjectByType<SceneUIConnection>();

                if (GameManager.Instance != null) {
                    GameManager.Instance.InitializeSceneUI(sceneUI, playerCamera);
                    GameManager.Instance.ConfigureGameplayState();
                }
                OnSceneSetupComplete?.Invoke();
            } else {
                Debug.LogError(" [GameInitializer] Player spawned, but no CameraController found nested inside its hierarchy!");
            }
        } else {
            Debug.Log($" [GameInitializer] Scene '{scene.name}' loaded without a gameplay player character.");
        }
        Physics.SyncTransforms();
    }
    private void SpawnPlayer(Vector3 position, Quaternion rotation) {

        if (playerTransform.TryGetComponent<CharacterController>(out var character)) {
            character.enabled = false;
            
            if (playerTransform.TryGetComponent<PlayerController>(out var player)) {
                player.ResetVelocity();
                player.transform.position = new Vector3(player.transform.position.x, 5f, player.transform.position.z);
            }
            playerTransform.SetPositionAndRotation(position, rotation);
            Physics.SyncTransforms();

            character.enabled = true;
            Physics.SyncTransforms();
        }
    }
    private void CleanAudioListeners() {

        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);

        string currentScene = SceneManager.GetActiveScene().name;

        AudioListener managerListener = GetComponent<AudioListener>();
        
        if (currentScene == "Menus_1") {
            
            if (managerListener != null) managerListener.enabled = true;

            foreach (var listener in listeners) {
                if (!listener.transform.IsChildOf(this.transform)) {
                    Destroy(listener);
                }
            }
        } else {

            if (managerListener != null) managerListener.enabled = false;

            foreach (var listener in listeners) {

                if (listener.transform.IsChildOf(this.transform)) continue;

                if (listeners.Length > 2 && !listener.gameObject.CompareTag("MainCamera")) {
                    Destroy(listener);
                    Debug.Log("[GameInitializer] Cleaned up duplicate scene AudioListener component.");
                }
            }
        }
    }
}