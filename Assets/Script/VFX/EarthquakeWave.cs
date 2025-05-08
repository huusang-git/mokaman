using UnityEngine;

public class EarthquakeWave : MonoBehaviour
{
    public Vector2 direction = Vector2.zero;
    public float speed = 5f;
    public float damage = 20f; // Sát thương gây cho player

    void Start()
    {
        Destroy(gameObject, 1.5f); // Biến mất sau 5 giây
    }

    void Update()
    {
        if (direction != Vector2.zero)
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // Gây sát thương cho player
                Debug.Log($"đã gây {damage} sát thương");
            }
        }
    } 
}