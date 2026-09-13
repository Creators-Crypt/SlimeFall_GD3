using UnityEngine;

public class LockedDoor : MonoBehaviour, IInteractable
{
    [Header("Door")]
    [SerializeField] private GameObject doorObject;

    [Header("Prompts")]
    [SerializeField] private string lockedPrompt = "Door is locked.";
    [SerializeField] private string unlockedPrompt = "Open door";

    [Header("Settings")]
    [SerializeField] private bool startsLocked = true;
    [SerializeField] private bool openOnUnlock = false;

    [Header("Gem Requirment")]
    [SerializeField] private bool requiresGems = false;
    [SerializeField] private int requiredGems = 3;

    [Header("Key Requirment")]
    [SerializeField] private bool requiresKey = false;
    [SerializeField] private string requiredKeyName;

    private bool locked;
    private bool opened = false;

    public string InteractionPrompt
    {
        get
        {
            if (opened)
                return "";

            if (locked)
                return lockedPrompt;

            return unlockedPrompt;
        }
    }

    private void Start()
    {
        locked = startsLocked;

    }

    public void Interact()
    {
        if (opened)
            return;

        if(locked && requiresGems)
        {
            if(GemCollectionManager.Instance != null && GemCollectionManager.Instance.GemsCollected >= requiredGems)
            {
                UnlockDoor();
            }
        }

        if(locked && requiresKey)
        {
            if(InventorySystem.Instance != null && InventorySystem.Instance.HasQuestItem(requiredKeyName))
            {
                UnlockDoor();
            }
        }

        if (locked)
            return;

        if(!opened)
            OpenDoor();
    }

    public void UnlockDoor()
    {
        if (!locked)
            return;

        locked = false;

        if (openOnUnlock)
            OpenDoor();
    }

    public void LockDoor()
    {
        locked = true;
    }

    public void OpenDoor()
    {
        if (opened)
            return;

        opened = true;

        if (doorObject != null)
            doorObject.SetActive(false);
    }
}
