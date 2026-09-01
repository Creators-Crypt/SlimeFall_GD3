using UnityEngine;
using UnityEngine.Playables;

public class BossSequenceManager : MonoBehaviour {
    
    [Header("Sequence Triggers")]
    public PlayableDirector sequenceTimeline;
    public GameObject keyObject;
    public Transform playerTransform;
    public float interactionDistance = 2.5f;

    [Header("Boss Settings")]
    public GameObject bossObject;
    public GameObject bossHealthBarUI;
    public float scaleSpeed = 1.5f;

    private bool sequenceStarted = false;
    private bool playerIsNearKey = false;
    private bool bossIsSpawning = false;
    private Vector3 targetScale = new(3f, 3f, 3f);

    void Start() {
        keyObject.SetActive(false);
        bossObject.SetActive(false);
        bossHealthBarUI.SetActive(false);
        bossObject.transform.localScale = Vector3.one;
    }
    public void StartBossSequence() {
        if (sequenceStarted) return;
        sequenceStarted = true;

        sequenceTimeline.Play();

        Invoke(nameof(SpawnKey), 1.0f);
    }

    private void SpawnKey() { keyObject.SetActive(true); }
    private void Update() {

        if (keyObject.activeSelf && !playerIsNearKey && !bossIsSpawning) {
            float distance = Vector3.Distance(playerTransform.position, keyObject.transform.position);
            if (distance <= interactionDistance) {
                TriggerBossSpawn();
            }
        }
        if (bossIsSpawning) {
            bossObject.transform.localScale = Vector3.Lerp(
                bossObject.transform.localScale,
                targetScale,
                Time.deltaTime * scaleSpeed
            );
            if (Vector3.Distance(bossObject.transform.localScale, targetScale) < 0.05f) {
                bossObject.transform.localScale = targetScale;
                bossIsSpawning = false;
            }
        }
    }
    private void TriggerBossSpawn() {
        playerIsNearKey = true;
        bossIsSpawning = true;

        bossObject.SetActive(true);
        bossHealthBarUI.SetActive(true); // Turns on the UI Slider/Bar
    }
}