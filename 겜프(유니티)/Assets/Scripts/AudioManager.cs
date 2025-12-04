using UnityEngine;

/// <summary>
/// 사운드 관리 시스템 (싱글톤)
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource; // 배경음악
    [SerializeField] private AudioSource sfxSource; // 효과음
    [SerializeField] private AudioSource ambientSource; // 환경음

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip victoryMusic;
    [SerializeField] private AudioClip defeatMusic;

    [Header("Player Sounds")]
    [SerializeField] private AudioClip[] footstepSounds; // 발소리 여러 개
    [SerializeField] private AudioClip playerHurtSound;
    [SerializeField] private AudioClip playerDeathSound;

    [Header("Weapon Sounds")]
    [SerializeField] private AudioClip rifleShootSound;
    [SerializeField] private AudioClip meleeSwingSound;
    [SerializeField] private AudioClip meleeHitSound;
    [SerializeField] private AudioClip reloadSound;

    [Header("Zombie Sounds")]
    [SerializeField] private AudioClip[] zombieIdleSounds; // 좀비 으르렁
    [SerializeField] private AudioClip[] zombieAttackSounds; // 좀비 공격
    [SerializeField] private AudioClip zombieHitSound;
    [SerializeField] private AudioClip zombieDeathSound;

    [Header("Item Sounds")]
    [SerializeField] private AudioClip itemPickupSound;
    [SerializeField] private AudioClip itemUseSound;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioClip ammoPickupSound;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip buttonHoverSound;

    [Header("Escape Sounds")]
    [SerializeField] private AudioClip repairSound;
    [SerializeField] private AudioClip repairCompleteSound;
    [SerializeField] private AudioClip escapeSound;

    [Header("Volume Settings")]
    [SerializeField][Range(0f, 1f)] private float musicVolume = 0.3f; // 배경음 낮춤
    [SerializeField][Range(0f, 1f)] private float sfxVolume = 1.0f; // 효과음 높임
    [SerializeField][Range(0f, 1f)] private float ambientVolume = 0.2f;

    [Header("Individual Volume Multipliers")]
    [SerializeField][Range(0f, 2f)] private float weaponSoundVolume = 1.0f;
    [SerializeField][Range(0f, 2f)] private float zombieSoundVolume = 0.8f;
    [SerializeField][Range(0f, 2f)] private float playerSoundVolume = 1.0f;
    [SerializeField][Range(0f, 2f)] private float itemSoundVolume = 0.7f;
    [SerializeField][Range(0f, 2f)] private float uiSoundVolume = 0.6f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        // AudioSource 자동 생성 (없으면)
        AudioSource[] sources = GetComponents<AudioSource>();

        // Music Source
        if (musicSource == null)
        {
            if (sources.Length > 0)
            {
                musicSource = sources[0];
            }
            else
            {
                musicSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Music 설정 강제 적용
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.priority = 64;
        musicSource.volume = musicVolume;

        // SFX Source
        if (sfxSource == null)
        {
            if (sources.Length > 1)
            {
                sfxSource = sources[1];
            }
            else
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // SFX 설정 강제 적용 (중요!)
        sfxSource.loop = false; // 절대 Loop 하면 안 됨!
        sfxSource.playOnAwake = false;
        sfxSource.priority = 128;
        sfxSource.volume = sfxVolume;

        // Ambient Source
        if (ambientSource == null)
        {
            if (sources.Length > 2)
            {
                ambientSource = sources[2];
            }
            else
            {
                ambientSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Ambient 설정 강제 적용
        ambientSource.loop = true;
        ambientSource.playOnAwake = false;
        ambientSource.priority = 32;
        ambientSource.volume = ambientVolume;

        Debug.Log($"AudioManager: Audio sources initialized! Music: {musicSource != null}, SFX: {sfxSource != null}, Ambient: {ambientSource != null}");
        Debug.Log($"SFX Source Loop: {sfxSource.loop} (should be false!)");
    }

    private void Start()
    {
        // 배경음악 시작 (게임 씬에서만)
        // StartScreen이나 다른 씬에서는 해당 씬에서 재생
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "StartScreen")
        {
            PlayBackgroundMusic();
            Debug.Log("Background music auto-started");
        }
    }

    // ==================== 음악 ====================

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            musicSource.loop = true; // 배경음악은 루프
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    public void PlayVictoryMusic()
    {
        if (victoryMusic != null)
        {
            musicSource.loop = false; // 승리 음악은 한 번만!
            musicSource.clip = victoryMusic;
            musicSource.Play();
            Debug.Log("Victory music playing (no loop)");
        }
    }

    public void PlayDefeatMusic()
    {
        if (defeatMusic != null)
        {
            musicSource.loop = false; // 패배 음악은 한 번만!
            musicSource.clip = defeatMusic;
            musicSource.Play();
            Debug.Log("Defeat music playing (no loop)");
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
        musicSource.clip = null; // Clip도 초기화
        Debug.Log("Music stopped");
    }

    // ==================== 효과음 ====================

    public void PlaySound(AudioClip clip, float volumeScale = 1f)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volumeScale);
        }
    }

    public void PlaySoundAtPosition(AudioClip clip, Vector3 position, float volumeScale = 1f)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position, sfxVolume * volumeScale);
        }
    }

    // ==================== 플레이어 ====================

    public void PlayFootstep()
    {
        if (footstepSounds != null && footstepSounds.Length > 0)
        {
            AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
            PlaySound(clip, 0.5f * playerSoundVolume);
        }
    }

    public void PlayPlayerHurt()
    {
        PlaySound(playerHurtSound, 1.0f * playerSoundVolume);
    }

    public void PlayPlayerDeath()
    {
        PlaySound(playerDeathSound, 1.0f * playerSoundVolume);
    }

    // ==================== 무기 ====================

    public void PlayRifleShoot()
    {
        PlaySound(rifleShootSound, 0.8f * weaponSoundVolume);
    }

    public void PlayMeleeSwing()
    {
        PlaySound(meleeSwingSound, 0.6f * weaponSoundVolume);
    }

    public void PlayMeleeHit()
    {
        PlaySound(meleeHitSound, 0.7f * weaponSoundVolume);
    }

    public void PlayReload()
    {
        PlaySound(reloadSound, 0.8f * weaponSoundVolume);
    }

    // ==================== 좀비 ====================

    public void PlayZombieIdle()
    {
        if (zombieIdleSounds != null && zombieIdleSounds.Length > 0)
        {
            AudioClip clip = zombieIdleSounds[Random.Range(0, zombieIdleSounds.Length)];
            PlaySound(clip, 0.4f * zombieSoundVolume);
        }
    }

    public void PlayZombieAttack()
    {
        if (zombieAttackSounds != null && zombieAttackSounds.Length > 0)
        {
            AudioClip clip = zombieAttackSounds[Random.Range(0, zombieAttackSounds.Length)];
            PlaySound(clip, 0.6f * zombieSoundVolume);
        }
    }

    public void PlayZombieHit()
    {
        PlaySound(zombieHitSound, 0.5f * zombieSoundVolume);
    }

    public void PlayZombieDeath()
    {
        PlaySound(zombieDeathSound, 0.6f * zombieSoundVolume);
    }

    // ==================== 아이템 ====================

    public void PlayItemPickup()
    {
        PlaySound(itemPickupSound, 0.5f * itemSoundVolume);
    }

    public void PlayItemUse()
    {
        PlaySound(itemUseSound, 0.7f * itemSoundVolume);
    }

    public void PlayHeal()
    {
        PlaySound(healSound, 0.8f * itemSoundVolume);
    }

    public void PlayAmmoPickup()
    {
        PlaySound(ammoPickupSound, 0.6f * itemSoundVolume);
    }

    // ==================== UI ====================

    public void PlayButtonClick()
    {
        PlaySound(buttonClickSound, 0.6f * uiSoundVolume);
    }

    public void PlayButtonHover()
    {
        PlaySound(buttonHoverSound, 0.3f * uiSoundVolume);
    }

    // ==================== 탈출 ====================

    public void PlayRepair()
    {
        if (repairSound != null && !sfxSource.isPlaying)
        {
            sfxSource.clip = repairSound;
            sfxSource.loop = true;
            sfxSource.Play();
        }
    }

    public void StopRepair()
    {
        if (sfxSource.clip == repairSound)
        {
            sfxSource.loop = false;
            sfxSource.Stop();
        }
    }

    public void PlayRepairComplete()
    {
        StopRepair();
        PlaySound(repairCompleteSound);
    }

    public void PlayEscape()
    {
        PlaySound(escapeSound);
    }

    // ==================== 볼륨 조절 ====================

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }

    public void SetAmbientVolume(float volume)
    {
        ambientVolume = Mathf.Clamp01(volume);
        ambientSource.volume = ambientVolume;
    }
}