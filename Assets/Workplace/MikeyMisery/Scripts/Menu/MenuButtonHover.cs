using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.CompilerServices;

public class MenuButtonHover : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    ISelectHandler, IDeselectHandler
{
    [SerializeField] private float hoverScale = 1.04f;
    [SerializeField] private float speed = 14f;

    private Button button;
    private Vector3 normalScale;
    private bool isHovered;
    private bool isSelected;

    private void Awake()
    {
        button = GetComponent<Button>();
        normalScale = transform.localScale;
    }

    private void Update()
    {
       bool isHighlighted = (isHovered || isSelected) && button.IsInteractable();

        Vector3 target = normalScale * (isHighlighted ? hoverScale : 1f);

        transform.localScale = Vector3.Lerp(transform.localScale, target, 1f - Mathf.Exp(-speed * Time.unscaledDeltaTime));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
    }

    private void OnDisable()
    {
        isHovered = isSelected = false;
        transform.localScale = normalScale;
    }
}
