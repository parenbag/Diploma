using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class ItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CanvasGroup canvasGroup;
    private InventoryUI inventoryUI;
    private RectTransform rt;
    private int itemIndex;
    private Vector2 originalAnchoredPos;
    private int originalIndex;
    private Canvas rootCanvas;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Init(InventoryUI invUI, int index)
    {
        inventoryUI = invUI;
        itemIndex = index;
        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalAnchoredPos = rt.anchoredPosition;
        originalIndex = itemIndex;
        rt.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.85f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventoryUI.gridParent, eventData.position, eventData.pressEventCamera, out local);
        rt.anchoredPosition = local;

        var model = inventoryUI.model;
        if (itemIndex < 0 || itemIndex >= model.items.Count) return;
        var inv = model.items[itemIndex];
        if (inv == null) return;

        Vector2Int cell = inventoryUI.ScreenPosToCell(eventData.position, eventData.pressEventCamera);

        bool can = model.CanPlace(inv.data, cell.x, cell.y, inv.rotated, ignoreIndex: itemIndex);
        var img = GetComponent<Image>();
        img.color = can ? Color.white : new Color(1f, 0.6f, 0.6f, 1f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        Vector2Int cell = inventoryUI.ScreenPosToCell(eventData.position, eventData.pressEventCamera);
        var model = inventoryUI.model;
        if (itemIndex < 0 || itemIndex >= model.items.Count) return;
        var inv = model.items[itemIndex];
        if (inv == null) { Destroy(gameObject); return; }

        bool moved = model.MoveItem(itemIndex, cell.x, cell.y, inv.rotated);
        if (moved)
        {
            inventoryUI.RefreshAll();
        }
        else
        {
            rt.anchoredPosition = originalAnchoredPos;
            GetComponent<Image>().color = Color.white;
        }
    }

    void Update()
    {
        if (canvasGroup != null && !canvasGroup.blocksRaycasts)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                var model = inventoryUI.model;
                if (itemIndex >= 0 && itemIndex < model.items.Count)
                {
                    var inv = model.items[itemIndex];
                    if (inv != null)
                    {
                        inv.rotated = !inv.rotated;
                        float cellFull = inventoryUI.cellSize + inventoryUI.spacing;
                        int w = inv.rotated ? inv.data.height : inv.data.width;
                        int h = inv.rotated ? inv.data.width : inv.data.height;
                        rt.sizeDelta = new Vector2(w * cellFull - inventoryUI.spacing, h * cellFull - inventoryUI.spacing);
                    }
                }
            }
        }
    }
}
