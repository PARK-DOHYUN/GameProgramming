using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 인벤토리 UI 관리
/// </summary>
public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }
    
    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform slotsParent;
    [SerializeField] private GameObject slotPrefab;
    
    [Header("Settings")]
    [SerializeField] private KeyCode toggleKey = KeyCode.E;
    
    private List<InventorySlot> slots = new List<InventorySlot>();
    private bool isOpen = false;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // 슬롯 생성
        CreateSlots();
        
        // 인벤토리 시스템 이벤트 구독
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnInventoryChanged += RefreshUI;
        }
        
        // 시작 시 닫기
        CloseInventory();
    }
    
    private void Update()
    {
        // E키로 열기/닫기
        if (Input.GetKeyDown(toggleKey))
        {
            if (isOpen)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }
    }
    
    /// <summary>
    /// 슬롯 생성
    /// </summary>
    private void CreateSlots()
    {
        if (slotPrefab == null || slotsParent == null)
        {
            Debug.LogError("Slot prefab or parent not assigned!");
            return;
        }
        
        // 20개 슬롯 생성
        for (int i = 0; i < 20; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotsParent);
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            
            if (slot != null)
            {
                slots.Add(slot);
                slot.ClearSlot();
            }
        }
    }
    
    /// <summary>
    /// 인벤토리 열기
    /// </summary>
    public void OpenInventory()
    {
        isOpen = true;
        inventoryPanel.SetActive(true);
        RefreshUI();
        
        // 게임 일시정지 (선택사항)
        // Time.timeScale = 0;
        
        Debug.Log("Inventory opened");
    }
    
    /// <summary>
    /// 인벤토리 닫기
    /// </summary>
    public void CloseInventory()
    {
        isOpen = false;
        inventoryPanel.SetActive(false);
        
        // 게임 재개
        // Time.timeScale = 1;
        
        Debug.Log("Inventory closed");
    }
    
    /// <summary>
    /// UI 갱신
    /// </summary>
    public void RefreshUI()
    {
        if (InventorySystem.Instance == null) return;
        
        List<InventoryItem> items = InventorySystem.Instance.GetAllItems();
        
        // 모든 슬롯 비우기
        foreach (InventorySlot slot in slots)
        {
            slot.ClearSlot();
        }
        
        // 아이템으로 슬롯 채우기
        for (int i = 0; i < items.Count && i < slots.Count; i++)
        {
            slots[i].SetItem(items[i]);
        }
    }
    
    private void OnDestroy()
    {
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnInventoryChanged -= RefreshUI;
        }
    }
}
