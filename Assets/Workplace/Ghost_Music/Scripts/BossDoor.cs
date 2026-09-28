using UnityEngine;

public class BossDoor : MonoBehaviour
{
   public void OpenDoor()
    {
        gameObject.SetActive(false);
    }

    public void CloseDoor()
    {
        gameObject.SetActive(true);
    }
}
