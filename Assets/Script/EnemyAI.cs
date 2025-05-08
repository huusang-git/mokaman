using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public float speed = 2f;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float damage = 10f;
    public float patrolDistance = 5f;
    public float waitTimeBeforeReturning = 1.5f;
    public float health = 100f; // 🩸 Máu của kẻ địch
    public string weakness; // 🔥 Điểm yếu của kẻ địch (supperdark, mixed, orangeespresso)

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
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
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
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        animator.SetBool("isRunning", true);
        Flip(targetPosition.x - transform.position.x);

        if (Vector2.Distance(transform.position, targetPosition) < 0.2f)
        {
            targetPosition = targetPosition.x > patrolOrigin.x ?
                patrolOrigin - new Vector2(patrolDistance, 0) :
                patrolOrigin + new Vector2(patrolDistance, 0);
        }
    }

    void ReturnToPatrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, patrolOrigin, speed * Time.deltaTime);
        animator.SetBool("isRunning", true);
        Flip(patrolOrigin.x - transform.position.x);

        if (Vector2.Distance(transform.position, patrolOrigin) < 0.2f)
        {
            returningToPatrol = false;
            animator.SetBool("isRunning", false);
        }
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

    public void DealDamage()
    {
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"Enemy gây {damage} sát thương!");
            }
        }
    }

    public void SetPatrolOrigin(Vector2 position)
    {
        patrolOrigin = position;
        targetPosition = patrolOrigin + new Vector2(patrolDistance, 0);
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

    // 🆕 Nhận sát thương từ đạn
    public void TakeDamage(float damage, string bulletType)
    {
        float finalDamage = damage;

        // Áp dụng yếu tố điểm yếu
        if (bulletType == weakness)
        {
            finalDamage *= 2f; // Nếu bắn đúng điểm yếu, nhận x2 sát thương
        }

        // Áp dụng hiệu ứng đặc biệt của kẻ địch
        if (weakness == "mixed" && bulletType == "supperdark")
        {
            speed *= 2f; // Tăng tốc độ di chuyển gấp đôi
            Debug.Log("⚡ Enemy Mixed nhận supperdark! Tăng tốc độ di chuyển.");
        }
        else if (weakness == "orange" && bulletType == "mixed")
        {
            health += finalDamage * 2f; // Hồi máu gấp đôi lượng sát thương nhận vào
            Debug.Log("🍊 Enemy Orange hồi máu!");
        }
        else if (weakness == "supperdark" && bulletType == "orange")
        {
            damage += 10f; // Tăng thêm 10 sát thương
            Debug.Log("🔥 Enemy Supperdark nhận Orange! Tăng sát thương.");
        }

        // Trừ máu kẻ địch
        health -= finalDamage;
        Debug.Log($"🩸 Enemy nhận {finalDamage} sát thương từ {bulletType}. Máu còn: {health}");

        // Kiểm tra chết
        if (health <= 0)
        {
            Die();
        }
    }


    void Die()
    {
        Debug.Log("💀 Enemy đã bị tiêu diệt!");
        Destroy(gameObject);
    }
}
