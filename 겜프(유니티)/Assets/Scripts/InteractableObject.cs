using UnityEngine;
using TMPro;

/// <summary>
/// 상호작용 가능한 오브젝트 (F키로 아이템 획득)
/// </summary>
public class InteractableObject : MonoBehaviour
{
    [Header("Object Info")]
    [SerializeField] private string objectName = "Object";
    [SerializeField] private Sprite usedSprite; // 사용 후 스프라이트 (선택사항)

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [SerializeField] private float interactionRange = 2f;

    [Header("Loot Settings")]
    [SerializeField] private bool useInventorySystem = true; // 인벤토리 시스템 사용
    [SerializeField] private bool useMultipleLoot = false; // 여러 아이템 동시 드랍

    // 단일 아이템 드랍 (useMultipleLoot = false)
    [SerializeField] private ItemData itemDrop; // 드랍할 아이템
    [SerializeField] private int dropAmount = 1; // 드랍 개수

    // 여러 아이템 드랍 (useMultipleLoot = true)
    [SerializeField] private int maxLootTypes = 3; // 최대 파밍 종류 수
    [SerializeField] private LootItem[] possibleLoot; // 파밍 가능한 아이템들

    // 구버전 호환 (useInventorySystem = false일 때만)
    [SerializeField] private LootType lootType;
    [SerializeField] private int minAmount = 5;
    [SerializeField] private int maxAmount = 15;

    [Header("UI")]
    [SerializeField] private bool createUIPrompt = false; // UI 프롬프트 자동 생성 (비활성화 추천)
    [SerializeField] private GameObject interactPrompt; // "Press F" UI
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private float promptOffset = 1f;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip interactSound;

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private bool isUsed = false;
    private bool playerInRange = false;

    public enum LootType
    {
        Ammo,
        Health,
        Random
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 플레이어 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning($"{objectName}: Player not found! Make sure Player has 'Player' tag.");
        }

        // UI 프롬프트 생성 시도 (활성화된 경우만)
        if (createUIPrompt && interactPrompt == null)
        {
            try
            {
                CreateInteractPrompt();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"{objectName}: Failed to create interact prompt. Interaction will work without UI. Error: {e.Message}");
            }
        }

        if (interactPrompt != null)
        {
            // World Space Canvas에 Event Camera 자동 설정
            Canvas canvas = interactPrompt.GetComponent<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
            {
                if (canvas.worldCamera == null)
                {
                    Camera mainCamera = Camera.main;
                    if (mainCamera != null)
                    {
                        canvas.worldCamera = mainCamera;
                        Debug.Log($"{objectName}: Auto-assigned Event Camera to Canvas");
                    }
                }
            }

            interactPrompt.SetActive(false);
        }

        // UI 없이도 작동 가능
        if (!createUIPrompt)
        {
            Debug.Log($"{objectName}: Ready for interaction (UI prompt disabled)");
        }
    }

    private void Update()
    {
        if (isUsed || player == null) return;

        // 플레이어와의 거리 체크
        float distance = Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= interactionRange;

        // UI 프롬프트 표시/숨김
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(playerInRange);

            if (playerInRange)
            {
                // 프롬프트 위치 업데이트 (오브젝트 위)
                interactPrompt.transform.position = transform.position + Vector3.up * promptOffset;
            }
        }

        // F키 상호작용
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            Interact();
        }
    }

    /// <summary>
    /// 상호작용 실행
    /// </summary>
    private void Interact()
    {
        if (isUsed) return;

        isUsed = true;

        // 아이템 지급
        GiveLoot();

        // 아이템 획득 소리 (AudioManager 사용)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayItemPickup();
        }

        // 사운드 재생 (기존 방식도 유지)
        if (interactSound != null)
        {
            AudioSource.PlayClipAtPoint(interactSound, transform.position);
        }

        // 사용 후 처리
        OnUsed();

        Debug.Log($"Interacted with {objectName}");
    }

    /// <summary>
    /// 아이템 지급
    /// </summary>
    private void GiveLoot()
    {
        // 인벤토리 시스템 사용
        if (useInventorySystem)
        {
            // 여러 아이템 동시 드랍
            if (useMultipleLoot && possibleLoot != null && possibleLoot.Length > 0)
            {
                GiveMultipleLoot();
                return;
            }

            // 단일 아이템 드랍
            if (itemDrop != null)
            {
                GiveSingleLoot();
                return;
            }
        }

        // 구버전 시스템 (호환성)
        int amount = Random.Range(minAmount, maxAmount + 1);

        // 실제 지급 타입 결정
        LootType actualLootType = lootType;
        if (lootType == LootType.Random)
        {
            actualLootType = Random.value > 0.5f ? LootType.Ammo : LootType.Health;
        }

        switch (actualLootType)
        {
            case LootType.Ammo:
                GiveAmmo(amount);
                break;

            case LootType.Health:
                GiveHealth(amount);
                break;
        }
    }

    /// <summary>
    /// 단일 아이템 지급
    /// </summary>
    private void GiveSingleLoot()
    {
        if (InventorySystem.Instance == null) return;

        bool success = InventorySystem.Instance.AddItem(itemDrop, dropAmount);

        if (success)
        {
            Debug.Log($"Added {dropAmount}x {itemDrop.itemName} to inventory");
        }
    }

    /// <summary>
    /// 여러 아이템 동시 지급 (KeyItem 우선)
    /// </summary>
    private void GiveMultipleLoot()
    {
        if (InventorySystem.Instance == null) return;

        int itemsGiven = 0;
        System.Collections.Generic.List<LootItem> remainingLoot = new System.Collections.Generic.List<LootItem>();

        // 1단계: KeyItem 먼저 지급 (필수!)
        foreach (LootItem loot in possibleLoot)
        {
            if (loot.item == null) continue;

            // KeyItem이면 무조건 지급
            if (loot.item.itemType == ItemType.KeyItem)
            {
                int amount = Random.Range(loot.minAmount, loot.maxAmount + 1);
                bool success = InventorySystem.Instance.AddItem(loot.item, amount);

                if (success)
                {
                    itemsGiven++;
                    Debug.Log($"[KEY ITEM LOOTED] {amount}x {loot.item.itemName} from {objectName}");
                }
            }
            else
            {
                // KeyItem 아니면 나중에 처리
                remainingLoot.Add(loot);
            }
        }

        // 2단계: 남은 슬롯에 일반 아이템 랜덤 지급
        if (itemsGiven < maxLootTypes && remainingLoot.Count > 0)
        {
            // Fisher-Yates 셔플 (일반 아이템만)
            for (int i = remainingLoot.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                LootItem temp = remainingLoot[i];
                remainingLoot[i] = remainingLoot[randomIndex];
                remainingLoot[randomIndex] = temp;
            }

            // 남은 슬롯만큼 지급
            foreach (LootItem loot in remainingLoot)
            {
                if (itemsGiven >= maxLootTypes) break;
                if (loot.item == null) continue;

                int amount = Random.Range(loot.minAmount, loot.maxAmount + 1);
                bool success = InventorySystem.Instance.AddItem(loot.item, amount);

                if (success)
                {
                    itemsGiven++;
                    Debug.Log($"Looted {amount}x {loot.item.itemName} from {objectName}");
                }
            }
        }

        if (itemsGiven > 0)
        {
            Debug.Log($"Total: Got {itemsGiven} different items from {objectName}!");
        }
    }

    /// <summary>
    /// 탄약 지급
    /// </summary>
    private void GiveAmmo(int amount)
    {
        if (player == null) return;

        // RangedWeapon 찾기
        RangedWeapon rifle = player.GetComponentInChildren<RangedWeapon>();

        if (rifle != null)
        {
            rifle.AddAmmo(amount);
            Debug.Log($"Got {amount} ammo from {objectName}!");
        }
    }

    /// <summary>
    /// 체력 회복
    /// </summary>
    private void GiveHealth(int amount)
    {
        if (player == null) return;

        // PlayerHealth 찾기
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.Heal(amount);
            Debug.Log($"Healed {amount} HP from {objectName}!");
        }
    }

    /// <summary>
    /// 사용 후 처리
    /// </summary>
    private void OnUsed()
    {
        // UI 프롬프트 숨김
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }

        // 스프라이트 변경 (사용됨 표시)
        if (usedSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = usedSprite;
        }
        else
        {
            // 스프라이트가 없으면 반투명하게
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 0.5f;
                spriteRenderer.color = color;
            }
        }

        // 또는 오브젝트 제거 (선택사항)
        // Destroy(gameObject, 0.5f);
    }

    /// <summary>
    /// UI 프롬프트 자동 생성
    /// </summary>
    private void CreateInteractPrompt()
    {
        // Canvas 찾기
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning($"{objectName}: No Canvas found. Create a Canvas in the scene (Right-click → UI → Canvas). Interaction will work without UI prompt.");
            return;
        }

        try
        {
            // 프롬프트 오브젝트 생성
            GameObject promptObj = new GameObject($"{objectName}_Prompt");
            promptObj.transform.SetParent(canvas.transform, false);

            // RectTransform 설정
            RectTransform rectTransform = promptObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(150, 40);

            // TextMeshPro 추가 시도
            TextMeshProUGUI textComponent = promptObj.AddComponent<TextMeshProUGUI>();
            if (textComponent == null)
            {
                Debug.LogWarning($"{objectName}: TextMeshPro component failed to create. Import TMP via Window → TextMeshPro → Import TMP Essential Resources");
                Destroy(promptObj);
                return;
            }

            textComponent.text = $"Press {interactKey}";
            textComponent.fontSize = 18;
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.color = Color.white;

            // 배경 추가
            UnityEngine.UI.Image bg = promptObj.AddComponent<UnityEngine.UI.Image>();
            bg.color = new Color(0, 0, 0, 0.7f);

            // 참조 저장
            interactPrompt = promptObj;
            promptText = textComponent;

            interactPrompt.SetActive(false);

            Debug.Log($"{objectName}: Interact prompt created successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"{objectName}: Failed to create interact prompt: {e.Message}");
        }
    }

    // Gizmo로 상호작용 범위 표시
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}

/// <summary>
/// 드랍 가능한 아이템 (여러 개 중 랜덤 선택)
/// </summary>
[System.Serializable]
public class LootItem
{
    public ItemData item;       // 아이템
    public int minAmount = 1;   // 최소 개수
    public int maxAmount = 3;   // 최대 개수
}