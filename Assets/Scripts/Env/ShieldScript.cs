using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
    public int maxShield = 5, shield;
    public float durationCooldown = 60f;
    bool isCooldown = false;

    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider2D;
    void Start()
    {
        shield = maxShield;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (shield <= 0 && !isCooldown)
        {
            StartCoroutine(Coolodown());
        }
    }

    public IEnumerator GetHitEffect()
    {

        spriteRenderer.color = new Color32(248, 92, 92, 255);
        yield return new WaitForSecondsRealtime(0.2f);
        spriteRenderer.color = new Color32(255, 255, 255, 255);
    }

    IEnumerator Coolodown()
    {
        isCooldown = true;
        boxCollider2D.enabled = false;
        spriteRenderer.color = new Color32(255, 255, 255, 0);

        yield return new WaitForSeconds(durationCooldown);

        shield = maxShield;
        boxCollider2D.enabled = true;
        spriteRenderer.color = new Color32(255, 255, 255, 255);
        isCooldown = false;
    }
}
