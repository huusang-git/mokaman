using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float health = 100f; // 🩸 Máu của kẻ địch
    public string weakness; // 🔥 Điểm yếu của kẻ địch (supperdark, mixed, orangeespresso)
    public float speed = 2f;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float damage = 10f;
    public float patrolDistance = 5f;
    public float waitTimeBeforeReturning = 1.5f;
    private string lastBulletType; // Lưu lại loại đạn gần nhất
    private bool hasReceivedBuff = false;

    public GameObject floatingDamagePrefab; // Prefab số sát thương

    private Transform player;
    private float lastAttackTime;
    private Vector2 patrolOrigin;
    private Vector2 targetPosition;
    private bool chasingPlayer = false;
    private bool returningToPatrol = false;
    private bool isWaiting = false;
    private Coroutine waitCoroutine = null;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private AudioClip hitSFX;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (player == null)
        {
            Debug.LogError("Không tìm thấy Player!");
        }

        patrolOrigin = transform.position;
        targetPosition = patrolOrigin + new Vector2(patrolDistance, 0);
    }

    void Update()
    {
        if (player == null || isWaiting) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            chasingPlayer = true;
            returningToPatrol = false;
            StopWaiting();
        }
        else if (chasingPlayer)
        {
            chasingPlayer = false;
            if (waitCoroutine == null)
            {
                waitCoroutine = StartCoroutine(WaitBeforeReturning());
            }
        }

        if (chasingPlayer)
        {
            ChasePlayer();
        }
        else if (returningToPatrol)
        {
            ReturnToPatrol();
        }
        else
        {
            Patrol();
        }
    }

    void ChasePlayer()
    {
        if (Vector2.Distance(transform.position, player.position) > attackRange)
        {
            Vector2 targetPosition = new Vector2(player.position.x, transform.position.y); // Giữ nguyên Y
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            animator.SetBool("isRunning", true);
            Flip(player.position.x - transform.position.x);
        }
        else
        {
            animator.SetBool("isRunning", false);
            AttackPlayer();
        }
    }


    void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }
    void Patrol()
    {
        Vector2 patrolTarget = new Vector2(targetPosition.x, transform.position.y); // Giữ nguyên Y
        if (Vector2.Distance(transform.position, patrolTarget) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, patrolTarget, speed * Time.deltaTime);
            animator.SetBool("isRunning", true);
            Flip(targetPosition.x - transform.position.x); // Chỉ flip khi thực sự di chuyển
        }
        else
        {
            animator.SetBool("isRunning", false); // Dừng chạy khi đến nơi
            targetPosition.x = Mathf.Approximately(targetPosition.x, patrolOrigin.x + patrolDistance) ?
                               patrolOrigin.x - patrolDistance :
                               patrolOrigin.x + patrolDistance;
        }
    }


    void ReturnToPatrol()
    {
        Vector2 returnTarget = new Vector2(patrolOrigin.x, transform.position.y);
        if (Vector2.Distance(transform.position, returnTarget) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, returnTarget, speed * Time.deltaTime);
            animator.SetBool("isRunning", true);
            Flip(patrolOrigin.x - transform.position.x); // Chỉ flip khi thực sự di chuyển
        }
        else
        {
            returningToPatrol = false;
            animator.SetBool("isRunning", false); // Dừng chạy khi về đến nơi
        }
    }

    private IEnumerator HitEffect()
    {
        Vector3 originalPos = transform.position;
        float shakeAmount = 0.1f;
        float duration = 0.2f; // Thời gian rung + nhấp nháy
        float elapsed = 0f;

        while (elapsed < duration)
        {
            AudioManager.Instance?.PlaySFX(hitSFX);
            transform.position = originalPos + (Vector3)Random.insideUnitCircle * shakeAmount;
            spriteRenderer.color = Color.red; // Nhấp nháy màu đỏ khi trúng đòn
            yield return new WaitForSeconds(0.05f);

            transform.position = originalPos;
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.05f);

            elapsed += 0.1f;
        }

        transform.position = originalPos; // Reset vị trí
        spriteRenderer.color = Color.white; // Reset màu
    }

    IEnumerator WaitBeforeReturning()
    {
        isWaiting = true;
        animator.SetBool("isRunning", false);

        float elapsedTime = 0f;

        while (elapsedTime < waitTimeBeforeReturning)
        {
            if (Vector2.Distance(transform.position, player.position) <= detectionRange)
            {
                chasingPlayer = true;
                isWaiting = false;
                waitCoroutine = null;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        returningToPatrol = true;
        isWaiting = false;
        waitCoroutine = null;
    }

    void StopWaiting()
    {
        if (waitCoroutine != null)
        {
            StopCoroutine(waitCoroutine);
            waitCoroutine = null;
        }
        isWaiting = false;
    }
    public void TakeDamage(float damage, string bulletType, bool isMelee = false)
    {
        float finalDamage = damage;
        lastBulletType = bulletType;
        hasReceivedBuff = false;

        if (!isMelee)
        {
            if (bulletType == weakness)
            {
                finalDamage *= 2f; // Nếu bắn đúng điểm yếu, nhận x2 sát thương
            }

            // Áp dụng hiệu ứng đặc biệt của kẻ địch
            if (weakness == "mixed" && bulletType == "supperdark")
            {
                speed *= 2f;
                Debug.Log("⚡ Enemy Mixed nhận supperdark! Tăng tốc độ di chuyển.");
            }
            else if (weakness == "orange" && bulletType == "mixed")
            {
                health += finalDamage * 2f; // Hồi máu gấp đôi lượng sát thương nhận vào
                Debug.Log("🍊 Enemy Orange hồi máu!");
            }
        }

        // Trừ máu kẻ địch
        health -= finalDamage;
        Debug.Log($"🩸 Enemy nhận {finalDamage} sát thương từ {bulletType}. Máu còn: {health}");

        if (floatingDamagePrefab != null)
        {
            GameObject damageText = Instantiate(floatingDamagePrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
            FloatingDamage floatingDamage = damageText.GetComponent<FloatingDamage>();
            if (floatingDamage != null)
            {
                floatingDamage.SetDamageText(finalDamage);
            }
        }

        // Chạy hiệu ứng rung + nhấp nháy màu
        StartCoroutine(HitEffect());

        // Kiểm tra chết
        if (health <= 0)
        {
            Die();
        }
    }


    void ApplyBuff()
    {
        if (!hasReceivedBuff && weakness == "supperdark" && lastBulletType == "orange")
        {
            damage += 10f;
            hasReceivedBuff = true; // Đánh dấu đã nhận buff
            Debug.Log($"🔥 Buff được áp dụng: damage mới = {damage}");
        }
    }

    public void DealDamage()
    {
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
               //Debug.Log($"Enemy gây {damage} sát thương!");
            }
        }
        //Debug.Log($"💥 Enemy gây {damage} sát thương! (Sau khi buff)");

    }

    void Die()
    {
        animator.SetTrigger("die");
        Debug.Log("💀 Enemy đã bị tiêu diệt!");
        Destroy(gameObject,1f);
    }

    void Flip(float direction)
    {
        if (direction > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
