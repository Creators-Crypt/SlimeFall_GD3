using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float lifetime = 0.8f;
    private float timer;

    private Color originalColor;

    public void Setup(string text, Color textColor) {

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

        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        timer += Time.deltaTime;

        float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);
        damageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        if (timer >= lifetime) Destroy(gameObject);
    }
}