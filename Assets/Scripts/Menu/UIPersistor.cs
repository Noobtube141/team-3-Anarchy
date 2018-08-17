using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPersistor : MonoBehaviour {

    // UI element to be set ingame
    public GameObject uiObject;

    // Which UI element is this?
    public bool isPlayerUI;
    public bool isPauseUI;
    public bool isFailSuccessUI;

    // The name of the object to find
    private string UIName;

    //Fail/success manager
    public GameObject failSuccessManager;
    
    // Ensure there is only one UI that isn't destroyed between levels
    void Awake()
    {
        if (isPlayerUI)
        {
            UIName = "PersistentPlayerUI";
        }
        else if (isPauseUI)
        {
            UIName = "PersistentPauseUI";
        }
        else if (isFailSuccessUI)
        {
            UIName = "PersistentFailSuccessUI";
        }

        uiObject = GameObject.Find(UIName);

        if (uiObject == null)
        {
            uiObject = this.gameObject;
            
            uiObject.name = UIName;

            DontDestroyOnLoad(uiObject);
            
            GameObject otherUIObject = GameObject.Find("PersistentFailSuccessUI");

            if (otherUIObject == null)
            {
                otherUIObject = failSuccessManager;

                failSuccessManager.name = "PersistentFailSuccessUI";

                DontDestroyOnLoad(failSuccessManager);

                failSuccessManager.GetComponent<UIPersistor>().UIName = "PersistentFailSuccessUI";
            }
            else
            {
                if (this.gameObject.name != UIName)
                {
                    Destroy(this.gameObject);
                }
            }
        }
        else
        {
            if (this.gameObject.name != UIName)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
