using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusManager : MonoBehaviour {

    // Player to be set ingame
    public GameObject playerObject;

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

    // Ensure there is only one player that isn't destroyed between levels
    void Awake()
    {
        playerObject = GameObject.Find("PersistentPlayer");

        if (playerObject == null)
        {
            playerObject = this.gameObject;

            playerObject.name = "PersistentPlayer";

            DontDestroyOnLoad(playerObject);
        }
        else
        {
            if (this.gameObject.name != "PersistentPlayer")
            {
                Destroy(this.gameObject);
            }
        }
    }
    
    // Update player UI
    void Update ()
    {
        // Set UI values
        healthBar.fillAmount = playerHealth / 1000.0f;
        armourBar.fillAmount = playerArmour / 1000.0f;

        if(playerHealth > 0)
        {
            healthValue.text = Mathf.Clamp(Mathf.FloorToInt(playerHealth / 10.0f), 1, 1000).ToString();
        }
        else
        {
            healthValue.text = "0";
        }
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
            OnDeath(true);
        }
    }

    // On player death
    public void OnDeath(bool isDeath)
    {
        if (isDeath)
        {
            playerHealth = 0;
        }

        this.GetComponent<PlayerController>().enabled = false;
        this.GetComponent<CombatController>().enabled = false;
        this.GetComponent<CameraController>().enabled = false;
        transform.Find("player pov").GetComponent<CameraController>().enabled = false;
        
        pauseManager.SetActive(false);

        failStateManager.GetComponent<PauseManager>().isDeath = isDeath;

        failStateManager.SetActive(true);
    }
}
