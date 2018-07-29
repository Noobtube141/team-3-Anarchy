using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatController : MonoBehaviour {

    // All weapon and attack prefabs
    // Order from 0 - 7: pistol, ar, shotgun, sniper rifle, smg, broken bottle, knife, shovel
    public GameObject[] allWeapons;
    public GameObject[] allAttacks;
    
    // Tracks current weapon
    public int currentWeapon = 0;
    public int weaponCount = 5;

    // Control combat delays
    public float[] combatDelay;
    private float attackTime = 0.0f;
    public float[] reloadDelay;
    private float reloadTime = 0.0f;

    // Track gun ammunition
    public int[] ammo;
    public int[] clipMax;
    public int[] clipCurrent;

    // Attack spawn point
    public Transform attackSpawn;

    // UI elements
    public Text ammoDisplay;

    // Bool controlling accessibility to exit object
    //public bool canExit = false;

    // Respond to inputs and update UI
    void FixedUpdate ()
    {
        // Cycle weapons
        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
        {
            if (Time.time - attackTime > combatDelay[currentWeapon] && Time.time - reloadTime > reloadDelay[currentWeapon])
            {
                currentWeapon += Mathf.FloorToInt(Input.GetAxisRaw("Mouse ScrollWheel") / Mathf.Abs(Input.GetAxisRaw("Mouse ScrollWheel")));

                if (currentWeapon < 0)
                {
                    currentWeapon += weaponCount + 1;
                }

                if (currentWeapon > weaponCount)
                {
                    currentWeapon -= (weaponCount + 1);
                }

                //print(currentWeapon);
            }
        }

        // Control combat for pistol, shotgun, sniper rifle, broken bottle, knife and shovel
        if(currentWeapon != 1 && currentWeapon != 4)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (Time.time - attackTime > combatDelay[currentWeapon])
                {
                    if(currentWeapon < 5)
                    {
                        if(Time.time - reloadTime > reloadDelay[currentWeapon] && clipCurrent[currentWeapon] > 0)
                        {
                            FireGun();
                        }
                    }
                    else
                    {
                        MeleeAttack();
                    }
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

        // Update ammo display
        if (currentWeapon == 0)
        {
            ammoDisplay.text = clipCurrent[currentWeapon].ToString() + " / " + "\u221E";
        }
        else if (currentWeapon < 5)
        {
            ammoDisplay.text = clipCurrent[currentWeapon].ToString() + " / " + ammo[currentWeapon].ToString();
        }
        else if (currentWeapon == 5)
        {
            ammoDisplay.text = "Broken bottle";
        }
        else if (currentWeapon == 6)
        {
            ammoDisplay.text = "Knife";
        }
        else if (currentWeapon == 7)
        {
            ammoDisplay.text = "Shovel";
        }
        else
        {
            ammoDisplay.text = "";
        }
	}

    // Instantiate bullet and decrement ammo
    void FireGun()
    {
        Instantiate(allAttacks[currentWeapon], attackSpawn.position, attackSpawn.rotation);

        clipCurrent[currentWeapon] -= 1;

        attackTime = Time.time;
    }

    // Instantiate melee weapon
    void MeleeAttack()
    {
        Instantiate(allAttacks[currentWeapon], attackSpawn.position + Vector3.down * 0.125f, attackSpawn.rotation, gameObject.transform);

        attackTime = Time.time;
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

    // Increase weaponCount to allow access to knife
    public void MakeKnifeUsable()
    {
        weaponCount = 6;
    }

    // Increase weaponCount to allow access to shovel
    public void MakeShovelUsable()
    {
        weaponCount = 7;
    }
}
