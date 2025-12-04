using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 플레이어의 인벤토리를 관리하는 시스템
/// </summary>
public class InventoryManager : MonoBehaviour
{
    [Header("Car Parts")]
    [SerializeField] private int requiredCarParts = 3; // 탈출에 필요한 부품 수
    private int currentCarParts = 0;

    [Header("Events")]
    public UnityEvent<int, int> OnCarPartsChanged; // (current, required)
    public UnityEvent OnAllPartsCollected;

    /// <summary>
    /// 자동차 부품 추가
    /// </summary>
    public void AddCarPart()
    {
        if (currentCarParts >= requiredCarParts) return;

        currentCarParts++;
        OnCarPartsChanged?.Invoke(currentCarParts, requiredCarParts);

        Debug.Log($"Car parts: {currentCarParts}/{requiredCarParts}");

        // 모든 부품을 모았을 때
        if (currentCarParts >= requiredCarParts)
        {
            OnAllPartsCollected?.Invoke();
            Debug.Log("All car parts collected! Find the car to escape!");
        }
    }

    /// <summary>
    /// 부품을 모두 모았는지 확인
    /// </summary>
    public bool HasAllCarParts()
    {
        return currentCarParts >= requiredCarParts;
    }

    /// <summary>
    /// 현재 부품 수 반환
    /// </summary>
    public int GetCurrentCarParts()
    {
        return currentCarParts;
    }

    /// <summary>
    /// 필요한 부품 수 반환
    /// </summary>
    public int GetRequiredCarParts()
    {
        return requiredCarParts;
    }

    /// <summary>
    /// 부품 수집 진행도 (0~1)
    /// </summary>
    public float GetCarPartsProgress()
    {
        return (float)currentCarParts / requiredCarParts;
    }
}
