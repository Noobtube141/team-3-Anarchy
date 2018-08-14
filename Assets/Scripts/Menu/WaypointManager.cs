using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaypointManager : MonoBehaviour {

    // Component references
    private RectTransform imagePos;
    private Image waypoint;

    private Transform exitObject;
    private Transform playerTransform;
    
    // Set component references
    private void Start()
    {
        imagePos = GetComponent<RectTransform>();
        waypoint = GetComponent<Image>();

        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Keep image on object unless it is behind the player
    void LateUpdate ()
    {
        if(Vector3.Angle(exitObject.position - playerTransform.position, playerTransform.forward) < 90)
        {
            waypoint.fillAmount = 1;

            imagePos.position = Camera.main.WorldToScreenPoint(exitObject.position);
        }
        else
        {
            waypoint.fillAmount = 0;
        }
    }

    // Find new exit object
    public void FindNewExit()
    {
        print("test");

        exitObject = GameObject.FindGameObjectWithTag("Exit").GetComponent<Transform>();
    }
}
