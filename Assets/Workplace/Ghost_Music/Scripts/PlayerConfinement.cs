using UnityEngine;

public class PlayerConfinement : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] GameObject player;

    [Header("Confinement")]
    [SerializeField] float confinementSize = 20f;
    [SerializeField] float wallHeight = 5f;

    [Header("Disappear Settings")]
    [SerializeField] bool disappearAfterTime = true;
    [SerializeField] float disappearTime = 30f;

    [SerializeField] bool disappearOnSequence = false;

    GameObject[] walls;
    bool confinementActive = true;

    private void Start()
    {
        CreateConfinement();

        if (disappearAfterTime)
        {
            Invoke(nameof(RemoveConfinement), disappearTime);
        }
    }

    void CreateConfinement()
    {
        Vector3 center = player.transform.position;

        walls = new GameObject[4];

        float halfSize = confinementSize / 2f;

        // North wall
        walls[0] = CreateWall(
            "North Wall",
            center + new Vector3(0, wallHeight / 2f, halfSize),
            new Vector3(confinementSize, wallHeight, 1f)
        );

        // South wall
        walls[1] = CreateWall(
            "South Wall",
            center + new Vector3(0, wallHeight / 2f, -halfSize),
            new Vector3(confinementSize, wallHeight, 1f)
        );

        // East wall
        walls[2] = CreateWall(
            "East Wall",
            center + new Vector3(halfSize, wallHeight / 2f, 0),
            new Vector3(1f, wallHeight, confinementSize)
        );

        // West wall
        walls[3] = CreateWall(
            "West Wall",
            center + new Vector3(-halfSize, wallHeight / 2f, 0),
            new Vector3(1f, wallHeight, confinementSize)
        );
    }

    GameObject CreateWall(string wallName, Vector3 position, Vector3 size)
    {
        GameObject wall = new GameObject(wallName);

        wall.transform.position = position;
        wall.transform.localScale = size;

        BoxCollider collider = wall.AddComponent<BoxCollider>();

        collider.size = Vector3.one;

        return wall;
    }

    public void RemoveConfinement()
    {
        if (!confinementActive)
            return;

        confinementActive = false;

        if (walls == null)
            return;

        foreach (GameObject wall in walls)
        {
            if (wall != null)
                Destroy(wall);
        }

        walls = null;

        Debug.Log("Player confinement removed.");
    }

    // Call this from another script when your sequence is finished
    public void SequenceFinished()
    {
        if (disappearOnSequence)
        {
            RemoveConfinement();
        }
    }
}

// Call confinement.SequenceFinished(); in other awake function. 
