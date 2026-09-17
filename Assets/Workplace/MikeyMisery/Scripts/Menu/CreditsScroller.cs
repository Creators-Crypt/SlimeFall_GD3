using UnityEngine;

public class CreditsScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 35f;

    private RectTransform creditsText;
    private RectTransform creditsWindow;

    private void Awake()
    {
        creditsText = GetComponent<RectTransform>();
        creditsWindow = transform.parent as RectTransform;
    }

    private void OnEnable()
    {
        ResetCredits();
    }

    private void Update()
    {
        if (creditsText == null)
            return;

        creditsText.anchoredPosition += Vector2.up * scrollSpeed * Time.unscaledDeltaTime;
    }

    private void ResetCredits()
    {
        if (creditsText == null || creditsWindow == null)
            return;

        float startY = -(creditsWindow.rect.height / 2f) -(creditsText.rect.height / 2f);

        creditsText.anchoredPosition = new Vector2(creditsText.anchoredPosition.x, startY);
    }
}
