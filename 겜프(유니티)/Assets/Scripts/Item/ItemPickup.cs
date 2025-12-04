using UnityEngine;

/// <summary>
/// 맵에 있는 아이템 획득 시스템
/// </summary>
public class ItemPickup : MonoBehaviour
{
    [Header("Item Info")]
    [SerializeField] private ItemType itemType;
    [SerializeField] private int amount = 1;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    public enum ItemType
    {
        Ammo,           // 총알
        Bat,            // 배트
        Rifle,          // 라이플
        CarPart,        // 자동차 부품
        HealthPack      // 체력 회복 아이템
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 충돌 확인
        if (collision.CompareTag("Player"))
        {
            PickupItem(collision.gameObject);
        }
    }

    /// <summary>
    /// 아이템 획득 처리
    /// </summary>
    private void PickupItem(GameObject player)
    {
        bool pickedUp = false;

        switch (itemType)
        {
            case ItemType.Ammo:
                pickedUp = PickupAmmo(player);
                break;

            case ItemType.Bat:
                pickedUp = PickupBat(player);
                break;

            case ItemType.Rifle:
                pickedUp = PickupRifle(player);
                break;

            case ItemType.CarPart:
                pickedUp = PickupCarPart(player);
                break;

            case ItemType.HealthPack:
                pickedUp = PickupHealthPack(player);
                break;
        }

        // 획득 성공 시 오브젝트 제거
        if (pickedUp)
        {
            Debug.Log($"Picked up {itemType} x{amount}");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 탄약 획득
    /// </summary>
    private bool PickupAmmo(GameObject player)
    {
        WeaponManager weaponManager = player.GetComponent<WeaponManager>();
        if (weaponManager != null)
        {
            weaponManager.AddAmmo(amount);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 배트 획득
    /// </summary>
    private bool PickupBat(GameObject player)
    {
        WeaponManager weaponManager = player.GetComponent<WeaponManager>();
        if (weaponManager != null)
        {
            // 배트 프리팹을 플레이어에게 장착 (실제로는 WeaponManager에서 처리)
            // 여기서는 간단히 획득 알림만
            Debug.Log("Bat acquired! Press '2' to equip.");
            return true;
        }
        return false;
    }

    /// <summary>
    /// 라이플 획득
    /// </summary>
    private bool PickupRifle(GameObject player)
    {
        WeaponManager weaponManager = player.GetComponent<WeaponManager>();
        if (weaponManager != null)
        {
            Debug.Log("Rifle acquired! Press '3' to equip.");
            return true;
        }
        return false;
    }

    /// <summary>
    /// 자동차 부품 획득
    /// </summary>
    private bool PickupCarPart(GameObject player)
    {
        InventoryManager inventory = player.GetComponent<InventoryManager>();
        if (inventory != null)
        {
            inventory.AddCarPart();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 체력 회복 아이템 획득
    /// </summary>
    private bool PickupHealthPack(GameObject player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.Heal(amount);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 아이템 타입 반환
    /// </summary>
    public ItemType GetItemType()
    {
        return itemType;
    }

    /// <summary>
    /// 아이템 수량 반환
    /// </summary>
    public int GetAmount()
    {
        return amount;
    }
}
