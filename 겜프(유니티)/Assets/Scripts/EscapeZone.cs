using UnityEngine;
using System.Collections;

/// <summary>
/// 탈출 존 (차량 수리 및 탈출 지점)
/// </summary>
public class EscapeZone : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [SerializeField] private float repairDuration = 3f; // 수리 시간

    [Header("UI")]
    [SerializeField] private GameObject interactPrompt;
    [SerializeField] private RepairProgressUI repairProgressUI;

    [Header("Messages")]
    [SerializeField] private string needItemsMessage = "Need all items!";
    [SerializeField] private string repairMessage = "Hold F to Repair";
    [SerializeField] private string escapeMessage = "Press F to Escape!";

    private TMPro.TextMeshProUGUI promptTextComponent;

    private Transform player;
    private bool playerInRange = false;
    private bool canEscape = false; // 모든 아이템 수집 완료
    private bool isRepaired = false;
    private bool isRepairing = false;
    private float repairProgress = 0f;

    private void Start()
    {
        // 플레이어 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // UI 초기화
        if (interactPrompt != null)
        {
            // TextMeshPro 컴포넌트 찾기
            promptTextComponent = interactPrompt.GetComponentInChildren<TMPro.TextMeshProUGUI>();

            if (promptTextComponent == null)
            {
                Debug.LogWarning("PromptText (TextMeshProUGUI) not found in Canvas!");
            }

            interactPrompt.SetActive(false);
        }

        // EscapeManager 이벤트 구독
        if (EscapeManager.Instance != null)
        {
            EscapeManager.Instance.OnAllItemsCollected += OnAllItemsCollected;
        }
    }

    private void Update()
    {
        if (player == null) return;

        // 플레이어 거리 체크
        float distance = Vector2.Distance(transform.position, player.position);
        bool inRange = distance <= interactionRange;

        if (inRange != playerInRange)
        {
            playerInRange = inRange;
            UpdatePrompt();
        }

        // 상호작용
        if (playerInRange)
        {
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        // 수리 완료 후 탈출
        if (isRepaired)
        {
            if (Input.GetKeyDown(interactKey))
            {
                Escape();
            }
        }
        // 수리 중
        else if (canEscape)
        {
            if (Input.GetKey(interactKey))
            {
                if (!isRepairing)
                {
                    StartCoroutine(RepairVehicle());
                }
            }
            else if (Input.GetKeyUp(interactKey))
            {
                // 수리 취소
                if (isRepairing)
                {
                    StopAllCoroutines();
                    isRepairing = false;
                    repairProgress = 0f;

                    // 수리 소리 중지
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.StopRepair();
                    }

                    // 진행바 숨기기
                    if (repairProgressUI != null)
                    {
                        repairProgressUI.Hide();
                    }

                    Debug.Log("Repair cancelled!");
                }
            }
        }
    }

    private IEnumerator RepairVehicle()
    {
        isRepairing = true;
        repairProgress = 0f;

        Debug.Log("Repairing vehicle...");

        // 수리 소리 시작 (루프)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayRepair();
        }

        // 진행바 표시
        if (repairProgressUI != null)
        {
            repairProgressUI.Show();
        }

        while (repairProgress < repairDuration)
        {
            repairProgress += Time.deltaTime;
            float progress = repairProgress / repairDuration;

            // 진행바 업데이트
            if (repairProgressUI != null)
            {
                repairProgressUI.UpdateProgress(progress);
            }

            yield return null;
        }

        // 진행바 숨기기
        if (repairProgressUI != null)
        {
            repairProgressUI.Hide();
        }

        // 수리 완료 소리
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayRepairComplete();
        }

        // 수리 완료
        isRepairing = false;
        repairProgress = 0f;

        if (EscapeManager.Instance != null)
        {
            EscapeManager.Instance.RepairVehicle();
        }

        SetRepaired(true);
    }

    private void Escape()
    {
        Debug.Log("Escaping...");

        // 탈출 소리
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEscape();
        }

        if (EscapeManager.Instance != null)
        {
            EscapeManager.Instance.Escape();
        }

        // UI 숨기기
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    private void UpdatePrompt()
    {
        if (interactPrompt == null)
        {
            Debug.LogWarning("Interact Prompt (Canvas) is not assigned!");
            return;
        }

        if (playerInRange)
        {
            // 메시지 결정
            string message = "";

            if (isRepaired)
            {
                message = escapeMessage; // "Press F to Escape!"
            }
            else if (canEscape)
            {
                message = repairMessage; // "Hold F to Repair"
            }
            else
            {
                message = needItemsMessage; // "Need all items!"
            }

            // 텍스트 업데이트
            if (promptTextComponent != null)
            {
                promptTextComponent.text = message;
                Debug.Log($"EscapeZone: Updated text to '{message}'");
            }

            interactPrompt.SetActive(true);
        }
        else
        {
            interactPrompt.SetActive(false);
        }
    }

    public void EnableEscape()
    {
        canEscape = true;
        Debug.Log("Escape zone enabled! You can now repair the vehicle.");
        UpdatePrompt();
    }

    public void SetRepaired(bool repaired)
    {
        isRepaired = repaired;
        UpdatePrompt();
    }

    private void OnAllItemsCollected()
    {
        EnableEscape();
    }

    private void OnDrawGizmosSelected()
    {
        // 상호작용 범위 표시
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (EscapeManager.Instance != null)
        {
            EscapeManager.Instance.OnAllItemsCollected -= OnAllItemsCollected;
        }
    }
}