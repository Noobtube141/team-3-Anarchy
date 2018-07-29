using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaypointManager : MonoBehaviour {

    // Component references
    private RectTransform imagePos;

    public Transform exitObject;
    private Transform playerTransform;
    
    // Set component references
    private void Start()
    {
        imagePos = GetComponent<RectTransform>();

        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Keep image on object unless it is behind the player
    void LateUpdate ()
    {
        if(Vector3.Angle(exitObject.position - playerTransform.position, playerTransform.forward) < 135)
        {
            imagePos.position = Camera.main.WorldToScreenPoint(exitObject.position);
        }
    }
}
