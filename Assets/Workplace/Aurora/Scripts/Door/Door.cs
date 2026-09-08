using UnityEngine;

public class Door : MonoBehaviour, IInteractable {
    
    
    public virtual string InteractionPrompt => string.Empty;

    public virtual void Interact() {
        
    }
}