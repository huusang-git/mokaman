using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    void Awake()
    {
        Instance = this;
    }

    public void ShakeCamera(float magnitude, float duration)
    {
        StartCoroutine(Shake(magnitude, duration));
    }

    private IEnumerator Shake(float magnitude, float duration)
    {
        Vector3 currentPos = transform.position; // Lấy vị trí hiện tại khi bắt đầu rung
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector2 randomOffset = Random.insideUnitCircle * magnitude; // Chỉ rung trên x và y
            transform.position = new Vector3(
                currentPos.x + randomOffset.x,
                currentPos.y + randomOffset.y,
                currentPos.z // Giữ nguyên trục z
            );
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = currentPos; // Đặt lại vị trí ban đầu sau khi rung
    }
}