using UnityEngine;

public class BossArenaLock : MonoBehaviour
{
    [Header("Arena Barriers")]
    [SerializeField] private GameObject[] barriers;

    private bool locked = false;

    public void LockArena()
    {
        if (locked)
            return;

        locked = true;

        foreach(GameObject barrier in barriers)
        {
            if(barrier != null)
            {
                barrier.SetActive(true);
            }
        }
        Debug.Log("Boss arena Locked.");
    }

    public void UnlockArena()
    {
        if (!locked)
            return;

        locked = false;

        foreach (GameObject barrier in barriers)
        {
            if (barrier != null)
            {
                barrier.SetActive(false);
            }
        }
        Debug.Log("Boss arena unlocked.");
    }
}
