using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusManager : MonoBehaviour {

    // Player health and armour values
    public int playerHealth = 1000;
    public int playerArmour = 1000;

    // UI component references
    public Image healthBar;
    public Image armourBar;

    public Text healthValue;
    public Text armourValue;

    // EGO manager references
    public GameObject failStateManager;
    public GameObject pauseManager;

    // Prevent the player from being destroyed between levels
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update player UI
    void Update ()
    {
        // Set UI values
        healthBar.fillAmount = playerHealth / 1000.0f;
        armourBar.fillAmount = playerArmour / 1000.0f;

        healthValue.text = Mathf.FloorToInt(playerHealth / 10.0f).ToString();
        armourValue.text = Mathf.FloorToInt(playerArmour / 10.0f).ToString();
    }

    // Called by enemies. Receieve damage and detect death
    public void TakeDamage(int damage)
    {
        int healthDamage = damage * 10;
        
        if (playerArmour > 0.0f)
        {
            int armourDamage = Mathf.FloorToInt(healthDamage * 0.75f);

            playerArmour -= armourDamage;

            if(playerArmour > 0.0f)
            {
                healthDamage = 0;
            }
            else
            {
                healthDamage = Mathf.FloorToInt(-playerArmour * 4 / 3);

                playerArmour = 0;
            }
        }

        playerHealth -= healthDamage;

        // Detect death
        if (playerHealth <= 0.0f)
        {
            OnDeath();
        }
    }

    // On player death
    private void OnDeath()
    {
        playerHealth = 0;

        this.GetComponent<PlayerController>().enabled = false;
        this.GetComponent<CombatController>().enabled = false;
        this.GetComponent<CameraController>().enabled = false;
        transform.Find("player pov").GetComponent<CameraController>().enabled = false;

        pauseManager.SetActive(false);

        failStateManager.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
    }
}
