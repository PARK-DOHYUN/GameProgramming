using UnityEngine;
using UnityEngine.Events;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private MeleeWeapon bareHands;
    [SerializeField] private MeleeWeapon bat;
    [SerializeField] private RangedWeapon rifle;

    [Header("Animators")]
    [SerializeField] private RuntimeAnimatorController handAnimator;
    [SerializeField] private RuntimeAnimatorController batAnimator;
    [SerializeField] private RuntimeAnimatorController gunAnimator;

    private WeaponBase currentWeapon;

    public enum WeaponType { BareHands, Bat, Rifle }
    private WeaponType currentWeaponType = WeaponType.BareHands;

    [Header("Events")]
    public UnityEvent<WeaponType> OnWeaponChanged;

    [Header("UI")]
    [SerializeField] private AmmoDisplayUI ammoDisplayUI;

    private PlayerController playerController;
    private Animator handsAnimator;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        handsAnimator = playerController.GetHandsAnimator();

        // AmmoDisplayUI 자동 찾기
        if (ammoDisplayUI == null)
        {
            ammoDisplayUI = FindObjectOfType<AmmoDisplayUI>();
        }

        SwitchWeapon(WeaponType.BareHands);
    }

    private void Update()
    {
        // 공격 입력 (좌클릭 또는 스페이스바)
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }

        // 무기 전환 (1, 2, 3)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(WeaponType.BareHands);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && bat != null)
        {
            SwitchWeapon(WeaponType.Bat);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && rifle != null)
        {
            SwitchWeapon(WeaponType.Rifle);
        }
    }

    private void Attack()
    {
        if (currentWeapon == null) return;

        // 바라보는 방향으로 공격
        Vector2 attackDirection = playerController.GetLookDirection();
        currentWeapon.Attack(attackDirection);

        // Hands Animator에 Attack Trigger
        if (handsAnimator != null)
        {
            handsAnimator.SetTrigger("Attack");
        }
    }

    public void SwitchWeapon(WeaponType weaponType)
    {
        // 기존 무기 비활성화
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(false);
        }

        // Hand 회전 컴포넌트 찾기
        HandRotation handRotation = handsAnimator?.GetComponent<HandRotation>();

        // 무기별 Animator 전환
        RuntimeAnimatorController targetAnimator = null;
        bool enableHandRotation = false; // Hand 회전 사용 여부

        switch (weaponType)
        {
            case WeaponType.BareHands:
                currentWeapon = bareHands;
                targetAnimator = handAnimator;
                enableHandRotation = false; // 맨손은 회전 안 함
                break;

            case WeaponType.Bat:
                if (bat != null && !bat.IsBroken())
                {
                    currentWeapon = bat;
                    targetAnimator = batAnimator;
                    enableHandRotation = false; // 배트는 회전 안 함
                }
                else
                {
                    Debug.Log("Bat not available!");
                    return;
                }
                break;

            case WeaponType.Rifle:
                if (rifle != null)
                {
                    currentWeapon = rifle;
                    targetAnimator = gunAnimator;
                    enableHandRotation = true; // 라이플은 회전
                }
                else
                {
                    Debug.Log("Rifle not available!");
                    return;
                }
                break;
        }

        // Animator 전환
        if (handsAnimator != null && targetAnimator != null)
        {
            handsAnimator.runtimeAnimatorController = targetAnimator;
        }

        // Hand 회전 켜기/끄기
        if (handRotation != null)
        {
            handRotation.SetRotationEnabled(enableHandRotation);
        }

        // 무기 활성화
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(true);
            currentWeaponType = weaponType;
            OnWeaponChanged?.Invoke(weaponType);
            Debug.Log($"Switched to {weaponType}");

            // AmmoDisplay UI 업데이트
            UpdateAmmoDisplay();
        }
    }

    /// <summary>
    /// 탄약 UI 업데이트
    /// </summary>
    private void UpdateAmmoDisplay()
    {
        if (ammoDisplayUI == null) return;

        // 라이플일 때만 표시
        if (currentWeaponType == WeaponType.Rifle && rifle != null)
        {
            ammoDisplayUI.SetWeapon(rifle);
            ammoDisplayUI.Show(true);
        }
        else
        {
            // 맨손/배트일 때는 숨김
            ammoDisplayUI.Show(false);
        }
    }

    public WeaponBase GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponType GetCurrentWeaponType()
    {
        return currentWeaponType;
    }

    public RangedWeapon GetRifle()
    {
        return rifle;
    }

    public MeleeWeapon GetBat()
    {
        return bat;
    }

    public void AcquireBat(MeleeWeapon newBat)
    {
        bat = newBat;
    }

    public void AcquireRifle(RangedWeapon newRifle)
    {
        rifle = newRifle;
    }

    public void AddAmmo(int amount)
    {
        if (rifle != null)
        {
            rifle.AddAmmo(amount);
        }
    }
}