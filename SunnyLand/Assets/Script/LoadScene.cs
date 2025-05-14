using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor; 
#endif

public class LoadScene : MonoBehaviour
{
    private bool isPaused = false;

    public GameObject pauseMenuUI; 

    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; 
#else
        Application.Quit();
#endif
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }
}
