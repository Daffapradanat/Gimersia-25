using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Slider healthSlider;
    [SerializeField] CanvasGroup canvasGroup;
    
    [Header("Visibility Settings")]
    [SerializeField] float hideDelay = 2f;
    [SerializeField] float fadeDuration = 0.3f;
    
    EnemyHealth enemyHealth;
    Coroutine hideCoroutine;
    
    void Awake()
    {
        enemyHealth = GetComponentInParent<EnemyHealth>();
        
        if (enemyHealth == null)
        {
            Debug.LogError("EnemyHealthBar: EnemyHealth tidak ditemukan di parent!");
            enabled = false;
            return;
        }
        
        if (healthSlider == null)
        {
            healthSlider = GetComponentInChildren<Slider>();
        }
        
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
    }
    
    void Start()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = 1f;
            healthSlider.value = 1f;
        }
        
        canvasGroup.alpha = 0f;
        
        enemyHealth.OnDamageTaken += HandleDamageTaken;
        enemyHealth.OnDeath += HandleDeath;
    }
    
    void HandleDamageTaken(float damage)
    {
        UpdateHealthBar();
        ShowHealthBar();
    }
    
    void UpdateHealthBar()
    {
        if (healthSlider != null && enemyHealth != null)
        {
            healthSlider.value = enemyHealth.HealthPercentage;
        }
    }
    
    void ShowHealthBar()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        
        StartCoroutine(FadeIn());
        
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }
    
    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, elapsed / fadeDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
    }
    
    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);
        
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
        hideCoroutine = null;
    }
    
    void HandleDeath()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        StopAllCoroutines();
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }
    
    void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnDamageTaken -= HandleDamageTaken;
            enemyHealth.OnDeath -= HandleDeath;
        }
    }
    
    void LateUpdate()
    {
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                           Camera.main.transform.rotation * Vector3.up);
        }
    }
}