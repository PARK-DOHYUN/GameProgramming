using UnityEngine;

/// <summary>
/// 탄약 아이템 (플레이어가 먹으면 탄약 증가)
/// </summary>
public class AmmoItem : MonoBehaviour
{
    [Header("Ammo Settings")]
    [SerializeField] private int ammoAmount = 10; // 줄 탄약 개수

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip pickupSound;

    private bool isPickedUp = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPickedUp) return;

        if (collision.CompareTag("Player"))
        {
            // Player에서 WeaponManager 찾기
            WeaponManager weaponManager = collision.GetComponent<WeaponManager>();

            if (weaponManager != null)
            {
                // 현재 무기가 라이플인지 확인
                WeaponBase currentWeapon = weaponManager.GetCurrentWeapon();
                RangedWeapon rifle = currentWeapon as RangedWeapon;

                // 라이플이 아니면 모든 무기에서 찾기
                if (rifle == null)
                {
                    rifle = collision.GetComponentInChildren<RangedWeapon>();
                }

                if (rifle != null)
                {
                    // 탄약 추가
                    rifle.AddAmmo(ammoAmount);

                    Debug.Log($"Picked up {ammoAmount} bullets!");

                    // 사운드 재생 (있으면)
                    if (pickupSound != null)
                    {
                        AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                    }

                    // 아이템 제거
                    isPickedUp = true;
                    Destroy(gameObject);
                }
            }
        }
    }

    // 에디터에서 보이도록 Gizmo
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}