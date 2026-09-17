using UnityEngine;

public class MenuLogoAnimation : MonoBehaviour
{
    [Header("Gentle floating motion")]
    [SerializeField] private float floatDistance = 5f;
    [SerializeField] private float floatDuration = 5f;

    [Header("Subtle size pulse")]
    [SerializeField, Range(0f, 0.05f)]
    private float pulseAmount = 0.012f;
    [SerializeField] private float pulseDuration = 6f;

    private RectTransform logo;
    private Vector2 startingPosition;
    private Vector3 startingScale;
    private float elapsed;
    private bool initialized;

    private void OnEnable()
    {
        logo = GetComponent<RectTransform>();
        startingPosition = logo.anchoredPosition;
        startingScale = logo.localScale;
        elapsed = 0f;
        initialized = true;
    }

    private void Update()
    {
        elapsed += Time.unscaledDeltaTime;

        float floatWave = Mathf.Sin(
            elapsed * Mathf.PI * 2f / Mathf.Max(0.1f, floatDuration)
        );

        float pulseWave = Mathf.Sin(
            elapsed * Mathf.PI * 2f / Mathf.Max(0.1f, pulseDuration)
        );

        logo.anchoredPosition = startingPosition
            + Vector2.up * floatWave * floatDistance;

        logo.localScale = startingScale
            * (1f + pulseWave * pulseAmount);
    }

    private void OnDisable()
    {
        if (!initialized) return;

        logo.anchoredPosition = startingPosition;
        logo.localScale = startingScale;
    }
}
