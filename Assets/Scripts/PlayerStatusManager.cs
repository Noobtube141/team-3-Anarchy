using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusManager : MonoBehaviour {

    // Player health and armour values
    public int playerHealth = 100;
    public int playerArmour = 100;

    // UI component references
    public Image healthBar;
    public Image armourBar;

    public Text healthValue;
    public Text armourValue;

    // EGO manager references
    public GameObject failStateManager;
    public GameObject pauseManager;

    // Temporary
    public float countdown = 7.5f;

    // stuff?
	void Start ()
    {
		
	}
	
    // Update player UI
	void Update ()
    {
        // Set UI values
        healthBar.fillAmount = playerHealth / 100.0f;
        armourBar.fillAmount = playerArmour / 100.0f;

        healthValue.text = playerHealth.ToString();
        armourValue.text = playerArmour.ToString();

        // Temporary - auto damage player
        countdown -= Time.deltaTime;

        if(countdown <= 0.0f)
        {
            TakeDamage(100);

            countdown = 1.0f;
        }
    }

    // Called by enemies. Receieve damage and detect death
    public void TakeDamage(int damage)
    {
        int healthDamage = damage;

        if (playerArmour > 0.0f)
        {
            int armourDamage = Mathf.FloorToInt(damage * 0.75f);

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
        this.GetComponent<CameraController>().enabled = false;
        transform.Find("player pov").GetComponent<CameraController>().enabled = false;

        pauseManager.SetActive(false);

        failStateManager.SetActive(true);
    }
}
