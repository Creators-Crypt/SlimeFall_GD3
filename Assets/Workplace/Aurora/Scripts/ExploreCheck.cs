using UnityEngine;

public class ExploreCheck : MonoBehaviour {

    [SerializeField] private CastleState state;

    private void Start() {
        state = GetComponentInParent<CastleState>();
    }
    private void OnTriggerEnter(Collider other) {
        
        if (other.CompareTag("Player")) {
            state.SetHomesVisited(1);
        }
    }
}