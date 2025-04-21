using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("UI Screens")]
    public GameObject victoryScreen;
    public GameObject defeatScreen;

    private void Start()
    {
        // Отключаем UI при старте
        if (victoryScreen != null) victoryScreen.SetActive(false);
        if (defeatScreen != null) defeatScreen.SetActive(false);

        // На всякий случай возвращаем время
        Time.timeScale = 1f;
    }

    public void ShowVictory()
    {
        if (victoryScreen != null)
        {
            victoryScreen.SetActive(true);
            Time.timeScale = 0f; // Останавливаем игру
        }
    }

    public void ShowDefeat()
    {
        if (defeatScreen != null)
        {
            defeatScreen.SetActive(true);
            Time.timeScale = 0f; // Останавливаем игру
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Сбрасываем заморозку
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
