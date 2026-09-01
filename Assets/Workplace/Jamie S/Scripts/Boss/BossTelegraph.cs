using UnityEngine;
using UnityEngine.UI;

public class BossTelegraph : MonoBehaviour
{
    [SerializeField] public Transform outline;
    [SerializeField] public Transform fill;

    [SerializeField] public float scale = 2f;

    [Header("Color")]
    [SerializeField]public Renderer colorTarget;
    [SerializeField] public Color startColor = new Color(1f,.8f,.1f,.35f);
    [SerializeField] public Color endColor = new Color(1f, .15f, .1f,.75f);

    [Header("LifeTime")]
    [SerializeField] public float destroyAfterHitTime = .25f;
    [SerializeField] public bool destryWhenDone = true;

    private float radius;
    private float duration;
    private float timer;
    private bool playing;

    private Material cachedMaterial;

    private void Awake()
    {
        if(colorTarget != null)
        {
            cachedMaterial = colorTarget.material;
        }
    }
    void Update()
    {
        if (playing == false) return;
        timer += Time.deltaTime;
        UpdateFill(GetProgress());

        if(timer >= duration)
        {
            playing = false;
            if(destryWhenDone)
            {
                Destroy(gameObject, destroyAfterHitTime);
            }
        }
    }
    public float GetProgress()
    {
        if (duration <= 0f) return 1f;
        return Mathf.Clamp01(timer /  duration);
    }

    public void Play(float _dangerRadius, float _secondToHit)
    {
        radius = Mathf.Max(_dangerRadius, .1f);
        duration = Mathf.Max(_secondToHit, .5f);
        timer = 0f;
        playing = true;

        if(outline != null)
        {
            float size = radius * scale;
            outline.localScale = new Vector3(size,outline.localScale.y,size);
        }
        UpdateFill(0f);
    }
    private void UpdateFill(float _amount)
    {
        if (fill != null)
        {
            float size = radius * scale * _amount;
            fill.localScale = new Vector3(size,fill.localScale.y,size);
        }

        if(cachedMaterial != null)
        {
            Color newColor = Color.Lerp(startColor,endColor,_amount);

            cachedMaterial.SetColor("_BaseColor", newColor);
        }
    }
}
