using UnityEngine;

public class EquipmentSidebarToggle : MonoBehaviour
{
    private RectTransform sidebar;

    private Vector2 openPosition;
    private Vector2 closedPosition;

    private bool isOpen = true;

    private void Awake()
    {
        sidebar = GetComponent<RectTransform>();
        openPosition = sidebar.anchoredPosition;
        closedPosition = openPosition + new Vector2(sidebar.rect.width, 0);
    }

    private void Start()
    {
        sidebar.anchoredPosition = closedPosition;
        isOpen = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleSidebar();
        }
    }

    public void ToggleSidebar()
    {
        if (isOpen)
        {
            sidebar.anchoredPosition = closedPosition;
        }
        else
        {
            sidebar.anchoredPosition = openPosition;
        }

        isOpen = !isOpen;
    }
}
