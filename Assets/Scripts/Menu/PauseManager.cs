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

    // Radial inventory menu
    public GameObject inventory;

    // Make cursor invisible
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Pause on pressing escape
    void Update ()
    {
        if (!isPaused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            //if (Input.GetKeyDown(KeyCode.Q))
            {
                TogglePause();

                inventory.SetActive(false);
            }
        }
	}
    
    // Toggle pause status
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0;

            pauseMenu.SetActive(isPaused);

            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1;

            pauseMenu.SetActive(isPaused);

            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Return to main menu on click
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(scene);
    }

    // Pause game when window is covered
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            isPaused = pause;

            Cursor.lockState = CursorLockMode.None;
        }
    }

    // Pause game when leaving window
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            isPaused = !focus;

            Cursor.lockState = CursorLockMode.None;
        }
    }
}
