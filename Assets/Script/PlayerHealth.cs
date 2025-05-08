using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f; // Máu tối đa của người chơi
    private float currentHealth;
    private SpriteRenderer spriteRenderer;
    private bool isInvulnerable = false; // Ngăn nhận sát thương khi đang chớp

    void Start()
    {
        currentHealth = maxHealth; // Khởi tạo máu đầy
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("PlayerHealth: Không tìm thấy SpriteRenderer trên Player!");
        }
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isInvulnerable || currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log($"💥 Player nhận {damage} sát thương! Máu còn: {currentHealth}");

        if (spriteRenderer != null)
        {
            StartCoroutine(FlashEffect());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashEffect()
    {
        isInvulnerable = true;
        int flashCount = 3; // Số lần chớp
        float flashDuration = 0.1f; // Thời gian mỗi lần chớp

        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f); // Chớp trắng mờ
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = Color.white; // Khôi phục màu gốc
            yield return new WaitForSeconds(flashDuration);
        }

        spriteRenderer.color = Color.white; // Đảm bảo màu gốc
        isInvulnerable = false;
    }

    void Die()
    {
        Debug.Log("☠️ Player đã chết!");
        // Ở đây bạn có thể gọi màn hình Game Over hoặc Respawn
        gameObject.SetActive(false);
    }
}