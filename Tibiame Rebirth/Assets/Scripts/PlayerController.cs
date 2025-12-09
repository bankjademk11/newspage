using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Components")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 movement;
    private UIManager uiManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        // หา UIManager
        uiManager = FindObjectOfType<UIManager>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        if (animator == null)
        {
            Debug.LogWarning("ไม่มี Animator ติดอยู่กับ Player นะคะนายท่าน 💕");
        }
        
        if (uiManager == null)
        {
            Debug.LogWarning("ไม่พบ UIManager กรุณาเพิ่ม UIManager ในฉาก");
        }
    }

    void Update()
    {
        // ตรวจสอบว่ามี UI เปิดอยู่หรือไม่ก่อนอนุญาตการเคลื่อนที่
        if (uiManager != null && uiManager.IsAnyUIOpen())
        {
            // ถ้ามี UI เปิดอยู่ ให้หยุดการเคลื่อนที่
            movement = Vector2.zero;
            
            // หยุด animation
            if (animator != null)
            {
                animator.SetBool("isWalk", false);
                animator.speed = 1f;
            }
            return;
        }
        
        // รับค่าการเดิน
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        // เล่น Animation เดิน / หยุด
        HandleAnimation();

        // จัดการทิศทางการหันหน้าตาม A/D
        HandleDirection();
    }

    void FixedUpdate()
    {
        // เคลื่อนที่เฉพาะเมื่อไม่มี UI เปิดอยู่
        if (uiManager == null || !uiManager.IsAnyUIOpen())
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
    }

    void HandleDirection()
    {
        // หันหน้าตามทิศทางการกดปุ่ม A/D เท่านั้น
        if (movement.x > 0) // กด D หรือลูกศรขวา
        {
            spriteRenderer.flipX = false; // หันหน้าขวา
        }
        else if (movement.x < 0) // กด A หรือลูกศรซ้าย
        {
            spriteRenderer.flipX = true; // หันหน้าซ้าย
        }
        // ถ้ากด W/S ไม่ต้องเปลี่ยนทิศทางการหันหน้า
    }

    void HandleAnimation()
    {
        if (animator == null) return;

        bool isWalking = movement.sqrMagnitude > 0.01f;
        animator.SetBool("isWalk", isWalking);

        // ปรับความเร็วแอนิเมชันเวลาเดิน
        if (isWalking)
            animator.speed = 0.5f;   // ทำให้เดินลื่น
        else
            animator.speed = 1f;     // idle ปกติ
    }
}
