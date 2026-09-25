using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DesNarFallTrigger : MonoBehaviour
{
    private int fallCount;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (DesNarManager.Instance == null)
            return;

        fallCount++;

        switch(fallCount)
        {
            case 1:
                DesNarManager.Instance.PlayLine(DesNarLine.DarkRoomFirstFall);
                break;

            case 2:
                DesNarManager.Instance.PlayLine(DesNarLine.DarkRoomSecondFall);
                break;

            case 3:
                DesNarManager.Instance.PlayLine(DesNarLine.DarkRoomThirdFall);
                break;
        }
    }
}
