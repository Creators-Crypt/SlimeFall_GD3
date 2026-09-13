using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerDeath : DeathHandler {

    [UnitHeaderInspectable("Death Presentation")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float fallDuration;
    [SerializeField] private float delayBeforeLoseScreen;

    private bool dying = false;

    protected override void Awake() {
        base.Awake();
    }
    protected override void OnEnable() {
        base.OnEnable();
    }
    protected override void OnDisable() {
        base.OnDisable();
    }
    protected override void HandleDeath() {
        if (dying)
            return;

        dying = true;

        StartCoroutine(DeathSequence());
       
    }

    private IEnumerator DeathSequence(){
        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0f,0f,90f);

        float elapsedTime = 0f;

        while (elapsedTime < fallDuration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / fallDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;

        yield return new WaitForSeconds(delayBeforeLoseScreen);

        if (GameManager.Instance != null)
            GameManager.Instance.SetLose();
    }
}