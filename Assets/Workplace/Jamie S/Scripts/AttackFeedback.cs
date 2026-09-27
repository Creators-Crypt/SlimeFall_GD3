using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AttackFeedback 
{
    public GameObject vfxPrefab;
    public AudioClip sound;
    public AudioMixerGroup mixerGroup;
    public float vfxLifetime = 3f;
    [Range(0f,1f)]public float volume = 1f;
    public float soundDist = 25f;

    private static AudioMixerGroup defaultSFXGroup;

    private static AudioMixerGroup GetDefaultSFXGroup()
    {
        if(defaultSFXGroup == null)
        {
            AudioMixer mixer = Resources.Load<AudioMixer>("MasterMixer");
            if(mixer != null )
            {
                AudioMixerGroup[] groups = mixer.FindMatchingGroups("SFX");
                if(groups.Length > 0)
                {
                    defaultSFXGroup = groups[0];
                }
            }
            if(defaultSFXGroup == null )
            {
                Debug.LogWarning("AttackFeedback: No SFX group found in MasterMixer. Sound will bypass mixer.");
            }
        }
        return defaultSFXGroup; 
    }



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
            source.outputAudioMixerGroup = mixerGroup != null ? mixerGroup : GetDefaultSFXGroup();
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
