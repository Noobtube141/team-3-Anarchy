using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    // Level to be loaded (main game)
    public string level;

    // Bools controlling the active menu
    public bool isMenuActive = true;
    public bool isSettingsActive = false;

    // EGOs containing menu UIs
    public GameObject menu;
    public GameObject settings;
    
    // Load level on button click
    public void LoadSceneOnClick()
    {
        SceneManager.LoadScene(level);
    }

    // Set initial menu activity
    private void Awake()
    {
        menu.SetActive(isMenuActive);
        settings.SetActive(isSettingsActive);
    }

    // Toggle main menu
    public void ToggleMenu()
    {
        isMenuActive = !isMenuActive;
        
        menu.SetActive(isMenuActive);
    }

    // Toggle settings menu
    public void ToggleSettings()
    {
        isSettingsActive = !isSettingsActive;

        settings.SetActive(isSettingsActive);
    }

    // Quit game
    public void QuitGame()
    {
        Application.Quit();
    }
}
