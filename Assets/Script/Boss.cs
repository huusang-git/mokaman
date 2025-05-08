using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class Boss : MonoBehaviour
{
    public float jumpCooldown = 5f;
    private float lastJumpTime = -5f;
    public float health = 300f;
    public float phase2Threshold = 150f;
    public string weakness = "supperdark"; // Điểm yếu ban đầu
    public float speed = 2f;
    public float attackCooldown = 2f;
    public float damage = 20f;
    public GameObject floatingDamagePrefab;
    public Transform player;
    public bool isPhase2 = false;
    private float lastAttackTime;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public GameObject groundSmashEffect;
    public GameObject earthquakeWavePrefab;
    public float meleeRange = 1.5f;
    public float jumpAttackRange = 5f;
    public float earthquakeRange = 8f;
    private Rigidbody2D rb;
    private float lastEarthquakeTime = 0f;
    public float earthquakeCooldown = 5f;
    private bool isSleeping = true;
    private CinemachineImpulseSource impulseSource;
    private bool isPerformingEarthquake = false;
    private bool isDead = false;
    public DialogueManager dialogueManager;
    [SerializeField] private GameObject portalPrefab; // Prefab cánh cổng

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        impulseSource = GetComponent<CinemachineImpulseSource>();
        if (player == null)
        {
            Debug.LogError("Không tìm thấy Player!");
        }
        if (impulseSource == null)
        {
            Debug.LogError("Không tìm thấy CinemachineImpulseSource trên Boss!");
        }
        if (portalPrefab == null)
        {
            Debug.LogError("Thiếu Portal Prefab trên Boss!");
        }
    }

    void Update()
    {
        if (player == null || isSleeping || isDead) return;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (!isPhase2 && health <= phase2Threshold)
        {
            EnterPhase2();
        }
        ChasePlayer();
    }

    public void WakeUp()
    {
        isSleeping = false;
        animator.SetTrigger("WakeUp");
        Debug.Log("👁️ Boss thức dậy và chuẩn bị chiến đấu!");
    }

    void EnterPhase2()
    {
        isPhase2 = true;
        speed *= 1.25f;
        damage *= 1.5f;
        attackCooldown *= 0.75f;
        weakness = "orange"; // Đổi điểm yếu
        animator.SetTrigger("Phase2");
        Debug.Log("🔥 Boss vào Phase 2!");
    }

    void ChasePlayer()
    {
        if (player == null) return;
        float direction = player.position.x - transform.position.x;
        float distance = Mathf.Abs(direction);

        if (isPerformingEarthquake)
        {
            if ((direction > 0 && transform.localScale.x < 0) || (direction < 0 && transform.localScale.x > 0))
                Flip();
            return;
        }

        if (distance <= meleeRange)
        {
            animator.SetBool("isRunning", false);
            AttackPlayer();
        }
        else if (isPhase2 && distance <= earthquakeRange)
        {
            animator.SetBool("isRunning", false);
            EarthquakeAttack();
        }
        else
        {
            Vector2 targetVelocity = (player.position - transform.position).normalized * speed;
            rb.linearVelocity = new Vector2(targetVelocity.x, rb.linearVelocity.y);
            animator.SetBool("isRunning", true);
        }

        if (distance > meleeRange && distance <= jumpAttackRange)
            JumpAttack();

        if ((direction > 0 && transform.localScale.x < 0) || (direction < 0 && transform.localScale.x > 0))
            Flip();
    }

    void Flip()
    {
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }

    void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    void JumpAttack()
    {
        if (Time.time - lastJumpTime >= jumpCooldown)
        {
            animator.SetTrigger("JumpAttack");
            lastJumpTime = Time.time;
            StartCoroutine(JumpAttackRoutine());
        }
    }

    private IEnumerator JumpAttackRoutine()
    {
        float jumpForce = 10f;
        float fallMultiplier = 2.5f;
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);
        animator.SetTrigger("Fall");
        rb.linearVelocity = new Vector2(0, jumpForce);
        yield return new WaitForSeconds(0.5f);

        Collider2D playerCollider = player.GetComponent<Collider2D>();
        Collider2D bossCollider = GetComponent<Collider2D>();

        if (playerCollider != null && bossCollider != null)
        {
            Bounds playerBounds = playerCollider.bounds;
            Bounds bossBounds = bossCollider.bounds;

            float offset = bossBounds.size.x + playerBounds.size.x + 0.1f;

            if (transform.position.x < player.position.x)
            {
                targetPosition.x = player.position.x - offset;
            }
            else
            {
                targetPosition.x = player.position.x + offset;
            }

            targetPosition.y = bossBounds.min.y;
        }

        transform.position = new Vector3(targetPosition.x, transform.position.y, transform.position.z);

        rb.linearVelocity = new Vector2(0, -jumpForce * fallMultiplier);
        yield return new WaitForSeconds(0.3f);

        if (groundSmashEffect != null)
        {
            Vector3 effectPosition = new Vector3(transform.position.x, GetBossBottomY(), transform.position.z);
            Instantiate(groundSmashEffect, effectPosition, Quaternion.identity);
        }
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
            Debug.Log("Kích hoạt rung màn hình với Cinemachine Impulse!");
        }
    }

    private float GetBossBottomY()
    {
        Collider2D bossCollider = GetComponent<Collider2D>();
        return bossCollider != null ? bossCollider.bounds.min.y : transform.position.y;
    }

    void EarthquakeAttack()
    {
        if (Time.time - lastEarthquakeTime >= earthquakeCooldown)
        {
            animator.SetTrigger("EarthquakeAttack");
            lastEarthquakeTime = Time.time;
            isPerformingEarthquake = true;
            StartCoroutine(EarthquakeRoutine());
        }
    }

    private IEnumerator EarthquakeRoutine()
    {
        animator.SetBool("isRunning", false);
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(1f);
        float spacing = 1.5f;
        float delayBetweenWaves = 0.2f;
        Vector3 basePos = new Vector3(transform.position.x, GetBossBottomY(), transform.position.z);

        for (int i = 1; i <= 12; i++)
        {
            Vector3 leftPos = basePos + Vector3.left * i * spacing;
            Vector3 rightPos = basePos + Vector3.right * i * spacing;
            GameObject leftWave = Instantiate(earthquakeWavePrefab, leftPos, Quaternion.identity);
            GameObject rightWave = Instantiate(earthquakeWavePrefab, rightPos, Quaternion.identity);
            leftWave.GetComponent<EarthquakeWave>()?.SetDirection(Vector2.zero);
            rightWave.GetComponent<EarthquakeWave>()?.SetDirection(Vector2.zero);
            yield return new WaitForSeconds(delayBetweenWaves);
        }

        isPerformingEarthquake = false;
    }

    public void DealDamage()
    {
        if (CheckPlayerInRange())
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"💥 Boss gây {damage} sát thương cho Player!");
            }
        }
    }

    private bool CheckPlayerInRange()
    {
        float horizontalDistance = Mathf.Abs(player.position.x - transform.position.x);
        float verticalDistance = Mathf.Abs(player.position.y - transform.position.y);
        return horizontalDistance <= meleeRange && verticalDistance <= 2f;
    }

    public void TakeDamage(float damage, string bulletType)
    {
        if (isDead) return;
        float finalDamage = (bulletType == weakness) ? damage * 2f : damage * 0.5f;
        health -= finalDamage;
        StartCoroutine(HitEffect());

        if (floatingDamagePrefab != null)
        {
            GameObject damageText = Instantiate(floatingDamagePrefab, transform.position + Vector3.up, Quaternion.identity);
            FloatingDamage floatingDamage = damageText.GetComponent<FloatingDamage>();
            if (floatingDamage != null)
            {
                floatingDamage.SetDamageText(finalDamage);
            }
        }

        if (health <= 0 && !isDead)
        {
            Die();
        }
    }

    private IEnumerator HitEffect()
    {
        Vector3 originalPos = transform.position;
        float shakeAmount = 0.2f;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = originalPos + (Vector3)Random.insideUnitCircle * shakeAmount;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.05f);
            elapsed += 0.1f;
        }
        transform.position = originalPos;
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        rb.linearVelocity = Vector2.zero;
        Debug.Log("💀 Boss bị tiêu diệt!");
        if (dialogueManager != null)
        {
            string[] deathDialogues = {
                "Player: Cuối cùng thì rốt cuộc chuyện gì đã xảy ra vậy!",
                "Boss: Tất cả ... là tại ngươi... Mo..k...",
                "Player: Vậy là kết thúc rồi.",
                "Player: Mok? lần này chẳng thu thập được gì nhiều."
            };
            dialogueManager.StartDialogue(deathDialogues, null);
        }
        if (portalPrefab != null)
        {
            Vector3 portalPos = transform.position + new Vector3(2f, 0, 0); // Spawn cổng bên phải boss
            Instantiate(portalPrefab, portalPos, Quaternion.identity);
            Debug.Log("🚪 Cánh cổng xuất hiện!");
        }
        BossAreaController areaController = FindObjectOfType<BossAreaController>();
        if (areaController != null)
        {
            areaController.OnBossDefeated();
        }
        Destroy(gameObject, 1f);
    }
}