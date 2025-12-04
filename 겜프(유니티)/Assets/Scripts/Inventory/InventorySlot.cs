using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// 인벤토리 슬롯 (우클릭으로 아이템 사용)
/// </summary>
public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private GameObject amountBackground;

    private InventoryItem currentItem;

    /// <summary>
    /// 아이템 설정
    /// </summary>
    public void SetItem(InventoryItem item)
    {
        currentItem = item;

        if (item != null && item.data != null)
        {
            // 아이콘 표시
            if (iconImage != null)
            {
                iconImage.sprite = item.data.icon;
                iconImage.enabled = true;
                iconImage.color = Color.white;
            }

            // 개수 표시
            if (amountText != null)
            {
                if (item.amount > 1)
                {
                    amountText.text = item.amount.ToString();
                    amountText.enabled = true;

                    // AmountBG가 있으면 활성화
                    if (amountBackground != null)
                    {
                        amountBackground.SetActive(true);
                    }
                }
                else
                {
                    amountText.enabled = false;

                    // AmountBG가 있으면 비활성화
                    if (amountBackground != null)
                    {
                        amountBackground.SetActive(false);
                    }
                }
            }
        }
        else
        {
            ClearSlot();
        }
    }

    /// <summary>
    /// 슬롯 비우기
    /// </summary>
    public void ClearSlot()
    {
        currentItem = null;

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }

        if (amountText != null)
        {
            amountText.enabled = false;
        }

        if (amountBackground != null)
        {
            amountBackground.SetActive(false);
        }
    }

    /// <summary>
    /// 클릭 이벤트 (우클릭 = 사용)
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // eventData null 체크
        if (eventData == null) return;

        // 우클릭
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 빈 슬롯인지 확인
            if (currentItem == null || currentItem.data == null)
            {
                Debug.Log("Empty slot - nothing to use!");
                return;
            }

            UseItem();
        }
        // 좌클릭 (나중에 드래그 & 드롭 등)
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 아이템 정보 표시 등
            if (currentItem != null && currentItem.data != null)
            {
                Debug.Log($"Clicked: {currentItem.data.itemName} x{currentItem.amount}");
            }
        }
    }

    /// <summary>
    /// 아이템 사용
    /// </summary>
    private void UseItem()
    {
        // null 체크 강화
        if (currentItem == null)
        {
            Debug.Log("No item in this slot!");
            return;
        }

        if (currentItem.data == null)
        {
            Debug.LogWarning("Item data is null!");
            return;
        }

        // 인벤토리 시스템 체크
        if (InventorySystem.Instance == null)
        {
            Debug.LogError("InventorySystem not found!");
            return;
        }

        // 아이템 사용
        bool success = InventorySystem.Instance.UseItem(currentItem);

        // success 체크 + currentItem이 여전히 유효한지 확인
        if (success && currentItem != null && currentItem.data != null)
        {
            Debug.Log($"Used {currentItem.data.itemName}!");
        }
        else if (!success)
        {
            Debug.Log("Failed to use item!");
        }
    }
}