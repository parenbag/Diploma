using System.Collections.Generic;
using UnityEngine;

public class InventoryModel : MonoBehaviour
{
    public int width = 10;
    public int height = 6;

    private int[,] grid;
    public List<InventoryItem> items = new List<InventoryItem>();

    void Awake()
    {
        InitGrid();
    }

    public void InitGrid()
    {
        grid = new int[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = -1;
        RebuildGridFromItems();
    }

    public bool IsInside(int x, int y) => x >= 0 && y >= 0 && x < width && y < height;

    public void GridSet(int x, int y, int value)
    {
        if (!IsInside(x, y)) return;
        grid[x, y] = value;
    }

    public int GetItemIndexAtCell(int x, int y)
    {
        if (!IsInside(x, y)) return -1;
        return grid[x, y];
    }

    public bool CanPlace(ItemData data, int x, int y, bool rotated, int ignoreIndex = -1)
    {
        int w = rotated ? data.height : data.width;
        int h = rotated ? data.width : data.height;

        if (x < 0 || y < 0 || x + w > width || y + h > height) return false;

        for (int i = 0; i < w; i++)
            for (int j = 0; j < h; j++)
            {
                int gx = x + i;
                int gy = y + j;
                int occupant = grid[gx, gy];
                if (occupant != -1 && occupant != ignoreIndex) return false;
            }

        return true;
    }

    public int PlaceItem(ItemData data, int x, int y, bool rotated, int amount = 1)
    {
        if (!CanPlace(data, x, y, rotated)) return -1;

        InventoryItem inv = new InventoryItem
        {
            data = data,
            posX = x,
            posY = y,
            rotated = rotated,
            amount = amount
        };

        int index = items.Count;
        items.Add(inv);

        int w = rotated ? data.height : data.width;
        int h = rotated ? data.width : data.height;
        for (int i = 0; i < w; i++)
            for (int j = 0; j < h; j++)
                grid[x + i, y + j] = index;

        return index;
    }

    public bool MoveItem(int index, int newX, int newY, bool newRotated)
    {
        if (index < 0 || index >= items.Count) return false;
        var inv = items[index];
        if (inv == null || inv.data == null) return false;

        int oldX = inv.posX;
        int oldY = inv.posY;
        bool oldRot = inv.rotated;

        int oldW = oldRot ? inv.data.height : inv.data.width;
        int oldH = oldRot ? inv.data.width : inv.data.height;

        for (int i = 0; i < oldW; i++)
            for (int j = 0; j < oldH; j++)
                GridSet(oldX + i, oldY + j, -1);

        if (CanPlace(inv.data, newX, newY, newRotated, ignoreIndex: index))
        {
            int newW = newRotated ? inv.data.height : inv.data.width;
            int newH = newRotated ? inv.data.width : inv.data.height;
            for (int i = 0; i < newW; i++)
                for (int j = 0; j < newH; j++)
                    GridSet(newX + i, newY + j, index);

            inv.posX = newX;
            inv.posY = newY;
            inv.rotated = newRotated;
            return true;
        }
        else
        {
            for (int i = 0; i < oldW; i++)
                for (int j = 0; j < oldH; j++)
                    GridSet(oldX + i, oldY + j, index);

            return false;
        }
    }

    public void RemoveItemAtIndex(int index)
    {
        if (index < 0 || index >= items.Count) return;
        var inv = items[index];
        if (inv == null) return;

        int w = inv.rotated ? inv.data.height : inv.data.width;
        int h = inv.rotated ? inv.data.width : inv.data.height;
        for (int i = 0; i < w; i++)
            for (int j = 0; j < h; j++)
                GridSet(inv.posX + i, inv.posY + j, -1);

        items[index] = null;
    }

    public void RebuildGridFromItems()
    {
        if (grid == null) InitGrid();
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = -1;

        for (int i = 0; i < items.Count; i++)
        {
            var inv = items[i];
            if (inv == null || inv.data == null) continue;
            int w = inv.rotated ? inv.data.height : inv.data.width;
            int h = inv.rotated ? inv.data.width : inv.data.height;
            for (int ix = 0; ix < w; ix++)
                for (int iy = 0; iy < h; iy++)
                {
                    int gx = inv.posX + ix;
                    int gy = inv.posY + iy;
                    if (IsInside(gx, gy)) grid[gx, gy] = i;
                }
        }
    }

    public void DebugPlaceTest(ItemData a, ItemData b)
    {
        PlaceItem(a, 0, 0, false);
        PlaceItem(b, 3, 1, false);
        RebuildGridFromItems();
    }
}
