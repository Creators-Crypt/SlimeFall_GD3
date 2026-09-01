using UnityEngine;

public class MiniMapCameraFlow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float height = 50f;

    private void LateUpdate()
    {
        if (player == null)
            return;

        transform.position = new Vector3(
            player.position.x, 
            player.position.y + height, 
            player.position.z
        );
    }
}
