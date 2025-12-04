using UnityEngine;

/// <summary>
/// 총알 동작을 처리하는 스크립트
/// </summary>
public class Bullet : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float damage;
    private LayerMask enemyLayer;

    [Header("Bullet Settings")]
    [SerializeField] private float lifetime = 5f; // 총알 생존 시간

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 총알 초기화
    /// </summary>
    public void Initialize(Vector2 dir, float spd, float dmg, LayerMask layer)
    {
        direction = dir.normalized;
        speed = spd;
        damage = dmg;
        enemyLayer = layer;

        // 총알 발사
        rb.velocity = direction * speed;

        // 총알 회전 (진행 방향으로)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // 일정 시간 후 자동 삭제
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 적과 충돌 체크
        if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
        {
            ZombieHealth zombieHealth = collision.GetComponent<ZombieHealth>();
            if (zombieHealth != null)
            {
                zombieHealth.TakeDamage(damage);
                Debug.Log($"Bullet hit {collision.name} for {damage} damage");
            }

            // 총알 파괴
            Destroy(gameObject);
        }
        // 벽이나 장애물과 충돌 (태그 안전 체크)
        else
        {
            try
            {
                if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Obstacle"))
                {
                    Destroy(gameObject);
                }
            }
            catch (UnityException)
            {
                // 태그가 정의되지 않은 경우 무시
            }
        }
    }
}