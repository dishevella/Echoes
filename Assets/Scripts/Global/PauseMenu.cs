using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseUI;

    [Header("Scene")]
    public string backSceneName = "StartMenu"; // 返回的场景名，比如开始菜单或选关界面

    private bool isPaused = false;

    void Awake()
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }

        Time.timeScale = 1f;
        isPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(true);
        }

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        isPaused = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitLevel()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (!string.IsNullOrEmpty(backSceneName))
        {
            SceneManager.LoadScene(backSceneName);
        }
    }
}