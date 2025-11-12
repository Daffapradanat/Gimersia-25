using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossSceneManager : MonoBehaviour
{
    public static BossSceneManager Instance { get; private set; }

    [Header("Boss Reference")]
    [SerializeField] GameObject bossObject;
    [SerializeField] BossController bossController;
    [SerializeField] BossAttackManager bossAttackManager;
    [SerializeField] BossAnimationController bossAnimationController;
    
    [Header("Timing")]
    [SerializeField] float delayBeforeBossAppears = 20f;
    [SerializeField] float entranceDialogDuration = 3f;
    [SerializeField] float deathDialogDuration = 3f;
    
    [Header("Dialog UI")]
    [SerializeField] GameObject dialogPanel;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] Image bossPortrait;
    [SerializeField] float textSpeed = 0.05f;
    
    [Header("Entrance Dialog")]
    [SerializeField] string entranceDialog = "Finally... you dare challenge me!";
    [SerializeField] Sprite entrancePortrait;
    
    [Header("Death Dialog")]
    [SerializeField] string deathDialog = "Impossible... I cannot be defeated!";
    [SerializeField] Sprite deathPortrait;
    
    [Header("Effects")]
    [SerializeField] GameObject bossEntranceVFX;
    [SerializeField] Vector3 bossSpawnPosition;
    [SerializeField] float entranceShakeDuration = 0.5f;
    [SerializeField] float entranceShakeStrength = 0.8f;
    
    bool bossHasAppeared;
    bool bossIsDead;
    bool dialogIsPlaying;
    bool bossSequenceStarted;
    Coroutine currentDialogCoroutine;
    float gameTimer;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        InitializeBoss();
    }

    void Update()
    {
        if (bossSequenceStarted) return;
        
        gameTimer += Time.deltaTime;
        
        if (gameTimer >= delayBeforeBossAppears)
        {
            Debug.Log($"[BossSceneManager] Timer reached! Starting boss appearance. Time: {gameTimer}");
            bossSequenceStarted = true;
            StartCoroutine(BossAppearanceSequence());
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        
        if (bossAnimationController != null)
        {
            bossAnimationController.OnDeathAnimationComplete -= HandleBossDeathComplete;
        }
    }

    void InitializeBoss()
    {
        if (bossObject != null)
        {
            bossObject.SetActive(false);
        }
        
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
        
        if (bossAnimationController != null)
        {
            bossAnimationController.OnDeathAnimationComplete += HandleBossDeathComplete;
        }
        
        if (bossController == null && bossObject != null)
        {
            bossController = bossObject.GetComponent<BossController>();
        }
        
        if (bossAttackManager == null && bossObject != null)
        {
            bossAttackManager = bossObject.GetComponent<BossAttackManager>();
        }
        
        if (bossAnimationController == null && bossObject != null)
        {
            bossAnimationController = bossObject.GetComponent<BossAnimationController>();
        }
    }

    IEnumerator BossAppearanceSequence()
    {
        if (bossHasAppeared) yield break;
        
        Debug.Log("[BossSceneManager] Boss appearance sequence started!");
        
        bossHasAppeared = true;
        
        FreezeAllEnemies(true);
        FreezePlayer(true);
        
        if (bossObject != null)
        {
            Debug.Log("[BossSceneManager] Activating boss object...");
            bossObject.transform.position = bossSpawnPosition;
            bossObject.SetActive(true);
        }
        else
        {
            Debug.LogError("[BossSceneManager] Boss object is NULL! Please assign it in inspector!");
        }
        
        if (bossEntranceVFX != null)
        {
            Instantiate(bossEntranceVFX, bossSpawnPosition, Quaternion.identity);
        }
        
        if (Camera.main != null)
        {
            StartCoroutine(ShakeCamera(entranceShakeDuration, entranceShakeStrength));
        }
        
        DisableBossComponents();
        
        yield return new WaitForSeconds(0.5f);
        
        yield return ShowDialog(entranceDialog, entrancePortrait, entranceDialogDuration);
        
        Debug.Log("[BossSceneManager] Enabling boss components...");
        EnableBossComponents();
        
        FreezeAllEnemies(false);
        FreezePlayer(false);
        
        Debug.Log("[BossSceneManager] Boss appearance complete!");
    }

    public void TriggerBossDeathSequence()
    {
        if (bossIsDead) return;
        
        bossIsDead = true;
        StartCoroutine(BossDeathSequence());
    }

    IEnumerator BossDeathSequence()
    {
        FreezeAllEnemies(true);
        FreezePlayer(true);
        
        DisableBossComponents();
        
        yield return new WaitForSeconds(0.3f);
        
        yield return ShowDialog(deathDialog, deathPortrait, deathDialogDuration);
        
        if (bossAnimationController != null)
        {
            bossAnimationController.ExecuteDeathWithoutDialog();
        }
    }

    void HandleBossDeathComplete()
    {
        FreezeAllEnemies(false);
        FreezePlayer(false);
    }

    IEnumerator ShowDialog(string text, Sprite portrait, float duration)
    {
        if (dialogPanel == null)
        {
            Debug.LogWarning("[BossSceneManager] Dialog Panel is NULL! Skipping dialog...");
            yield break;
        }
        
        if (dialogText == null)
        {
            Debug.LogWarning("[BossSceneManager] Dialog Text is NULL! Skipping dialog...");
            yield break;
        }
        
        Debug.Log($"[BossSceneManager] Showing dialog: {text}");
        
        dialogIsPlaying = true;
        dialogPanel.SetActive(true);
        
        if (bossPortrait != null && portrait != null)
        {
            bossPortrait.sprite = portrait;
            bossPortrait.enabled = true;
        }
        
        dialogText.text = "";
        
        foreach (char c in text)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        
        yield return new WaitForSeconds(duration);
        
        dialogPanel.SetActive(false);
        dialogIsPlaying = false;
        
        Debug.Log("[BossSceneManager] Dialog finished");
    }

    void DisableBossComponents()
    {
        if (bossController != null)
        {
            bossController.enabled = false;
        }
        
        if (bossAttackManager != null)
        {
            bossAttackManager.enabled = false;
        }
    }

    void EnableBossComponents()
    {
        if (bossController != null)
        {
            bossController.enabled = true;
        }
        
        if (bossAttackManager != null)
        {
            bossAttackManager.enabled = true;
        }
    }

    void FreezeAllEnemies(bool freeze)
    {
        MinionController[] minions = FindObjectsByType<MinionController>(FindObjectsSortMode.None);
        foreach (MinionController minion in minions)
        {
            minion.enabled = !freeze;
        }
        
        if (MinionSpawner.Instance != null)
        {
            if (freeze)
            {
                MinionSpawner.Instance.StopAutoWave();
            }
        }
    }

    void FreezePlayer(bool freeze)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                if (freeze)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.simulated = false;
                }
                else
                {
                    rb.simulated = true;
                }
            }
            
            MonoBehaviour[] playerScripts = player.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in playerScripts)
            {
                if (script.GetType().Name != "Transform")
                {
                    script.enabled = !freeze;
                }
            }
        }
    }

    IEnumerator ShakeCamera(float duration, float strength)
    {
        if (Camera.main == null) yield break;
        
        Vector3 originalPos = Camera.main.transform.position;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;
            
            Camera.main.transform.position = new Vector3(
                originalPos.x + x,
                originalPos.y + y,
                originalPos.z
            );
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        Camera.main.transform.position = originalPos;
    }

    public void SkipDialog()
    {
        if (dialogIsPlaying && currentDialogCoroutine != null)
        {
            StopCoroutine(currentDialogCoroutine);
            if (dialogPanel != null)
            {
                dialogPanel.SetActive(false);
            }
            dialogIsPlaying = false;
        }
    }

    public bool IsBossActive()
    {
        return bossHasAppeared && !bossIsDead;
    }

    public bool IsDialogPlaying()
    {
        return dialogIsPlaying;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(bossSpawnPosition, 1f);
        Gizmos.DrawLine(bossSpawnPosition + Vector3.up, bossSpawnPosition + Vector3.down);
        Gizmos.DrawLine(bossSpawnPosition + Vector3.left, bossSpawnPosition + Vector3.right);
    }
}