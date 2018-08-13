using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour {
    
    // The type of pickup
    public bool isHealth;
    public bool isArmour;
    public bool isAmmo;
    public int ammoType;
    public bool isKnife;
    public bool isShovel;

    // Value of pickup
    public int value;

    // Lifetime of pickup
    private float lifetime = 7.0f;

	// Set auto delete
	void Start ()
    {
        if (isKnife)
        {
            if(GameObject.FindGameObjectWithTag("Player").GetComponent<CombatController>().weaponCount == 6)
            {
                GameObject.FindGameObjectWithTag("Shovel").SetActive(true);

                Destroy(gameObject);
            }
        }
        else if (isShovel)
        {
            // Nothing
        }
        else
        {
            Destroy(gameObject, lifetime);
        }
	}

    // Apply effect on pick up
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (isHealth)
            {
                int newValue = other.GetComponent<PlayerStatusManager>().playerHealth += value * 10;

                if(newValue > 1000)
                {
                    other.GetComponent<PlayerStatusManager>().playerHealth = 1000;
                }
            }
            else if (isArmour)
            {
                int newValue = other.GetComponent<PlayerStatusManager>().playerArmour += value * 10;

                if (newValue > 1000)
                {
                    other.GetComponent<PlayerStatusManager>().playerArmour = 1000;
                }
            }
            else if (isAmmo)
            {
                other.GetComponent<CombatController>().ammo[ammoType] += value;
            }
            else if (isKnife)
            {
                other.GetComponent<CombatController>().MakeKnifeUsable();
            }
            else if (isShovel)
            {
                other.GetComponent<CombatController>().MakeShovelUsable();
            }

            Destroy(gameObject);
        }
    }
}
