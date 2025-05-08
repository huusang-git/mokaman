using UnityEngine;
using TMPro;
using System.Collections;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh; // TextMeshProUGUI để hiển thị text
    private float lifetime = 1.5f; // Thời gian tồn tại
    private float moveDistance = 1.5f; // Khoảng cách di chuyển lên

    void Awake()
    {
        if (textMesh == null)
        {
            textMesh = GetComponentInChildren<TextMeshProUGUI>(); // Tự tìm TextMeshProUGUI
        }

        if (textMesh == null)
        {
            Debug.LogError("Không tìm thấy TextMeshProUGUI trên FloatingText!");
        }
    }

    public void SetText(string text)
    {
        if (textMesh == null) return;

        // Đặt text và màu xanh lá cây
        textMesh.text = text;
        textMesh.color = Color.green; // RGB: (0, 1, 0)

        // Bắt đầu hiệu ứng
        StartCoroutine(FadeAndMove());
    }

    private IEnumerator FadeAndMove()
    {
        float elapsedTime = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, moveDistance, 0); // Di chuyển lên
        Color startColor = textMesh.color; // Lưu màu xanh lá cây

        while (elapsedTime < lifetime)
        {
            // Di chuyển lên mượt mà
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / lifetime);

            // Mờ dần
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / lifetime);
            textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject); // Hủy sau khi hoàn tất
    }
}