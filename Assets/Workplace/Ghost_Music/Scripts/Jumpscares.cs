using UnityEngine;
using System.Collections;

public class Jumpscares : MonoBehaviour
{
    [SerializeField] GameObject scareObject;
    [SerializeField] AudioSource scareSound;
    [SerializeField] float scareDuration;

    bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if(hasTriggered)
        {
            return;
        }

        if(other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(PlayJumpscare());
        }
    }

    IEnumerator PlayJumpscare()
    {
        scareObject.SetActive(true);
        
        if(scareSound != null)
        {
            scareSound.Play();
        }

        yield return new WaitForSeconds(scareDuration);

        scareObject.SetActive(false); 
    }
}
