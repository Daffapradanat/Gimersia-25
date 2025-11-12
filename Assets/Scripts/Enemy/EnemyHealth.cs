using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public event Action OnDeath;
    public event Action<float> OnDamageTaken;
    
    [Header("Health")]
    [SerializeField] float maxHealth = 100f;
    
    [Header("Enemy Type")]
    [SerializeField] bool isBoss = false;
    [SerializeField] int scoreReward = 10;
    
    [Header("Visual Feedback")]
    [SerializeField] bool flashOnHit = true;
    [SerializeField] Color hitColor = Color.red;
    [SerializeField] float flashDuration = 0.1f;
    
    [Header("Death Behavior")]
    [SerializeField] bool autoDestroy = true;
    [SerializeField] float destroyDelay = 0f;
    
    float currentHealth;
    bool isDead;
    SpriteRenderer spriteRenderer;
    Color originalColor;
    Coroutine flashCoroutine;
    
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsAlive => !isDead && currentHealth > 0;
    public float HealthPercentage => maxHealth > 0 ? currentHealth / maxHealth : 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) 
            originalColor = spriteRenderer.color;
    }

    void Start()
    {
        currentHealth = maxHealth;
        
        if (isBoss && GameManager.Instance != null)
        {
            GameManager.Instance.totalHealthBoss += (int)maxHealth;
            GameManager.Instance.isHaveBoss = true;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead || amount <= 0) return;
        
        float previousHealth = currentHealth;
        currentHealth -= amount;
        currentHealth = Mathf.Max(0f, currentHealth);
        
        float actualDamage = previousHealth - currentHealth;
        
        OnDamageTaken?.Invoke(actualDamage);
        
        if (isBoss && GameManager.Instance != null)
        {
            GameManager.Instance.totalHealthBoss -= (int)actualDamage;
            GameManager.Instance.totalHealthBoss = Mathf.Max(0, GameManager.Instance.totalHealthBoss);
        }
        
        if (flashOnHit && spriteRenderer != null)
        {
            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine);
            
            flashCoroutine = StartCoroutine(FlashEffect());
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead || amount <= 0) return;
        
        float previousHealth = currentHealth;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        float actualHeal = currentHealth - previousHealth;
        
        if (isBoss && GameManager.Instance != null)
        {
            GameManager.Instance.totalHealthBoss += (int)actualHeal;
        }
    }

    public void SetMaxHealth(float value)
    {
        if (value <= 0) return;
        
        float healthPercentage = HealthPercentage;
        maxHealth = value;
        currentHealth = maxHealth * healthPercentage;
    }

    public void SetCurrentHealth(float value)
    {
        currentHealth = Mathf.Clamp(value, 0f, maxHealth);
        
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    IEnumerator FlashEffect()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        
        if (!isDead && spriteRenderer != null)
            spriteRenderer.color = originalColor;
        
        flashCoroutine = null;
    }

    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }
        
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreReward);
            
            if (transform.position != null)
            {
                GameManager.Instance.SpawnVfxExplode(transform.position);
            }
        }
        
        OnDeath?.Invoke();
        
        if (autoDestroy)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    void OnDestroy()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
    }
}