using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPersistor : MonoBehaviour {

    // Player to be set ingame
    public GameObject uiObject;

    // Ensure there is only one UI that isn't destroyed between levels
    void Awake()
    {
        uiObject = GameObject.Find("PersistentUI");

        if (uiObject == null)
        {
            uiObject = this.gameObject;
            
            uiObject.name = "PersistentUI";

            DontDestroyOnLoad(uiObject);
        }
        else
        {
            if (this.gameObject.name != "PersistentUI")
            {
                Destroy(this.gameObject);
            }
        }
    }
}
