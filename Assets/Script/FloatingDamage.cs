using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FloatingDamage : MonoBehaviour
{
    public TextMeshPro textMesh; // Text hiển thị số damage
    public float moveSpeed = 1f; // Tốc độ di chuyển lên
    public float fadeDuration = 1f; // Thời gian mờ dần

    private Color startColor; // Màu ban đầu của text

    void Awake()
    {
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshPro>(); // Tự động tìm TextMeshPro nếu chưa gán
        }

        if (textMesh == null)
        {
            Debug.LogError(" Không tìm thấy TextMeshPro trên FloatingDamage!");
        }
    }

    public void SetDamageText(float damage)
    {
        if (textMesh == null) return;
        // Màu mặc định
        textMesh.color = Color.white;

        // Hiển thị sát thương (làm tròn số)
        textMesh.text = damage.ToString("F0");

        // Đổi màu text dựa trên mức sát thương
        if (damage < 10)
            textMesh.color = Color.green; // Sát thương thấp
        else if (damage < 30)
            textMesh.color = Color.yellow; // Sát thương trung bình
        else
            textMesh.color = Color.red; // Sát thương cao

        // Lưu màu ban đầu để dùng khi mờ dần
        startColor = textMesh.color;

        // Kích hoạt hiệu ứng di chuyển và mờ dần
        StartCoroutine(FadeAndMove());
    }

    private IEnumerator FadeAndMove()
    {
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, 1.5f, 0); // Di chuyển lên trên

        while (elapsedTime < fadeDuration)
        {
            // Di chuyển lên
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / fadeDuration);

            // Mờ dần
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject); // Xóa object sau khi hoàn tất hiệu ứng
    }
}
