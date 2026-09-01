using UnityEngine;

public class DestroyAfterxx : MonoBehaviour
{
    public float lifetime = 2f;

    private void Start()
    {
        Destroy(gameObject,lifetime);
    }
}
