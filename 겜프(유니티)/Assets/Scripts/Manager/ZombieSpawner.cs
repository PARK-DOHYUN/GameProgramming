using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 좀비 스포너 시스템
/// </summary>
public class ZombieSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject zombiePrefab; // 좀비 프리팹
    [SerializeField] private Transform[] spawnPoints; // 스폰 포인트들
    [SerializeField] private float spawnInterval = 5f; // 스폰 간격 (초)
    [SerializeField] private int maxZombies = 10; // 최대 좀비 수

    [Header("Difficulty Settings")]
    [SerializeField] private bool increaseDifficulty = true; // 난이도 증가 여부
    [SerializeField] private float difficultyIncreaseInterval = 60f; // 난이도 증가 간격 (초)
    [SerializeField] private float minSpawnInterval = 1f; // 최소 스폰 간격
    [SerializeField] private int maxZombiesIncrease = 2; // 난이도 증가 시 최대 좀비 증가량

    [Header("Auto Generate Spawn Points")]
    [SerializeField] private bool autoGenerateSpawnPoints = true;
    [SerializeField] private float spawnRadius = 15f; // 플레이어로부터의 스폰 반경
    [SerializeField] private int autoSpawnPointCount = 8; // 자동 생성할 스폰 포인트 개수

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;

    private List<GameObject> activeZombies = new List<GameObject>();
    private float nextSpawnTime;
    private float nextDifficultyIncreaseTime;
    private Transform player;
    private int currentWave = 1;

    private void Start()
    {
        // 플레이어 찾기
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure Player has 'Player' tag.");
            enabled = false;
            return;
        }

        // 스폰 포인트 자동 생성
        if (autoGenerateSpawnPoints && (spawnPoints == null || spawnPoints.Length == 0))
        {
            GenerateSpawnPoints();
        }

        // 프리팹 체크
        if (zombiePrefab == null)
        {
            Debug.LogError("Zombie prefab is not assigned!");
            enabled = false;
            return;
        }

        // 초기 시간 설정
        nextSpawnTime = Time.time + spawnInterval;
        nextDifficultyIncreaseTime = Time.time + difficultyIncreaseInterval;

        Debug.Log($"Zombie Spawner started! Max zombies: {maxZombies}, Spawn interval: {spawnInterval}s");
    }

    private void Update()
    {
        // 죽은 좀비 리스트에서 제거
        activeZombies.RemoveAll(zombie => zombie == null);

        // 스폰 타이머
        if (Time.time >= nextSpawnTime)
        {
            TrySpawnZombie();
            nextSpawnTime = Time.time + spawnInterval;
        }

        // 난이도 증가
        if (increaseDifficulty && Time.time >= nextDifficultyIncreaseTime)
        {
            IncreaseDifficulty();
            nextDifficultyIncreaseTime = Time.time + difficultyIncreaseInterval;
        }
    }

    /// <summary>
    /// 좀비 스폰 시도
    /// </summary>
    private void TrySpawnZombie()
    {
        // 최대 좀비 수 체크
        if (activeZombies.Count >= maxZombies)
        {
            if (showDebugInfo)
                Debug.Log("Max zombies reached. Waiting...");
            return;
        }

        // 스폰 포인트가 없으면 리턴
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points available!");
            return;
        }

        // 랜덤 스폰 포인트 선택
        Transform spawnPoint = GetRandomSpawnPoint();

        if (spawnPoint != null)
        {
            SpawnZombie(spawnPoint.position);
        }
    }

    /// <summary>
    /// 좀비 생성
    /// </summary>
    private void SpawnZombie(Vector3 position)
    {
        // 콜라이더 체크 (건물, 벽 등)
        Vector3 validPosition = FindValidSpawnPosition(position);

        if (validPosition == Vector3.zero)
        {
            if (showDebugInfo)
                Debug.LogWarning($"Could not find valid spawn position near {position}");
            return;
        }

        GameObject zombie = Instantiate(zombiePrefab, validPosition, Quaternion.identity);
        zombie.transform.SetParent(transform); // Spawner 하위로 정리
        activeZombies.Add(zombie);

        if (showDebugInfo)
            Debug.Log($"Zombie spawned at {validPosition}. Active zombies: {activeZombies.Count}/{maxZombies}");
    }

    /// <summary>
    /// 유효한 스폰 위치 찾기 (콜라이더 회피)
    /// </summary>
    private Vector3 FindValidSpawnPosition(Vector3 targetPosition)
    {
        // 1. 원래 위치가 유효한지 체크
        if (IsPositionValid(targetPosition))
        {
            return targetPosition;
        }

        // 2. 주변에서 유효한 위치 찾기 (최대 10번 시도)
        int maxAttempts = 10;
        float searchRadius = 3f; // 탐색 반경

        for (int i = 0; i < maxAttempts; i++)
        {
            // 랜덤한 오프셋 생성
            Vector2 randomOffset = Random.insideUnitCircle * searchRadius;
            Vector3 testPosition = targetPosition + new Vector3(randomOffset.x, randomOffset.y, 0);

            if (IsPositionValid(testPosition))
            {
                return testPosition;
            }
        }

        // 유효한 위치를 찾지 못함
        return Vector3.zero;
    }

    /// <summary>
    /// 위치가 유효한지 체크 (콜라이더 없는지)
    /// </summary>
    private bool IsPositionValid(Vector3 position)
    {
        // Physics2D.OverlapCircle로 해당 위치에 콜라이더가 있는지 체크
        float checkRadius = 0.3f; // 좀비 크기 고려

        // 벽, 건물 등과 겹치는지 체크
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, checkRadius);

        foreach (Collider2D col in colliders)
        {
            // 플레이어, 좀비, 트리거는 무시
            if (col.CompareTag("Player") ||
                col.CompareTag("Enemy") ||
                col.isTrigger)
            {
                continue;
            }

            // 다른 콜라이더가 있으면 유효하지 않음
            return false;
        }

        // 콜라이더 없음 = 유효한 위치
        return true;
    }

    /// <summary>
    /// 랜덤 스폰 포인트 선택
    /// </summary>
    private Transform GetRandomSpawnPoint()
    {
        // 플레이어와 거리가 너무 가까운 스폰 포인트 제외
        List<Transform> validSpawnPoints = new List<Transform>();

        foreach (Transform sp in spawnPoints)
        {
            if (sp != null)
            {
                float distance = Vector3.Distance(sp.position, player.position);
                if (distance >= 5f) // 최소 5 거리 유지
                {
                    validSpawnPoints.Add(sp);
                }
            }
        }

        if (validSpawnPoints.Count > 0)
        {
            return validSpawnPoints[Random.Range(0, validSpawnPoints.Count)];
        }

        // 유효한 스폰 포인트가 없으면 그냥 랜덤
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    /// <summary>
    /// 난이도 증가
    /// </summary>
    private void IncreaseDifficulty()
    {
        currentWave++;

        // 스폰 간격 감소
        spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval * 0.9f);

        // 최대 좀비 수 증가
        maxZombies += maxZombiesIncrease;

        Debug.Log($"=== WAVE {currentWave} ===");
        Debug.Log($"Difficulty increased! Spawn interval: {spawnInterval:F1}s, Max zombies: {maxZombies}");
    }

    /// <summary>
    /// 스폰 포인트 자동 생성
    /// </summary>
    private void GenerateSpawnPoints()
    {
        List<Transform> generatedPoints = new List<Transform>();

        // 원형으로 배치
        for (int i = 0; i < autoSpawnPointCount; i++)
        {
            float angle = i * (360f / autoSpawnPointCount);
            float rad = angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Cos(rad) * spawnRadius,
                Mathf.Sin(rad) * spawnRadius,
                0
            );

            Vector3 spawnPos = player.position + offset;

            // 스폰 포인트 오브젝트 생성
            GameObject spawnPoint = new GameObject($"SpawnPoint_{i + 1}");
            spawnPoint.transform.position = spawnPos;
            spawnPoint.transform.SetParent(transform);

            generatedPoints.Add(spawnPoint.transform);
        }

        spawnPoints = generatedPoints.ToArray();

        Debug.Log($"Auto-generated {autoSpawnPointCount} spawn points at radius {spawnRadius}");
    }

    /// <summary>
    /// 현재 활성 좀비 수 반환
    /// </summary>
    public int GetActiveZombieCount()
    {
        activeZombies.RemoveAll(zombie => zombie == null);
        return activeZombies.Count;
    }

    /// <summary>
    /// 현재 웨이브 반환
    /// </summary>
    public int GetCurrentWave()
    {
        return currentWave;
    }

    /// <summary>
    /// 즉시 좀비 스폰 (테스트용)
    /// </summary>
    public void SpawnZombieNow()
    {
        TrySpawnZombie();
    }

    /// <summary>
    /// 모든 좀비 제거 (테스트용)
    /// </summary>
    public void ClearAllZombies()
    {
        foreach (GameObject zombie in activeZombies)
        {
            if (zombie != null)
                Destroy(zombie);
        }
        activeZombies.Clear();
        Debug.Log("All zombies cleared!");
    }

    // Gizmo로 스폰 포인트 표시
    private void OnDrawGizmos()
    {
        if (spawnPoints != null)
        {
            Gizmos.color = Color.red;
            foreach (Transform sp in spawnPoints)
            {
                if (sp != null)
                {
                    Gizmos.DrawWireSphere(sp.position, 0.5f);
                    Gizmos.DrawLine(sp.position, sp.position + Vector3.up * 2f);
                }
            }
        }

        // 스폰 반경 표시
        if (player != null && autoGenerateSpawnPoints)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, spawnRadius);
        }
    }
}