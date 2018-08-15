using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    // Is this the pause menu or the fail/success state manager
    public bool isPause;

    // Is this the fail or success state?
    public bool isDeath;

    // The string to be displayed on fail/success
    public string stateMessage = "You've died";

    // The text component to use the above string
    public Text stateText;

    // Make cursor invisible
    private void Start()
    {
        if (isPause)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Update for fail/success message
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;

        Cursor.visible = true;
        
        if (!isPause)
        {
            if (!isDeath)
            {
                stateMessage = "You win!";

                stateText.text = stateMessage;

                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    // Pause on pressing escape
    void Update ()
    {
        if (!isPaused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
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
    
    // Pause game when window is covered
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (!isPaused)
            {
                TogglePause();
            }
        }
    }

    // Pause game when leaving window
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            if (!isPaused)
            {
                TogglePause();
            }
        }
    }

    // Return to main menu on click
    public void ReturnToMenu()
    {
        GameObject.FindGameObjectWithTag("Music Player").SendMessage("CrossFade", "IntoMenu");

        SceneManager.LoadScene(scene);
    }
}
