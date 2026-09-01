using UnityEngine;

public class MiniBoss : MonoBehaviour {

    [SerializeField] private GameObject spawnMiniBoss;
    
    private void OnTriggerEnter(Collider other) {

        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance.GameStage == GameStage.Castle_ForgotKey) {
            
            spawnMiniBoss.SetActive(true);

            GetComponent<Collider>().enabled = false;
        }
    }
}