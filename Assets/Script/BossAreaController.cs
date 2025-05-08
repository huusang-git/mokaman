using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BossAreaController : MonoBehaviour
{
    public GameObject boss; // Gán GameObject của boss
    public DialogueManager dialogueManager; // Gán DialogueManager
    public PolygonCollider2D confinerBounds; // Ranh giới camera
    public Vector2 areaSize = new Vector2(20f, 10f); // Kích thước khu vực boss
    public LayerMask boundaryLayer; // Layer của tường, ví dụ "Ground"
    public Sprite wallSprite; // Sprite cho tường
    [SerializeField] private AudioClip wallSpawnSFX; // Âm thanh khi tường spawn
    [SerializeField] private AudioClip bossWakeSFX; // Âm thanh khi boss thức

    private bool hasTriggered = false;
    private CinemachineCamera virtualCamera;
    private CinemachineConfiner2D confiner;
    private List<GameObject> boundaryWalls = new List<GameObject>();
    private Boss bossScript;

    void Start()
    {
        virtualCamera = Object.FindFirstObjectByType<CinemachineCamera>();
        if (virtualCamera == null)
        {
            Debug.LogError("BossAreaController: Không tìm thấy CinemachineCamera!");
            return;
        }

        confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
        if (confiner == null)
        {
            confiner = virtualCamera.gameObject.AddComponent<CinemachineConfiner2D>();
        }

        if (dialogueManager != null)
        {
            dialogueManager.OnDialogueComplete += OnDialogueFinished;
        }
    }

    void OnDestroy()
    {
        if (dialogueManager != null)
        {
            dialogueManager.OnDialogueComplete -= OnDialogueFinished;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            Debug.Log("Player vào khu vực boss!");

            bossScript = boss.GetComponent<Boss>();
            if (bossScript == null)
            {
                Debug.LogError("BossAreaController: Không tìm thấy Boss script!");
            }

            if (dialogueManager != null)
            {
                dialogueManager.StartDialogue(bossScript);
                Debug.Log("Bắt đầu hội thoại boss!");
            }
            else
            {
                Debug.LogError("BossAreaController: DialogueManager chưa được gán!");
                if (bossScript != null)
                {
                    bossScript.WakeUp();
                    AudioManager.Instance?.PlaySFX(bossWakeSFX);
                    Debug.Log("Boss thức dậy (không có hội thoại)!");
                }
            }

            if (confiner != null && confinerBounds != null)
            {
                confiner.BoundingShape2D = confinerBounds;
                confiner.InvalidateBoundingShapeCache();
                Debug.Log("Camera giới hạn trong khu vực boss!");
            }

            SpawnBoundaryWalls();
        }
    }

    void OnDialogueFinished()
    {
        if (bossScript != null)
        {
            bossScript.WakeUp();
            AudioManager.Instance?.PlaySFX(bossWakeSFX);
            Debug.Log("Boss thức dậy sau hội thoại!");
        }
    }

    void SpawnBoundaryWalls()
    {
        Vector2 center = transform.position;
        float width = areaSize.x;
        float height = areaSize.y;

        CreateWall(new Vector2(center.x - width / 2, center.y), new Vector2(1f, height));
        CreateWall(new Vector2(center.x + width / 2, center.y), new Vector2(1f, height));

        AudioManager.Instance?.PlaySFX(wallSpawnSFX);
        Debug.Log("Đã spawn tường trái và phải!");
    }

    void CreateWall(Vector2 position, Vector2 size)
    {
        GameObject wall = new GameObject("BoundaryWall");
        wall.layer = boundaryLayer.value;

        BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
        collider.size = size;

        SpriteRenderer renderer = wall.AddComponent<SpriteRenderer>();
        if (wallSprite != null)
        {
            renderer.sprite = wallSprite;
            renderer.sortingLayerName = "Foreground";
            renderer.sortingOrder = 0;
        }
        else
        {
            Debug.LogWarning("BossAreaController: wallSprite chưa được gán!");
        }

        wall.transform.position = position;
        boundaryWalls.Add(wall);
    }

    public void OnBossDefeated()
    {
        if (hasTriggered)
        {
            hasTriggered = false;

            foreach (GameObject wall in boundaryWalls)
            {
                if (wall != null)
                {
                    Destroy(wall);
                }
            }
            boundaryWalls.Clear();
            Debug.Log("Đã xóa tường!");

            if (confiner != null)
            {
                confiner.BoundingShape2D = null;
                confiner.InvalidateBoundingShapeCache();
                Debug.Log("Boss chết, camera trở lại bình thường!");
            }

            bossScript = null;
        }
    }

    public void OnLevelCompleted()
    {
        Debug.Log("🎉 Màn chơi hoàn thành!");
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Không còn scene, về MainMenu!");
            SceneManager.LoadScene("MainMenu");
        }
    }
}