using UnityEngine;

public class InventoryTestPlacer : MonoBehaviour
{
    public InventoryModel model;
    public ItemData itemA;
    public ItemData itemB;

    void Start()
    {
        if (model != null && itemA != null)
            model.PlaceItem(itemA, 0, 0, false);
        if (model != null && itemB != null)
            model.PlaceItem(itemB, 3, 1, false);

        model.RebuildGridFromItems();
    }
}
