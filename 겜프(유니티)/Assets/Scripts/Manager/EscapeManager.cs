using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 탈출 시스템 관리 (필수 아이템 수집 및 탈출)
/// </summary>
public class EscapeManager : MonoBehaviour
{
    public static EscapeManager Instance { get; private set; }

    [Header("Required Items")]
    [SerializeField] private List<ItemData> requiredItems = new List<ItemData>(); // ScriptableObject 직접 참조

    [Header("Escape Zone")]
    [SerializeField] private EscapeZone escapeZone;

    [Header("Item Spawn")]
    [SerializeField] private bool autoSpawnItems = true; // 자동으로 맵에 배치
    [SerializeField] private bool debugSpawn = true; // 스폰 위치 디버그

    private HashSet<string> collectedItems = new HashSet<string>();
    private Dictionary<InteractableObject, ItemData> spawnedItemLocations = new Dictionary<InteractableObject, ItemData>(); // 어디에 뭐가 있는지 추적
    private bool isRepaired = false;
    private bool hasEscaped = false;

    // 이벤트
    public delegate void ItemCollected(string itemName, int current, int total);
    public event ItemCollected OnItemCollected;

    public delegate void AllItemsCollected();
    public event AllItemsCollected OnAllItemsCollected;

    public delegate void VehicleRepaired();
    public event VehicleRepaired OnVehicleRepaired;

    public delegate void PlayerEscaped();
    public event PlayerEscaped OnPlayerEscaped;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Scene 로드 이벤트 구독
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeEscapeSystem();
    }

    /// <summary>
    /// Scene 로드 시 호출
    /// </summary>
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Debug.Log($"EscapeManager: Scene loaded - {scene.name}");
        InitializeEscapeSystem();
    }

    /// <summary>
    /// 탈출 시스템 초기화
    /// </summary>
    private void InitializeEscapeSystem()
    {
        // 상태 초기화
        collectedItems.Clear();
        spawnedItemLocations.Clear();
        isRepaired = false;
        hasEscaped = false;

        // 필수 아이템 재배치
        if (autoSpawnItems)
        {
            SpawnRequiredItems();
        }

        Debug.Log($"Escape Manager initialized. Required items: {requiredItems.Count}");
    }

    /// <summary>
    /// 필수 아이템을 랜덤 오브젝트에 배치
    /// </summary>
    private void SpawnRequiredItems()
    {
        if (requiredItems == null || requiredItems.Count == 0)
        {
            Debug.LogError("No required items assigned! Please add ItemData to Required Items list.");
            return;
        }

        // 모든 InteractableObject 찾기
        InteractableObject[] allObjects = FindObjectsOfType<InteractableObject>();
        List<InteractableObject> validObjects = new List<InteractableObject>();

        // 파밍 가능한 오브젝트만 필터링
        foreach (var obj in allObjects)
        {
            // Use Multiple Loot이 활성화된 오브젝트만
            if (obj.name.Contains("Car") || obj.name.Contains("Garbage") ||
                obj.name.Contains("TrashCan") || obj.name.Contains("Box") ||
                obj.name.Contains("Vending"))
            {
                validObjects.Add(obj);
            }
        }

        if (validObjects.Count < requiredItems.Count)
        {
            Debug.LogWarning($"Not enough farmable objects! Need {requiredItems.Count}, found {validObjects.Count}");
            return;
        }

        Debug.Log($"Found {validObjects.Count} valid objects for spawning {requiredItems.Count} key items");

        // Fisher-Yates 셔플
        for (int i = validObjects.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            var temp = validObjects[i];
            validObjects[i] = validObjects[randomIndex];
            validObjects[randomIndex] = temp;
        }

        // 필수 아이템 배치
        for (int i = 0; i < requiredItems.Count; i++)
        {
            ItemData itemData = requiredItems[i];
            InteractableObject targetObject = validObjects[i];

            // 아이템을 오브젝트의 loot에 추가
            AddItemToObject(targetObject, itemData);

            // 추적 저장
            spawnedItemLocations[targetObject] = itemData;

            if (debugSpawn)
            {
                Debug.Log($"[SPAWN] '{itemData.itemName}' placed at {targetObject.name} (Position: {targetObject.transform.position})");
            }
        }

        Debug.Log($"Successfully spawned {requiredItems.Count} key items in random locations!");
    }

    /// <summary>
    /// InteractableObject에 아이템 추가 (public 필드 직접 수정)
    /// </summary>
    private void AddItemToObject(InteractableObject obj, ItemData itemData)
    {
        // possibleLoot 필드 찾기 (SerializeField라서 GetField 필요)
        var possibleLootField = typeof(InteractableObject).GetField("possibleLoot",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (possibleLootField != null)
        {
            var currentLoot = possibleLootField.GetValue(obj) as LootItem[];

            if (currentLoot != null)
            {
                // 새 배열 생성 (기존 + 1)
                LootItem[] newLoot = new LootItem[currentLoot.Length + 1];

                // 새 KeyItem을 맨 앞에 추가
                newLoot[0] = new LootItem
                {
                    item = itemData,
                    minAmount = 1,
                    maxAmount = 1
                };

                // 기존 아이템들 복사
                for (int i = 0; i < currentLoot.Length; i++)
                {
                    newLoot[i + 1] = currentLoot[i];
                }

                // 새 배열 설정
                possibleLootField.SetValue(obj, newLoot);

                Debug.Log($"Successfully added {itemData.itemName} to {obj.name}'s loot table (Total: {newLoot.Length} items)");
            }
            else
            {
                Debug.LogWarning($"possibleLoot is null for {obj.name}");
            }
        }
        else
        {
            Debug.LogError("Could not find possibleLoot field!");
        }
    }

    /// <summary>
    /// 필수 아이템 수집
    /// </summary>
    public void CollectKeyItem(string itemName)
    {
        if (collectedItems.Contains(itemName))
        {
            Debug.Log($"Already collected: {itemName}");
            return;
        }

        // requiredItems에 있는지 확인
        bool isRequired = false;
        foreach (var item in requiredItems)
        {
            if (item != null && item.itemName == itemName)
            {
                isRequired = true;
                break;
            }
        }

        if (isRequired)
        {
            collectedItems.Add(itemName);
            Debug.Log($"[KEY ITEM] Collected: {itemName} ({collectedItems.Count}/{requiredItems.Count})");

            OnItemCollected?.Invoke(itemName, collectedItems.Count, requiredItems.Count);

            // 모든 아이템 수집 확인
            if (collectedItems.Count >= requiredItems.Count)
            {
                Debug.Log("=== ALL KEY ITEMS COLLECTED! Head to the escape zone! ===");
                OnAllItemsCollected?.Invoke();

                // 탈출 존 활성화
                if (escapeZone != null)
                {
                    escapeZone.EnableEscape();
                }
            }
        }
    }

    /// <summary>
    /// 차량 수리
    /// </summary>
    public void RepairVehicle()
    {
        if (!HasAllItems())
        {
            Debug.Log("Cannot repair: Missing required items!");
            return;
        }

        if (isRepaired)
        {
            Debug.Log("Vehicle already repaired!");
            return;
        }

        isRepaired = true;
        Debug.Log("Vehicle repaired! Press F to escape!");
        OnVehicleRepaired?.Invoke();

        // 탈출 존에 알림
        if (escapeZone != null)
        {
            escapeZone.SetRepaired(true);
        }
    }

    /// <summary>
    /// 탈출 실행
    /// </summary>
    public void Escape()
    {
        if (!isRepaired)
        {
            Debug.Log("Cannot escape: Vehicle not repaired!");
            return;
        }

        if (hasEscaped)
        {
            Debug.Log("Already escaped!");
            return;
        }

        hasEscaped = true;
        Debug.Log("Player escaped! Victory!");
        OnPlayerEscaped?.Invoke();

        // 승리 처리
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Victory();
        }
    }

    /// <summary>
    /// 모든 필수 아이템을 수집했는지 확인
    /// </summary>
    public bool HasAllItems()
    {
        return collectedItems.Count >= requiredItems.Count;
    }

    /// <summary>
    /// 특정 아이템을 가지고 있는지 확인
    /// </summary>
    public bool HasItem(string itemName)
    {
        return collectedItems.Contains(itemName);
    }

    /// <summary>
    /// 수집된 아이템 개수
    /// </summary>
    public int GetCollectedCount()
    {
        return collectedItems.Count;
    }

    /// <summary>
    /// 필요한 아이템 개수
    /// </summary>
    public int GetRequiredCount()
    {
        return requiredItems.Count;
    }

    /// <summary>
    /// 수리 완료 여부
    /// </summary>
    public bool IsRepaired()
    {
        return isRepaired;
    }

    /// <summary>
    /// 탈출 완료 여부
    /// </summary>
    public bool HasEscaped()
    {
        return hasEscaped;
    }

    /// <summary>
    /// 필수 아이템 목록
    /// </summary>
    public List<ItemData> GetRequiredItems()
    {
        return new List<ItemData>(requiredItems);
    }

    /// <summary>
    /// 수집된 아이템 목록
    /// </summary>
    public HashSet<string> GetCollectedItems()
    {
        return new HashSet<string>(collectedItems);
    }

    /// <summary>
    /// 특정 오브젝트에 어떤 아이템이 있는지 확인 (디버그용)
    /// </summary>
    public ItemData GetItemAtLocation(InteractableObject obj)
    {
        if (spawnedItemLocations.ContainsKey(obj))
        {
            return spawnedItemLocations[obj];
        }
        return null;
    }

    private void OnDestroy()
    {
        // Scene 로드 이벤트 구독 해제
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}