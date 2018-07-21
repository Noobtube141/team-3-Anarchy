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
    private float lifetime = 20.0f;

	// Set auto delete
	void Start ()
    {
        Destroy(gameObject, lifetime);
	}

    // Apply effect on pick up
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (isHealth)
            {
                int newValue = other.GetComponent<PlayerStatusManager>().playerHealth += value;

                if(newValue > 100)
                {
                    other.GetComponent<PlayerStatusManager>().playerHealth = 100;
                }
            }
            else if (isArmour)
            {
                int newValue = other.GetComponent<PlayerStatusManager>().playerArmour += value;

                if (newValue > 100)
                {
                    other.GetComponent<PlayerStatusManager>().playerArmour = 100;
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
