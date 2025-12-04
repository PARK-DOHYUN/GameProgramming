using UnityEngine;

/// <summary>
/// 아이템 데이터 (ScriptableObject)
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName = "Item";
    public string description = "An item";
    public Sprite icon;
    
    [Header("Item Settings")]
    public ItemType itemType = ItemType.Consumable;
    public bool isStackable = true;
    public int maxStackSize = 99;
    
    [Header("Effects")]
    public int healAmount = 0;      // 회복량 (Consumable)
    public int ammoAmount = 0;      // 탄약 개수 (Ammo)
    
    [Header("Prefab (Optional)")]
    public GameObject itemPrefab;   // 월드에 떨어진 아이템 프리팹
}
