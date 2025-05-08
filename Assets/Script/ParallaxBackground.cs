using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform; // Cinemachine Virtual Camera
    [SerializeField] private float parallaxFactor = 0.5f; // Tốc độ cuộn (0-1)
    [SerializeField] private AudioClip ambientSFX; // SFX môi trường
    private float spriteWidth; // Chiều rộng sprite
    private Vector3 startPos;
    private Transform[] sprites; // 2 sprite để lặp

    void Start()
    {
        // Tìm Cinemachine Virtual Camera nếu chưa gán
        if (cameraTransform == null)
        {
            var cinemachineCam = FindObjectOfType<Unity.Cinemachine.CinemachineVirtualCamera>();
            if (cinemachineCam != null)
            {
                cameraTransform = cinemachineCam.transform;
            }
            else
            {
                cameraTransform = Camera.main?.transform;
            }

            if (cameraTransform == null)
            {
                Debug.LogError($"ParallaxBackground ({gameObject.name}): Không tìm thấy Camera!");
                enabled = false;
                return;
            }
            Debug.Log($"ParallaxBackground ({gameObject.name}): Dùng camera {cameraTransform.name}");
        }

        startPos = transform.position;

        // Tính spriteWidth từ SpriteRenderer
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError($"ParallaxBackground ({gameObject.name}): Thiếu SpriteRenderer!");
            enabled = false;
            return;
        }
        spriteWidth = sr.bounds.size.x;
        Debug.Log($"ParallaxBackground ({gameObject.name}): spriteWidth={spriteWidth}");

        // Lấy 2 sprite con
        sprites = new Transform[2];
        if (transform.childCount < 2)
        {
            Debug.LogError($"ParallaxBackground ({gameObject.name}): Cần 2 sprite con để lặp!");
            enabled = false;
            return;
        }
        sprites[0] = transform.GetChild(0); // Sprite chính
        sprites[1] = transform.GetChild(1); // Sprite phụ
        Debug.Log($"ParallaxBackground ({gameObject.name}): Sprites - {sprites[0].name}, {sprites[1].name}");

        // Phát SFX môi trường
        if (ambientSFX != null)
        {
            AudioManager.Instance?.PlaySFX(ambientSFX);
            Debug.Log($"ParallaxBackground ({gameObject.name}): Phát SFX {ambientSFX.name}");
        }
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Tính vị trí dựa trên camera
        float deltaX = cameraTransform.position.x * parallaxFactor;
        Vector3 targetPos = startPos + new Vector3(deltaX, startPos.y, startPos.z);
        transform.position = targetPos;

        // Tính khung camera
        float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float cameraLeftEdge = cameraTransform.position.x - camHalfWidth;
        float cameraRightEdge = cameraTransform.position.x + camHalfWidth;

        // Lặp sprite
        for (int i = 0; i < sprites.Length; i++)
        {
            Transform sprite = sprites[i];
            float spriteLeft = sprite.position.x - spriteWidth / 2;
            float spriteRight = sprite.position.x + spriteWidth / 2;

            // Di chuyển sprite nếu ra khỏi camera
            if (spriteRight < cameraLeftEdge)
            {
                sprite.position += new Vector3(spriteWidth * sprites.Length, 0, 0);
                Debug.Log($"ParallaxBackground ({gameObject.name}): Di chuyển {sprite.name} sang phải");
            }
            else if (spriteLeft > cameraRightEdge)
            {
                sprite.position -= new Vector3(spriteWidth * sprites.Length, 0, 0);
                Debug.Log($"ParallaxBackground ({gameObject.name}): Di chuyển {sprite.name} sang trái");
            }
        }
    }
}