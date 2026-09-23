using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameInitializer : Singleton<GameInitializer> {

    [SerializeField] private Transform playerTransform;

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        
        if (mode == LoadSceneMode.Additive) return;

        CleanAudioListeners();
        //CleanEventSystem();

        SpawnPoint spawn = FindFirstObjectByType<SpawnPoint>();

        if (spawn != null && playerTransform != null)
            SpawnPlayer(spawn.transform.position, spawn.transform.rotation);

        Physics.SyncTransforms();
    }
    private void SpawnPlayer(Vector3 position, Quaternion rotation) {

        if (playerTransform.TryGetComponent<CharacterController>(out var character)) {
            character.enabled = false;
            playerTransform.SetPositionAndRotation(position, rotation);

            if (playerTransform.TryGetComponent<PlayerController>(out var player)) {
                player.ResetVelocity();
            }
            
            character.enabled = true;
        }
    }
    private void CleanEventSystem() {

        EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        foreach (var eventSystem in eventSystems) {

            if (!eventSystem.transform.IsChildOf(this.transform)) Destroy(eventSystem.gameObject);
        }
    }
    private void CleanAudioListeners() {

        AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        foreach (var listener in listeners) {
            if (!listener.transform.IsChildOf(this.transform)) Destroy(listener.gameObject);
        }
    }
}