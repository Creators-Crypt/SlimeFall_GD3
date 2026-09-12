using UnityEngine;

public class BossArenaTrigger : MonoBehaviour
{
    [SerializeField] private BossArenaLock bossArenaLock;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;

        bossArenaLock.LockArena();
    }
}
