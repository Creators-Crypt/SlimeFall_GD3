using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour {

    [SerializeField] private TextMeshPro damageText;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float lifetime = 0.8f;
    private float timer;

    private Color originalColor;

    public void Setup(string text, Color textColor) {

        if (damageText == null) damageText = GetComponentInChildren<TextMeshPro>();

        if (damageText == null) {
            Debug.LogError($"[DamagePopup] Critical: No TextMeshPro component found on {gameObject.name} or its children! Make sure you are using a 3D TextMeshPro object.", this);
            return;
        }

        damageText.text = text;
        damageText.color = textColor;
        originalColor = textColor;
        timer = 0;

        transform.position += new Vector3(
            Random.Range(-0.5f, 0.5f), 
            Random.Range(0f, 0.3f), 
            Random.Range(-0.5f, 0.5f)
        );
    }
    private void Update() {

        transform.position += moveSpeed * Time.deltaTime * Vector3.up;
        timer += Time.deltaTime;

        float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);
        if (damageText != null) {
            damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        }
        if (timer >= lifetime) Destroy(gameObject);
    }
}