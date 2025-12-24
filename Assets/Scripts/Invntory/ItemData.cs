using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemId;
    public Sprite icon;
    public int width = 1;
    public int height = 1;
    public bool stackable = false;
    public int maxStack = 1;
}
