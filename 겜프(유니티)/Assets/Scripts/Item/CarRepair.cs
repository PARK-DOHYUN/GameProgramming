using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 자동차 수리 및 탈출 시스템
/// </summary>
public class CarRepair : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    [Header("UI")]
    [SerializeField] private GameObject interactionPrompt; // "Press E to Repair Car" UI

    [Header("Events")]
    public UnityEvent OnCarRepaired;

    private Transform player;
    private InventoryManager playerInventory;
    private bool isPlayerNearby = false;
    private bool isRepaired = false;

    private void Start()
    {
        // 플레이어 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerInventory = playerObj.GetComponent<InventoryManager>();
        }

        // UI 초기화
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        if (isRepaired || player == null) return;

        // 플레이어와의 거리 체크
        float distance = Vector2.Distance(transform.position, player.position);
        isPlayerNearby = distance <= interactionRange;

        // UI 표시
        if (interactionPrompt != null)
        {
            // 모든 부품을 모았고 근처에 있을 때만 UI 표시
            bool shouldShowPrompt = isPlayerNearby && 
                                   playerInventory != null && 
                                   playerInventory.HasAllCarParts();
            interactionPrompt.SetActive(shouldShowPrompt);
        }

        // 상호작용 입력
        if (isPlayerNearby && Input.GetKeyDown(interactionKey))
        {
            TryRepairCar();
        }
    }

    /// <summary>
    /// 자동차 수리 시도
    /// </summary>
    private void TryRepairCar()
    {
        if (isRepaired) return;

        // 인벤토리 체크
        if (playerInventory == null)
        {
            Debug.Log("Player inventory not found!");
            return;
        }

        // 부품을 모두 모았는지 확인
        if (playerInventory.HasAllCarParts())
        {
            RepairCar();
        }
        else
        {
            int current = playerInventory.GetCurrentCarParts();
            int required = playerInventory.GetRequiredCarParts();
            Debug.Log($"Need more parts! ({current}/{required})");
        }
    }

    /// <summary>
    /// 자동차 수리 및 탈출
    /// </summary>
    private void RepairCar()
    {
        isRepaired = true;

        // 수리 이벤트 발생
        OnCarRepaired?.Invoke();

        // UI 숨기기
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        Debug.Log("Car repaired! Escaping...");

        // 게임 승리 처리
        GameManager.Instance?.Victory();
    }

    /// <summary>
    /// 수리 완료 여부 반환
    /// </summary>
    public bool IsRepaired()
    {
        return isRepaired;
    }

    // 디버그용 기즈모
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
