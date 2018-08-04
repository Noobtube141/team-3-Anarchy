using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    // Sliders
    public Slider sliderX;
    public Slider sliderY;
    public Slider sliderSound;
    public Slider sliderGraphics;

    // Toggle
    public Toggle toggleInversion;

    // Load level on button click
    public void LoadSceneOnClick()
    {
        SceneManager.LoadScene(level);
    }

    // Set initial menu activity, cursor lock state and settings (player prefs)
    private void Awake()
    {
        menu.SetActive(isMenuActive);
        settings.SetActive(isSettingsActive);

        Cursor.lockState = CursorLockMode.None;

        sliderX.value = PlayerPrefs.GetFloat("MouseSensitivityX", 10);
        sliderY.value = PlayerPrefs.GetFloat("MouseSensitivityY", 10);

        if (PlayerPrefs.GetInt("MouseInversion", 1) == 1)
        {
            toggleInversion.isOn = false;
        }
        else if (PlayerPrefs.GetInt("MouseInversion", 1) == -1)
        {
            toggleInversion.isOn = true;
        }

        sliderSound.value = PlayerPrefs.GetFloat("SoundLevels", 1);

        sliderGraphics.value = PlayerPrefs.GetInt("GraphicsSettings", 4);
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

    // Set slider values (mouse sensitivity, sound and graphics)
    public void SetSliderValue(string type)
    {
        if(type == "X")
        {
            PlayerPrefs.SetFloat("MouseSensitivityX", sliderX.value);
        }
        else if (type == "Y")
        {
            PlayerPrefs.SetFloat("MouseSensitivityY", sliderY.value);
        }
        else if (type == "Sound")
        {
            PlayerPrefs.SetFloat("SoundLevels", sliderSound.value);

            AudioListener.volume = PlayerPrefs.GetFloat("SoundLevels", 1);
        }
        else if (type == "Graphics")
        {
            PlayerPrefs.SetInt("GraphicsSettings", (int)sliderGraphics.value);

            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("GraphicsSettings", 4));
        }
    }

    // Set mouse inversion
    public void ToggleInversion()
    {
        if (toggleInversion.isOn)
        {
            PlayerPrefs.SetInt("MouseInversion", -1);
        }
        else
        {
            PlayerPrefs.SetInt("MouseInversion", 1);
        }
    }


}
