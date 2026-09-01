using UnityEngine;

public class CastleState : MonoBehaviour {

    [SerializeField] private GameObject[] explorableHomes;
    [SerializeField] private GameObject portal;

    private int numberOfHomes = 0;
    private int requiredPlaces = 3;

    private bool isExploring = false;

    public int SetHomesVisited(int value) => numberOfHomes += value;

    private void Start() {

        foreach (var item in explorableHomes) {
            item.gameObject.SetActive(true);
        }
    }
    private void Update() {

        if (!isExploring) return;

        if(numberOfHomes >= requiredPlaces) {
            portal.SetActive(true);
            ObjectiveManager.Instance.SetObjective("Enter through the portal");
        }
        ObjectiveManager.Instance.SetObjective($"Explored Homes: {numberOfHomes} / {requiredPlaces}");
    }
    private void OnTriggerEnter(Collider other) {
        
        if (other.CompareTag("Player")) {
            if (GameManager.Instance.GameStage == GameStage.Castle_Explore) {
                ObjectiveManager.Instance.SetObjective($"Explored Homes: {numberOfHomes} / {requiredPlaces}");
                isExploring = true;

                GetComponent<Collider>().enabled = false;
            }
        }
    }
}