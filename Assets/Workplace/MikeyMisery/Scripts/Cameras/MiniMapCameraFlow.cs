using UnityEngine;

public class MiniMapCameraFlow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float height = 150f;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null )
                player = playerObject.transform;
        }
    }

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