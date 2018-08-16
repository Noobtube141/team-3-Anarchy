﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CombatController : MonoBehaviour {

    // All weapon and attack prefabs
    // Order from 0 - 7: pistol, ar, shotgun, sniper rifle, smg, broken bottle, knife, shovel
    public GameObject[] allWeapons;
    public GameObject[] allAttacks;

    private GameObject equipedWeapon;

    private string[] gunName = { "Pistol", "AR", "Shotgun", "Sniper Rifle", "SMG" };

    // Animator Reference
    private Animator weaponAnimator;

    private bool FireAnim = false;
    private bool SprintingAnim = false;
    private bool ReloadAnim = false;

    //Particle Reference and Disable
    public GameObject MuzzleFlashPistol;


    // Tracks current weapon
    public int currentWeapon = 0;
    public int weaponCount = 5;

    // Control combat delays
    private float currentTime = 0.0f;
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

    // Gun stats
    public int[] damage;
    public float[] bulletSpeed;
    public float[] bulletLife;
    public float[] bloom;

    // Sniper scope fov
    public int fovRange;
    public int fovAlt = 1;
    private Coroutine fovCoroutine;

    // Player status manager reference
    private PlayerController playerController;
    
    // UI elements
    public Text ammoDisplay;

    public Image sniperScope;

    public Image waypoint;

    // Bool controlling accessibility to exit object. Set true by spawn manager
    public bool canExit = false;

    // Scene to load upon clicking the exit object
    public string scene;

    // Radial inventory menu
    public GameObject inventory;
    public bool isInventoryActive = false;

    // Is it the last (third) level?
    public bool isLastLevel = false;

    // Set component references
    private void Start()
    {
        playerController = gameObject.GetComponent<PlayerController>();

        equipedWeapon = Instantiate(allWeapons[currentWeapon], Camera.main.transform);

        weaponAnimator = GetComponentInChildren<Animator>();
        MuzzleFlashPistol.SetActive(false);
    }

    // Respond to inputs and update UI
    void Update ()
    {
        // Exit on clicking exit object
        if (canExit)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                Debug.DrawRay(cameraRay.origin, cameraRay.direction * 20, Color.blue);

                RaycastHit hit;

                if (Physics.Raycast(cameraRay, out hit))
                {
                    if (hit.collider.tag == "Exit" && hit.distance < 1.5f)
                    {
                        if (isLastLevel)
                        {
                            gameObject.GetComponent<PlayerStatusManager>().OnDeath(false);
                            
                            return;
                        }
                        else
                        {
                            SceneManager.LoadScene(scene);
                        }
                    }
                }
            }
        }

        // Select weapons (only if not scoped)
        if (Camera.main.fieldOfView >= 58 && fovAlt == 1)
        {
            // Cycle weapons with scroll wheel
            if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
            {

                currentTime = Time.time;

                if (currentTime - attackTime > combatDelay[currentWeapon] && currentTime - reloadTime > reloadDelay[currentWeapon])
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

                    SwapWeapon();
                }
            }

            // Choose weapons with numbers
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                currentWeapon = 0;

                SwapWeapon();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                currentWeapon = 1;

                SwapWeapon();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                currentWeapon = 2;

                SwapWeapon();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                currentWeapon = 3;

                SwapWeapon();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                currentWeapon = 4;

                SwapWeapon();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                currentWeapon = 5;

                SwapWeapon();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                if (weaponCount > 5)
                {
                    currentWeapon = 6;

                    SwapWeapon();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                if (weaponCount > 6)
                {
                    currentWeapon = 7;

                    SwapWeapon();
                }
            }

            // Open radial menu
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                isInventoryActive = !isInventoryActive;

                inventory.SetActive(isInventoryActive);
            }
        }

        // Control combat while the inventory is closed and not running
        if (!isInventoryActive && !playerController.isSprinting)
        {
            // Control combat for pistol, shotgun, sniper rifle, broken bottle, knife and shovel
            if (currentWeapon != 1 && currentWeapon != 4)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    currentTime = Time.time;

                    if (currentTime - attackTime > combatDelay[currentWeapon])
                    {
                        if (currentWeapon < 5)
                        {
                            if (currentTime - reloadTime > reloadDelay[currentWeapon] && clipCurrent[currentWeapon] > 0)
                            {
                                FireGun();
                                FireAnim = true;
                                StartCoroutine(ResetFireAndReload());
                                MuzzleFlashPistol.SetActive(true);
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
                    currentTime = Time.time;

                    if (currentTime - attackTime > combatDelay[currentWeapon] && currentTime - reloadTime > reloadDelay[currentWeapon] && clipCurrent[currentWeapon] > 0)
                    {
                        FireGun();
                    }
                }
            }
        }
        // Enable Sprinting Animation
        if (playerController.isSprinting)
        {
            SprintingAnim = true;
        }
        else
        {
            SprintingAnim = false;
        }
        
        // Detect reload
        if (Input.GetKey(KeyCode.R))
        {
            if(currentWeapon < 5)
            {
                currentTime = Time.time;

                if (currentTime - attackTime > combatDelay[currentWeapon] && currentTime - reloadTime > reloadDelay[currentWeapon])
                {
                    ReloadGun();
                    ReloadAnim = true;
                    StartCoroutine(ResetFireAndReload());
                    reloadTime = currentTime;
                }
            }
        }

        // Control sniper scope
        if(currentWeapon == 3)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                fovAlt *= -1;

                if(fovCoroutine != null)
                {
                    StopCoroutine(fovCoroutine);
                }

                fovCoroutine = StartCoroutine(SniperScope(fovAlt));
            }
        }
        
        // Update ammo display
        if (currentWeapon == 0)
        {
            ammoDisplay.text = gunName[currentWeapon] + "\n" + clipCurrent[currentWeapon].ToString() + " / " + "\u221E";
        }
        else if (currentWeapon < 5)
        {
            ammoDisplay.text = gunName[currentWeapon] + "\n" + clipCurrent[currentWeapon].ToString() + " / " + ammo[currentWeapon].ToString();
        }
        else if (currentWeapon == 5)
        {
            ammoDisplay.text = "Knife";
        }
        else if (currentWeapon == 6)
        {
            ammoDisplay.text = "Broken bottle";
        }
        else if (currentWeapon == 7)
        {
            ammoDisplay.text = "Shovel";
        }
        else
        {
            ammoDisplay.text = "";
        }

        weaponAnimator.SetBool("Fire", FireAnim);
        weaponAnimator.SetBool("Sprinting", SprintingAnim);
        weaponAnimator.SetBool("Reload", ReloadAnim);
    }

    IEnumerator ResetFireAndReload()
    {
        yield return new WaitForEndOfFrame();

        FireAnim = false;
        ReloadAnim = false;

        yield return new WaitForSeconds(0.12f);
        MuzzleFlashPistol.SetActive(false);

    }

    // Instantiate bullet and decrement ammo
    void FireGun()
    {
        GameObject newBullet = Instantiate(allAttacks[currentWeapon], attackSpawn.position, attackSpawn.rotation * Quaternion.Euler(Random.Range(-bloom[currentWeapon], bloom[currentWeapon]),
            Random.Range(-bloom[currentWeapon], bloom[currentWeapon]), Random.Range(-bloom[currentWeapon], bloom[currentWeapon])));

        newBullet.GetComponent<BulletController>().type = currentWeapon;
        
        if (currentWeapon == 2)
        {
            for (int i = 0; i < 5; i++)
            {
                newBullet = Instantiate(allAttacks[currentWeapon], attackSpawn.position, attackSpawn.rotation * Quaternion.Euler(Random.Range(-bloom[currentWeapon], bloom[currentWeapon]),
                    Random.Range(-bloom[currentWeapon], bloom[currentWeapon]), Random.Range(-bloom[currentWeapon], bloom[currentWeapon])));

                newBullet.GetComponent<BulletController>().type = currentWeapon;
            }
        }

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

    // Alternate sniper scope zoom. Normal: 60, Scoped: 20
    IEnumerator SniperScope(int alt)
    {
        int newFOV = (60 - fovRange + fovRange * alt) * alt;
        
        while (Camera.main.fieldOfView * alt < newFOV)
        {
            Camera.main.fieldOfView += alt * 2;

            yield return new WaitForSeconds(0.01f);
        }

        Camera.main.fieldOfView = Mathf.Abs(newFOV);
    }

    // Try to swap to a weapon
    public void InventoryWeaponSelect(int number)
    {
        if(number <= weaponCount)
        {
            currentWeapon = number;

            SwapWeapon();
        }
    }

    // Swap weapon prefabs
    void SwapWeapon()
    {
        Destroy(equipedWeapon);

        equipedWeapon = Instantiate(allWeapons[currentWeapon], Camera.main.transform);

        weaponAnimator = GetComponentInChildren<Animator>();
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

    // Make exit UI visible and enable exiting
    public void EnableExiting()
    {
        waypoint.gameObject.SetActive(true);

        waypoint.GetComponent<WaypointManager>().FindNewExit();

        canExit = true;
    }

    /*void Update()
    {
        weaponAnimator.SetBool("Fire", FireAnim);
        weaponAnimator.SetBool("Sprinting", SprintingAnim);
        weaponAnimator.SetBool("Reload", ReloadAnim);
    }*/
}
