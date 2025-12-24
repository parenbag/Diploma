using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public InventoryModel model;
    public RectTransform gridParent;       
    public GameObject cellPrefab;          
    public GameObject itemUIPrefab;        
    public int cellSize = 64;
    public int spacing = 4;

    private RectTransform[,] slotRects;
    private Dictionary<int, GameObject> itemUIByIndex = new Dictionary<int, GameObject>();

    void Start()
    {
        if (model == null) Debug.LogError("InventoryModel not assigned!");
        if (gridParent == null) Debug.LogError("GridParent not assigned!");
        GenerateGridVisual();
        RefreshAll();
    }

    public void GenerateGridVisual()
    {
        for (int i = gridParent.childCount - 1; i >= 0; i--)
            DestroyImmediate(gridParent.GetChild(i).gameObject);

        int w = model.width;
        int h = model.height;
        slotRects = new RectTransform[w, h];

        float cellFull = cellSize + spacing;
        Vector2 size = new Vector2(w * cellFull - spacing, h * cellFull - spacing);
        gridParent.sizeDelta = size;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                GameObject cell = Instantiate(cellPrefab, gridParent);
                RectTransform rt = cell.GetComponent<RectTransform>();
                rt.anchorMin = rt.anchorMax = new Vector2(0, 1); 
                rt.pivot = new Vector2(0, 1); 
                float posX = x * cellFull;
                float posY = -y * cellFull;
                rt.anchoredPosition = new Vector2(posX, posY);
                rt.sizeDelta = new Vector2(cellSize, cellSize);
                slotRects[x, y] = rt;
                cell.name = $"cell_{x}_{y}";
            }
        }
    }

    public Vector2Int ScreenPosToCell(Vector2 screenPos, Camera uiCamera = null)
    {
        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(gridParent, screenPos, uiCamera, out local);
        float cellFull = cellSize + spacing;
        int cellX = Mathf.FloorToInt(local.x / cellFull);
        int cellY = Mathf.FloorToInt(-local.y / cellFull);
        return new Vector2Int(cellX, cellY);
    }

    public void RefreshAll()
    {
        foreach (var kv in itemUIByIndex) if (kv.Value != null) Destroy(kv.Value);
        itemUIByIndex.Clear();

        float cellFull = cellSize + spacing;

        for (int i = 0; i < model.items.Count; i++)
        {
            var inv = model.items[i];
            if (inv == null || inv.data == null) continue;

            GameObject go = Instantiate(itemUIPrefab, gridParent);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0, 1); 
            rt.pivot = new Vector2(0, 1); 

            int w = inv.rotated ? inv.data.height : inv.data.width;
            int h = inv.rotated ? inv.data.width : inv.data.height;

            float posX = inv.posX * cellFull;
            float posY = -inv.posY * cellFull;
            rt.anchoredPosition = new Vector2(posX, posY);
            rt.sizeDelta = new Vector2(w * cellFull - spacing, h * cellFull - spacing);

            var image = go.GetComponent<Image>();
            image.sprite = inv.data.icon;
            image.preserveAspect = false; 
            image.type = Image.Type.Sliced;

            var itemUI = go.GetComponent<ItemUI>();
            itemUI.Init(this, i);

            itemUIByIndex[i] = go;
        }
    }
}
