﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {
    
    // Image button references
    public Image[] imageButtons;

    // Player reference
    public CombatController combatController;

    // Knife and shovel buttons
    public Button knifeButton;
    public Button shovelButton;

    // Was a button clicked?
    private bool wasButtonClicked = false;

    // Left click close corooutine
    private Coroutine closeCoroutine;
    
    // Set hit alpha minimum
    private void Start()
    {
        foreach(Image img in imageButtons)
        {
            img.alphaHitTestMinimumThreshold = 1;
        }
    }

    // Free cursor on opening inventory and make appropriate buttons interactable
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;

        if(combatController.weaponCount > 5)
        {
            knifeButton.interactable = true;
        }

        if (combatController.weaponCount > 6)
        {
            shovelButton.interactable = true;
        }

        wasButtonClicked = false;
    }

    // Lock cursor on closing inventory
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Close inventory on input other than movement
    void LateUpdate ()
    {
        if (Input.GetButtonDown("Fire2") || Input.GetAxisRaw("Mouse ScrollWheel") != 0 || Input.GetKey(KeyCode.R) ||
            Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Alpha4) ||
            Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Alpha8))
        {
            DisableInventory();
        }

        if (Input.GetButtonDown("Fire1") && !wasButtonClicked)
        {
            closeCoroutine = StartCoroutine(CloseInventory());
        }
	}

    // Try to swap to a weapon
    public void SelectOnClick(int number)
    {
        wasButtonClicked = true;

        StopCoroutine(closeCoroutine);

        combatController.InventoryWeaponSelect(number);
        
        DisableInventory();
    }

    // Close inventory on left click next frame
    IEnumerator CloseInventory()
    {
        yield return new WaitForSeconds(0.1f);
        
        if (!wasButtonClicked)
        {
            DisableInventory();
        }
    }

    // Disable inventory and update combat controller
    void DisableInventory()
    {
        combatController.isInventoryActive = false;

        gameObject.SetActive(false);
    }
}
