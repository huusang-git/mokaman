using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("MapSelection"); // Chuyển đến màn hình chọn map
    }

    public void OpenSettings()
    {
        Debug.Log("Mở cài đặt (chưa triển khai)");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Thoát game!");
    }
}
