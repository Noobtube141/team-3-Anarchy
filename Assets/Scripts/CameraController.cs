using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    // Boolean for whether or not rotation is horizontal (body/player) rather than vertical (head/camera)
    public bool isRotatingHorizontally = false;

    // Mouse sensitivity
    public float mouseSensitivityX;
    public float mouseSensitivityY;

    // Minimum and maximum vertical rotation
    public float minVerticalRotation = -45.0f;
    public float maxVerticalRotation =  45.0f;

    // Current rotations
    public float rotationHorizontal = 0;
    public float rotationVertical = 0;

    // Set mouse sensitivity
    void Start ()
    {
        mouseSensitivityX = PlayerPrefs.GetFloat("MouseSensitivityX", 15.0f);
        mouseSensitivityY = PlayerPrefs.GetFloat("MouseSensitivityY", 15.0f);
    }
	
	// Rotate based on mouse movement
	void Update ()
    {
        if (isRotatingHorizontally)
        {
            rotationHorizontal += Input.GetAxis("Mouse X") * mouseSensitivityX;

            Quaternion quaternionHorizontal = Quaternion.AngleAxis(rotationHorizontal, Vector3.up);

            transform.localRotation = Quaternion.identity * quaternionHorizontal;
        }
        else
        {
            rotationVertical += Input.GetAxis("Mouse Y") * mouseSensitivityY;

            if (rotationVertical < -45.0f)
            {
                rotationVertical = -45.0f;
            }
            if (rotationVertical >  45.0f)
            {
                rotationVertical =  45.0f;
            }

            Quaternion quaternionVertical = Quaternion.AngleAxis(rotationVertical, Vector3.left);

            transform.localRotation = Quaternion.identity * quaternionVertical;
        }
	}
}
