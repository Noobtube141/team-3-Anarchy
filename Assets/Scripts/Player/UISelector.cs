using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelector : MonoBehaviour {

    // Is this the mobile UI?
    public bool isMobileUI = false;

    private void Awake()
    {
        // Check if running in the editor or standalone build
        #if UNITY_STANDALONE || UNITY_WEBPLAYER

        if (isMobileUI)
        {
            Destroy(gameObject);
        }

        // Check if running on mobile
        #elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

        if (!isMobileUI)
        {
            Destroy(gameObject);
        }

        #endif
    }
}
