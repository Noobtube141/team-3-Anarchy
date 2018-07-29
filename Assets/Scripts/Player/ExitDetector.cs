using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExitDetector : MonoBehaviour {

    // Scene to load upon clicking the exit object
    public string scene;

    // Componenet reference
    public GameObject waypointMarker;

    // Enable waypoint on activation (all enemies defeated)
    private void Start()
    {
        waypointMarker.SetActive(true);
    }

    // Detect input
    void Update ()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(cameraRay.origin, cameraRay.direction * 20, Color.blue);

            RaycastHit hit;

            if (Physics.Raycast(cameraRay, out hit))
            {
                if (hit.collider.tag == "Exit")
                {
                    //SceneManager.LoadScene(scene);

                    print("next level");
                }
            }
        }
	}
}
