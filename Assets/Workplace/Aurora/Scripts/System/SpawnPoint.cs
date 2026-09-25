using UnityEngine;

public class SpawnPoint : MonoBehaviour {

    [SerializeField] private string spawnPoint = "Player Spawn Position";
    public string SpawnPointPosition => spawnPoint;
}