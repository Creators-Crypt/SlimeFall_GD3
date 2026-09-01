using UnityEngine;

public class Grow : MonoBehaviour {

    private void Update() {

        transform.localScale += new Vector3(0.25f, 0.25f, 0.25f);
    }
}