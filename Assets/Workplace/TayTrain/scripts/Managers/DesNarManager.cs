using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DesNarManager : MonoBehaviour
{
    public static DesNarManager Instance { get; private set; }

    [System.Serializable]
    public class NarratorClip
    {
        public DesNarLine line;
        public AudioClip clip;
        public bool playOnce = true;
    }

    [Header("References")]
    [SerializeField] private AudioSource audioSource;

    [Header("Desert Narrator Clips")]
    [SerializeField] private List<NarratorClip> clips = new();

    private readonly HashSet<DesNarLine> playedLines = new();
    private readonly Queue<NarratorClip> queuedLines = new();

    private Coroutine queueCoroutine;

    private void Awake()
    {
        Instance = this;

        if(audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void PlayLine(DesNarLine requestedLine)
    {
        NarratorClip narratorClip = clips.Find(entry => entry.line == requestedLine);

        if(narratorClip == null || narratorClip.clip == null)
        {
            Debug.LogWarning($"No Desert narrator clip assigned for {requestedLine}.");
            return;
        }

        if (narratorClip.playOnce && playedLines.Contains(requestedLine))
            return;

        if (narratorClip.playOnce)
            playedLines.Add(requestedLine);

        queuedLines.Enqueue(narratorClip);

        if (queueCoroutine == null)
            queueCoroutine = StartCoroutine(PlayQueue());
    }

    private IEnumerator PlayQueue()
    {
        while(queuedLines.Count > 0)
        {
            NarratorClip nextLine = queuedLines.Dequeue();

            audioSource.clip = nextLine.clip;
            audioSource.Play();

            yield return new WaitWhile(() => audioSource.isPlaying);
        }

        queueCoroutine = null;
    }
}
