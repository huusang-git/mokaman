using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Cho List
using System.Linq;
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    public float fallMultiplier = 3f;
    public float lowJumpMultiplier = 2f;

    public Transform groundCheck;
    public Transform firePoint;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private bool facingRight = true;

    // 🏃‍♂️ Dash Variables
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private bool isDashing = false;
    private bool canDash = true;
    private float dashDirection;

    private bool canControl = true;
    // 🐺 Coyote Time Variables
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    // 🚫 Danh sách để lưu va chạm bị tắt khi dash
    private List<Collider2D> ignoredColliders = new List<Collider2D>();
    private Collider2D playerCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>(); // Lấy Collider của player
        if (playerCollider == null)
        {
            Debug.LogError("PlayerController: Không tìm thấy Collider2D trên Player!");
        }
    }

    void Update()
    {
        if (!canControl || isDashing) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        animator.SetBool("isGrounded", isGrounded);

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));

        if (horizontalInput > 0 && !facingRight)
            Flip();
        else if (horizontalInput < 0 && facingRight)
            Flip();

        if (Input.GetKeyDown(KeyCode.Space) && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetTrigger("Jump");
            coyoteTimeCounter = 0f;
        }

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            animator.SetBool("isFalling", true);
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        else
        {
            animator.SetBool("isFalling", false);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    public void SetCanControl(bool canMove)
    {
        canControl = canMove;
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetFloat("Speed", 0);
        }
    }

    public bool IsFacingRight()
    {
        return facingRight;
    }

    void Flip()
    {
        if (isDashing) return;

        facingRight = !facingRight;
        transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);

        if (firePoint != null)
        {
            firePoint.localPosition = new Vector3(-firePoint.localPosition.x, firePoint.localPosition.y, firePoint.localPosition.z);
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        animator.SetTrigger("Dash");

        dashDirection = facingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);
        gameObject.layer = LayerMask.NameToLayer("Invulnerable");

        // Tắt va chạm với boss và quái
        Collider2D[] enemies = GameObject.FindGameObjectsWithTag("Enemy")
            .Select(go => go.GetComponent<Collider2D>())
            .Where(col => col != null)
            .ToArray();
        foreach (Collider2D enemyCollider in enemies)
        {
            if (playerCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, enemyCollider, true);
                ignoredColliders.Add(enemyCollider);
            }
        }

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        gameObject.layer = LayerMask.NameToLayer("Default");
        rb.linearVelocity = Vector2.zero;

        // Bật lại va chạm
        foreach (Collider2D enemyCollider in ignoredColliders)
        {
            if (enemyCollider != null && playerCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, enemyCollider, false);
            }
        }
        ignoredColliders.Clear();

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}