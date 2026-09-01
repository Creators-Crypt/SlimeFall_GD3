using UnityEngine;
using UnityEngine.UI;

public class SpellDisplayUI : MonoBehaviour
{
    [SerializeField] private Outline spellSlot1Outline;
    [SerializeField] private Outline spellSlot2Outline;
    [SerializeField] private Outline spellSlot3Outline;
    [SerializeField] private Outline spellSlot4Outline;

    private void Start() {
        // Initialize the first spell slot as selected
        SelectSpell(0);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectSpell(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectSpell(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SelectSpell(2);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SelectSpell(3);
    }

    private void SelectSpell(int selectedSlot) {
        spellSlot1Outline.enabled = selectedSlot == 0;
        spellSlot2Outline.enabled = selectedSlot == 1;
        spellSlot3Outline.enabled = selectedSlot == 2;
        spellSlot4Outline.enabled = selectedSlot == 3;
    }
}