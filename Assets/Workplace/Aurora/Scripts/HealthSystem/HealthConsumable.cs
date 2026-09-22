using UnityEngine;

public class HealthConsumable : MonoBehaviour, IInteractable {
    public string InteractionPrompt => "Press E To Consume";

    public void Interact() {

        GameObject target = GameObject.FindGameObjectWithTag("Player");

        if (target.TryGetComponent(out HealthSystem health)) {
            health.OnHeal(10f);
        }

        Destroy(gameObject);
    }
}