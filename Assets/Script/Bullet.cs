using UnityEngine;
using UnityEngine.Tilemaps; // Thêm thư viện này để làm việc với Tilemap

public class Bullet : MonoBehaviour
{
    public float speed = 10f;  // Tốc độ bay
    public float damage = 10f;
    public string bulletType; // Loại đạn (VD: "browncoffe", "whitecoffe")
    public float lifeTime = 3f; // Thời gian tồn tại

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float direction = transform.localScale.x > 0 ? 1f : -1f;
            rb.velocity = new Vector2(speed * direction, 0f);
        }
        else
        {
            Debug.LogError("❌ Rigidbody2D bị thiếu trên Bullet!");
        }

        Destroy(gameObject, lifeTime); // Hủy sau X giây để tránh rác
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu chạm kẻ địch, gây sát thương và hủy đạn
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, bulletType);
            Destroy(gameObject);
            return;
        }
        // Nếu chạm Boss, gây sát thương
        Boss boss = collision.GetComponent<Boss>();
        if (boss != null)
        {
            boss.TakeDamage(damage, bulletType);
            Destroy(gameObject);
            return;
        }
        // Nếu chạm vào Tilemap (mặt đất), hủy viên đạn
        if (collision.GetComponent<TilemapCollider2D>() != null)
        {
            Destroy(gameObject);
        }
    }
}
