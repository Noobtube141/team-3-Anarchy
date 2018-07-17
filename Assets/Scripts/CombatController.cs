using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour {

    // All weapon prefabs
    // Order from 0 - 7: pistol, ar, shotgun, sniper rifle, smg, broken bottle, knife, shovel
    public GameObject[] allWeapons;

    // Control player access to melee weapons
    public bool isKnifeUsable = false;
    public bool isShovelUsable = false;

    // Tracks current weapon
    public int currentWeapon = 0;
    public int weaponCount = 5;

    // Control combat delays
    public float[] combatDelay;
    public float attackTime = 0.0f;
    public float[] reloadDelay;
    public float reloadTime = 0.0f;

    // Track gun ammunition
    public int[] ammo;
    public int[] clipMax;
    public int[] clipCurrent;
    
    // 
	void FixedUpdate ()
    {
        // Cycle weapons
        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
        {
            currentWeapon += Mathf.FloorToInt(Input.GetAxisRaw("Mouse ScrollWheel") / Mathf.Abs(Input.GetAxisRaw("Mouse ScrollWheel")));

            if(currentWeapon < 0)
            {
                currentWeapon += weaponCount + 1;
            }

            if(currentWeapon > weaponCount)
            {
                currentWeapon -= (weaponCount + 1);
            }

            print(currentWeapon);
        }

        // Control combat for pistol, shotgun, sniper rifle, broken bottle, knife and shovel
        if(currentWeapon != 1 && currentWeapon != 4)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (currentWeapon < 5)
                {
                    if (Time.time - attackTime > combatDelay[currentWeapon] && Time.time - reloadTime > reloadDelay[currentWeapon] && clipCurrent[currentWeapon] > 0)
                    {
                        FireGun();
                    }
                else
                {
                    if (Time.time - attackTime > combatDelay[currentWeapon])
                    {
                        MeleeAttack();
                    }
                }

                    attackTime = Time.time;
                }
            }
        }
        // Control combat for ar and smg
        else
        {
            if (Input.GetButton("Fire1"))
            {
                if (Time.time - attackTime > combatDelay[currentWeapon] && Time.time - reloadTime > reloadDelay[currentWeapon] && clipCurrent[currentWeapon] > 0)
                {
                    FireGun();
                    
                    attackTime = Time.time;
                }
            }
        }

        // Detect reload
        if (Input.GetKey(KeyCode.R))
        {
            if(currentWeapon < 5)
            {
                if (Time.time - attackTime > combatDelay[currentWeapon] && Time.time - reloadTime > reloadDelay[currentWeapon])
                {
                    ReloadGun();

                    reloadTime = Time.time;
                }
            }
        }
	}

    // 
    void FireGun()
    {
        // shooting

        clipCurrent[currentWeapon] -= 1;
    }

    // 
    void MeleeAttack()
    {
        // instantiate melee weapon prefab - 
    }

    // Control reloading
    void ReloadGun()
    {
        if(currentWeapon > 0)
        {
            int reloadAmount = clipMax[currentWeapon] - clipCurrent[currentWeapon];

            if (ammo[currentWeapon] >= reloadAmount)
            {
                clipCurrent[currentWeapon] += reloadAmount;

                ammo[currentWeapon] -= reloadAmount;
            }
            else
            {
                clipCurrent[currentWeapon] += ammo[currentWeapon];

                ammo[currentWeapon] = 0;
            }
        }
        else
        {
            clipCurrent[currentWeapon] = clipMax[currentWeapon];
        }
    }
}
