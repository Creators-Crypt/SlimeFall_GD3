using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SlidingDoor : Door {

    [SerializeField] private GameObject target;
    [SerializeField] private BoxCollider doorCollider;

    [SerializeField] private Vector3 currentPosition;
    [SerializeField] private Vector3 openPosition;

    [SerializeField] private bool hideDoor = false;

    public override string InteractionPrompt => (hideDoor) ? string.Empty : "Press Z to Open";
    private void Start() {
        
        doorCollider = GetComponentInChildren<BoxCollider>();
        currentPosition = target.transform.position;
    }
    public override void Interact() {

        StartCoroutine(OpenDoor());
    }

    private IEnumerator OpenDoor() {

        doorCollider.enabled = false;

        yield return new WaitForSeconds(0.1f);

        float duration = 1.0f;
        float elaspedTime = 0f;

        Vector3 startPosition = currentPosition;
        Vector3 endPosition = startPosition + openPosition;

        while (elaspedTime < duration) {

            target.transform.position = Vector3.Lerp(startPosition, endPosition, elaspedTime / duration);
            elaspedTime += Time.deltaTime;
            yield return null;
        }
        target.transform.position = endPosition;

        hideDoor = true;

        yield return new WaitForSeconds(0.1f);

        if (hideDoor) { target.SetActive(false); }

        yield return new WaitForSeconds(1f);
    }
}