using UnityEngine;

[System.Serializable]
public class AttackFeedback 
{
    public GameObject vfxPrefab;
    public AudioClip sound;
    public float vfxLifetime = 3f;
    [Range(0f,1f)]public float volume = 1f;
    public float soundDist = 25f;


    public void Play(Vector3 _postion)
    {
        if (vfxPrefab != null)
        {
            GameObject effect = Object.Instantiate(vfxPrefab, _postion, Quaternion.identity);
            Object.Destroy(effect, vfxLifetime);
        }

        if(sound != null)
        {
            GameObject soundObject = new GameObject("Slime Attack Sound");
            soundObject.transform.position = _postion;
            AudioSource source = soundObject.AddComponent<AudioSource>();
            source.clip = sound;
            source.volume = volume;
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = 1f;
            source.maxDistance = soundDist;
            source.Play();
            Object.Destroy(soundObject, sound.length + .1f);
        }
    }
}
