using UnityEngine;

public class TutorialTrigger : MonoBehaviour {

    [SerializeField] private Spawner tutorialSpawner;
    [SerializeField] private GameObject tutorialCage;
    [SerializeField] private TutorialProgress tutorialProgress;

    private void OnTriggerEnter(Collider other) {
        
        if (other.CompareTag("Player")) {

            GameManager.Instance.SetStage(GameStage.Intro_TutorialTrapped);
            TrapPlayerInArena();
            SpawnSlimes();
            tutorialProgress.StartTutorial();
        }
    }

    private void TrapPlayerInArena() {
        tutorialCage.SetActive(true);
    }

    private void SpawnSlimes() {
        if(tutorialSpawner != null) {
            tutorialSpawner.gameObject.SetActive(true);
        }   
    }
}