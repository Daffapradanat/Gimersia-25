using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    public static CharacterMove Instance;
    
    public float moveSpeed = 5f;
    public Vector2 movement;
    public Rigidbody2D rb;

    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public float dashAccelerationTime = 0.1f;
    
    bool isDashing = false;
    float dashTime = 0.3f;
    float dashCooldownTimer = 1f;
    float dashSpeedMultiplier = 1f;

    public bool isCanMove = true;

    void Start()
    {
        Instance = this;
    }

    void Update()
    {
        if (!GameManager.Instance.isPlay) return;
        if (!isCanMove) return;

        movement.x = Input.GetAxisRaw("Horizontal");
        movement = movement.normalized;

        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.L)) && !isDashing && dashCooldownTimer <= 0)
        {
            isDashing = true;
            dashTime = dashDuration;
            dashCooldownTimer = dashCooldown + dashDuration;
            dashSpeedMultiplier = 1f;
        }

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.Instance.isPlay) return;
        
        if (isDashing)
        {
            if (dashSpeedMultiplier < 4f)
            {
                dashSpeedMultiplier += (3f / dashAccelerationTime) * Time.fixedDeltaTime;
                dashSpeedMultiplier = Mathf.Min(dashSpeedMultiplier, 4f);
            }

            rb.linearVelocity = movement * moveSpeed * dashSpeedMultiplier;

            dashTime -= Time.fixedDeltaTime;
            if (dashTime <= 0)
            {
                isDashing = false;
                dashSpeedMultiplier = 1f;
            }
        }
        else
        {
            rb.linearVelocity = movement * moveSpeed;
        }
    }
}