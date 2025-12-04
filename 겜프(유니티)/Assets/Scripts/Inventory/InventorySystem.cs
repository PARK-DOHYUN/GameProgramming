using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인벤토리 시스템 (싱글톤)
/// </summary>
public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private int maxSlots = 20; // 최대 슬롯 수

    [Header("Items")]
    private List<InventoryItem> items = new List<InventoryItem>();

    // 이벤트
    public delegate void InventoryChanged();
    public event InventoryChanged OnInventoryChanged;

    private void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 아이템 추가
    /// </summary>
    public bool AddItem(ItemData itemData, int amount = 1)
    {
        if (itemData == null) return false;

        // 스택 가능한 아이템인지 확인
        if (itemData.isStackable)
        {
            // 이미 있는 아이템 찾기
            InventoryItem existingItem = items.Find(x => x.data.itemName == itemData.itemName);
            if (existingItem != null)
            {
                // 스택에 추가
                existingItem.amount += amount;
                OnInventoryChanged?.Invoke();
                Debug.Log($"Added {amount}x {itemData.itemName} (Total: {existingItem.amount})");

                // KeyItem이면 EscapeManager에 알림
                if (itemData.itemType == ItemType.KeyItem && EscapeManager.Instance != null)
                {
                    EscapeManager.Instance.CollectKeyItem(itemData.itemName);
                }

                return true;
            }
        }

        // 새 슬롯에 추가
        if (items.Count >= maxSlots)
        {
            Debug.Log("Inventory is full!");
            return false;
        }

        InventoryItem newItem = new InventoryItem(itemData, amount);
        items.Add(newItem);
        OnInventoryChanged?.Invoke();

        Debug.Log($"Added {amount}x {itemData.itemName} to inventory");

        // KeyItem이면 EscapeManager에 알림
        if (itemData.itemType == ItemType.KeyItem && EscapeManager.Instance != null)
        {
            EscapeManager.Instance.CollectKeyItem(itemData.itemName);
        }

        return true;
    }

    /// <summary>
    /// 아이템 제거
    /// </summary>
    public bool RemoveItem(ItemData itemData, int amount = 1)
    {
        InventoryItem item = items.Find(x => x.data.itemName == itemData.itemName);
        if (item == null) return false;

        item.amount -= amount;

        if (item.amount <= 0)
        {
            items.Remove(item);
        }

        OnInventoryChanged?.Invoke();
        Debug.Log($"Removed {amount}x {itemData.itemName}");
        return true;
    }

    /// <summary>
    /// 아이템 사용
    /// </summary>
    public bool UseItem(InventoryItem item)
    {
        if (item == null || item.data == null) return false;

        // 아이템 타입에 따라 처리
        bool success = false;

        switch (item.data.itemType)
        {
            case ItemType.Consumable:
                success = UseConsumable(item);
                break;

            case ItemType.Ammo:
                success = UseAmmo(item);
                break;

            default:
                Debug.Log($"Cannot use {item.data.itemName}");
                return false;
        }

        if (success)
        {
            // 아이템 사용 소리
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayItemUse();
            }

            // 사용 후 제거
            RemoveItem(item.data, 1);
        }

        return success;
    }

    /// <summary>
    /// 소모품 사용 (회복 아이템)
    /// </summary>
    private bool UseConsumable(InventoryItem item)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth == null) return false;

        // 체력 회복
        playerHealth.Heal(item.data.healAmount);
        Debug.Log($"Used {item.data.itemName} - Healed {item.data.healAmount} HP!");

        return true;
    }

    /// <summary>
    /// 탄약 사용 (총알 추가)
    /// </summary>
    private bool UseAmmo(InventoryItem item)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        RangedWeapon rifle = player.GetComponentInChildren<RangedWeapon>();
        if (rifle == null)
        {
            Debug.Log("No weapon found!");
            return false;
        }

        // 탄약 추가
        rifle.AddAmmo(item.data.ammoAmount);
        Debug.Log($"Used {item.data.itemName} - Added {item.data.ammoAmount} ammo!");

        return true;
    }

    /// <summary>
    /// 아이템 개수 확인
    /// </summary>
    public int GetItemCount(string itemName)
    {
        InventoryItem item = items.Find(x => x.data.itemName == itemName);
        return item != null ? item.amount : 0;
    }

    /// <summary>
    /// 모든 아이템 반환
    /// </summary>
    public List<InventoryItem> GetAllItems()
    {
        return items;
    }

    /// <summary>
    /// 인벤토리 비우기
    /// </summary>
    public void ClearInventory()
    {
        items.Clear();
        OnInventoryChanged?.Invoke();
    }
}

/// <summary>
/// 인벤토리 아이템
/// </summary>
[System.Serializable]
public class InventoryItem
{
    public ItemData data;
    public int amount;

    public InventoryItem(ItemData itemData, int itemAmount)
    {
        data = itemData;
        amount = itemAmount;
    }
}

/// <summary>
/// 아이템 타입
/// </summary>
public enum ItemType
{
    Consumable,  // 소모품 (회복 아이템)
    Ammo,        // 탄약
    Key,         // 열쇠
    Material,    // 재료
    KeyItem      // 필수 아이템 (탈출용)
}