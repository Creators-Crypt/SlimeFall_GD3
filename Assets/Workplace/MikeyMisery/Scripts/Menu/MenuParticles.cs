using UnityEngine;
using UnityEngine.UI;

public class MenuParticles : MonoBehaviour
{
    [SerializeField, Range(1, 60)] private int count = 22;
    [SerializeField]
    private Color tint =
        new Color(0.45f, 0.75f, 0.3f, 0.25f);

    private RectTransform area;
    private RectTransform[] dots;
    private Image[] images;
    private float[] speeds;
    private float[] phases;

    private Texture2D texture;
    private Sprite sprite;
    private float elapsed;

    private void Start()
    {
        area = GetComponent<RectTransform>();
        Canvas.ForceUpdateCanvases();

        // Create a white circle with a soft transparent edge.
        texture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                float distance = Vector2.Distance(
                    new Vector2(x, y), new Vector2(15.5f, 15.5f)
                ) / 15.5f;

                float alpha = Mathf.Pow(
                    Mathf.Clamp01(1f - distance), 2f
                );

                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();

        sprite = Sprite.Create(
            texture, new Rect(0, 0, 32, 32),
            new Vector2(0.5f, 0.5f)
        );

        dots = new RectTransform[count];
        images = new Image[count];
        speeds = new float[count];
        phases = new float[count];

        for (int i = 0; i < count; i++)
        {
            var dot = new GameObject(
                "Particle", typeof(RectTransform), typeof(Image)
            );

            dot.transform.SetParent(transform, false);

            dots[i] = dot.GetComponent<RectTransform>();
            dots[i].anchorMin = dots[i].anchorMax = area.pivot;
            dots[i].sizeDelta = Vector2.one * Random.Range(8f, 20f);
            dots[i].anchoredPosition = new Vector2(
                Random.Range(area.rect.xMin, area.rect.xMax),
                Random.Range(area.rect.yMin, area.rect.yMax)
            );

            images[i] = dot.GetComponent<Image>();
            images[i].sprite = sprite;
            images[i].color = tint;
            images[i].raycastTarget = false;

            speeds[i] = Random.Range(10f, 25f);
            phases[i] = Random.Range(0f, Mathf.PI * 2f);
        }
    }

    private void Update()
    {
        if (dots == null) return;

        float dt = Time.unscaledDeltaTime;
        elapsed += dt;
        Rect bounds = area.rect;

        for (int i = 0; i < dots.Length; i++)
        {
            Vector2 position = dots[i].anchoredPosition;
            position.y += speeds[i] * dt;
            position.x += Mathf.Sin(elapsed * 0.5f + phases[i]) * 6f * dt;

            if (position.y > bounds.yMax + 20f)
            {
                position.y = bounds.yMin - 20f;
                position.x = Random.Range(bounds.xMin, bounds.xMax);
            }

            dots[i].anchoredPosition = position;

            float height = Mathf.InverseLerp(
                bounds.yMin, bounds.yMax, position.y
            );

            Color color = tint;
            color.a *= Mathf.Sin(height * Mathf.PI);
            images[i].color = color;
        }
    }

    private void OnDestroy()
    {
        if (sprite != null) Destroy(sprite);
        if (texture != null) Destroy(texture);
    }
}
