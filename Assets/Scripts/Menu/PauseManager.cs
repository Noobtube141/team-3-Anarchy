using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour {

    // Is the game paused?
    public bool isPaused = false;

    // Pause menu
    public GameObject pauseMenu;

    // Scene to load
    public string scene;

	// Pause on pressing escape
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
        }

        if (isPaused)
        {
            Time.timeScale = 0;

            pauseMenu.SetActive(isPaused);
        }
        else
        {
            Time.timeScale = 1;

            pauseMenu.SetActive(isPaused);
        }
	}

    // Toggle pause status on button click
    public void TogglePause()
    {
        isPaused = !isPaused;
    }

    // Return to main menu on click
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(scene);
    }
}
