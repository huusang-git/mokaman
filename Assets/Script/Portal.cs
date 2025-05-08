using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private AudioClip portalSFX; // SFX khi vào cổng
    private bool canInteract = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
            Debug.Log("Player gần cổng, nhấn E để vào!");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
        }
    }

    void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            CompleteLevel();
        }
    }

    void CompleteLevel()
    {
        if (portalSFX != null)
        {
            AudioManager.Instance?.PlaySFX(portalSFX);
        }
        Debug.Log("🎉 Màn chơi hoàn thành!");
        // Load scene tiếp theo hoặc gọi BossAreaController
        BossAreaController areaController = FindObjectOfType<BossAreaController>();
        if (areaController != null)
        {
            areaController.OnLevelCompleted();
        }
        else
        {
            // Fallback: Load scene tiếp theo
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.Log("Không còn scene tiếp theo, về MainMenu!");
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}