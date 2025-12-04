using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 탄약 UI 표시 (텍스트 + 총알 아이콘)
/// </summary>
public class AmmoDisplayUI : MonoBehaviour
{
    public static AmmoDisplayUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Transform bulletIconsContainer;
    [SerializeField] private GameObject bulletIconPrefab;

    [Header("Bullet Sprites")]
    [SerializeField] private Sprite bulletFull;
    [SerializeField] private Sprite bulletEmpty;

    [Header("Settings")]
    [SerializeField] private int maxBulletIcons = 10; // 표시할 최대 총알 아이콘 개수

    private List<Image> bulletIcons = new List<Image>();
    private RangedWeapon currentWeapon;

    private void Awake()
    {
        // 싱글톤 설정
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
        // 초기화
        UpdateAmmoDisplay(0, 0);
    }

    /// <summary>
    /// 탄약 UI 업데이트 (RangedWeapon 이벤트에서 호출)
    /// </summary>
    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        UpdateAmmoDisplay(currentAmmo, maxAmmo);
    }

    /// <summary>
    /// 탄약 UI 업데이트
    /// </summary>
    public void UpdateAmmoDisplay(int currentAmmo, int maxAmmo)
    {
        // 텍스트 업데이트 (현재 / 최대)
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo} / {maxAmmo}";
        }

        // 총알 아이콘 업데이트
        UpdateBulletIcons(currentAmmo);
    }

    /// <summary>
    /// 총알 아이콘 생성
    /// </summary>
    private void CreateBulletIcons(int count)
    {
        // 기존 아이콘 삭제
        foreach (var icon in bulletIcons)
        {
            if (icon != null)
                Destroy(icon.gameObject);
        }
        bulletIcons.Clear();

        // 새 아이콘 생성
        int iconCount = Mathf.Min(count, maxBulletIcons);

        for (int i = 0; i < iconCount; i++)
        {
            GameObject iconObj;

            if (bulletIconPrefab != null)
            {
                iconObj = Instantiate(bulletIconPrefab, bulletIconsContainer);
            }
            else
            {
                // Prefab이 없으면 수동 생성
                iconObj = new GameObject($"Bullet{i + 1}");
                iconObj.transform.SetParent(bulletIconsContainer);
                Image img = iconObj.AddComponent<Image>();
                img.sprite = bulletFull;
                img.raycastTarget = false;

                RectTransform rt = iconObj.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(20, 40);
            }

            Image iconImage = iconObj.GetComponent<Image>();
            if (iconImage != null)
            {
                bulletIcons.Add(iconImage);
            }
        }
    }

    /// <summary>
    /// 총알 아이콘 업데이트
    /// </summary>
    private void UpdateBulletIcons(int currentAmmo)
    {
        // 아이콘이 없으면 생성
        if (bulletIcons.Count == 0)
        {
            CreateBulletIcons(maxBulletIcons);
        }

        // 각 아이콘 업데이트
        for (int i = 0; i < bulletIcons.Count; i++)
        {
            if (bulletIcons[i] != null)
            {
                // i < currentAmmo면 가득 찬 총알, 아니면 빈 총알
                if (i < currentAmmo)
                {
                    bulletIcons[i].sprite = bulletFull;
                    bulletIcons[i].color = Color.white;
                }
                else
                {
                    bulletIcons[i].sprite = bulletEmpty;
                    bulletIcons[i].color = new Color(1f, 1f, 1f, 0.3f); // 투명하게
                }
            }
        }
    }

    /// <summary>
    /// 무기 변경 시 호출
    /// </summary>
    public void SetWeapon(RangedWeapon weapon)
    {
        currentWeapon = weapon;

        if (weapon != null)
        {
            UpdateAmmoDisplay(weapon.GetCurrentAmmo(), weapon.GetMaxAmmo());
            gameObject.SetActive(true);
        }
        else
        {
            // 근접 무기면 숨김
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// UI 표시/숨김
    /// </summary>
    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }
}