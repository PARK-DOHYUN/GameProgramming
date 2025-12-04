using UnityEngine;

/// <summary>
/// 월드에 떨어진 아이템 (E키로 획득)
/// </summary>
public class WorldItem : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private ItemData itemData;
    [SerializeField] private int amount = 1;

    [Header("Interaction")]
    [SerializeField] private KeyCode pickupKey = KeyCode.E;
    [SerializeField] private float pickupRange = 2f;

    [Header("UI")]
    [SerializeField] private GameObject pickupPrompt; // "Press E" UI

    private Transform player;
    private bool playerInRange = false;

    private void Start()
    {
        // 플레이어 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (pickupPrompt != null)
        {
            pickupPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        if (player == null) return;

        // 거리 체크
        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= pickupRange;

        // UI 표시/숨김
        if (pickupPrompt != null)
        {
            pickupPrompt.SetActive(playerInRange);
        }

        // E키로 획득
        if (playerInRange && Input.GetKeyDown(pickupKey))
        {
            PickupItem();
        }
    }

    /// <summary>
    /// 아이템 획득
    /// </summary>
    private void PickupItem()
    {
        if (itemData == null)
        {
            Debug.LogWarning("No item data assigned!");
            return;
        }

        if (InventorySystem.Instance == null)
        {
            Debug.LogError("InventorySystem not found!");
            return;
        }

        // 인벤토리에 추가
        bool success = InventorySystem.Instance.AddItem(itemData, amount);

        if (success)
        {
            Debug.Log($"Picked up {amount}x {itemData.itemName}");

            // 아이템 제거
            Destroy(gameObject);
        }
    }

    // Gizmo로 획득 범위 표시
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}