using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectionUI : MonoBehaviour
{
    public void LoadMap1()
    {
        SceneManager.LoadScene("Map1"); // Đổi tên nếu cần
    }

    public void LoadMap2()
    {
        SceneManager.LoadScene("Map2"); // Đổi tên nếu cần
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
