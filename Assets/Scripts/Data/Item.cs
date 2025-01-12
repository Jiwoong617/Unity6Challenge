using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string itemFunc;
    public Sprite itemSprite;
    public ItemType itemType;

    [TextArea]
    public string itemDescription;
}
