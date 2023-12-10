using UnityEngine;
public abstract class ItemObject : ScriptableObject
{
    public enum ItemType
    {
        RESTOREITEM,
        EQUIPEMENT,
        DEFAULT,
    }

    public GameObject prefab;
    public ItemType type;
    [TextArea(15, 20)]
    public string description;
}
