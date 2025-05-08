using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image healthBarFill; // Thanh máu
    public Vector3 offset = new Vector3(0, 1f, 0); // Điều chỉnh vị trí trên đầu Enemy

    private Enemy enemy;
    private Transform enemyTransform;
    private Camera mainCamera;

    void Start()
    {
        enemy = GetComponentInParent<Enemy>(); // Lấy Enemy script từ parent
        enemyTransform = enemy.transform; // Lấy Transform của Enemy
        mainCamera = Camera.main; // Lấy camera chính
    }

    void Update()
    {
        if (enemy != null)
        {
            // Cập nhật lượng máu
            healthBarFill.fillAmount = enemy.health / 100f;

            // Luôn cập nhật vị trí của thanh máu theo enemy
            transform.position = enemyTransform.position + offset;

            // Giữ nguyên hướng nhìn về camera (không bị xoay theo enemy)
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}
