using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    // Boolean for whether or not rotation is horizontal (body/player) rather than vertical (head/camera)
    public bool isRotatingHorizontally = false;

    // Mouse sensitivity
    public float mouseSensitivityX;
    public float mouseSensitivityY;

    // Mouse inversion
    public int mouseInversion;

    // Minimum and maximum vertical rotation
    public float minVerticalRotation = -45.0f;
    public float maxVerticalRotation =  45.0f;

    // Current rotations
    public float rotationHorizontal = 0;
    public float rotationVertical = 0;

    // Pause manager
    public PauseManager pauseManager;

    // Touch to detect as moving the camera
    public int cameraTouchID;

    // Joystick to prevent looking while moving
    public JoystickController joystickController;

    // Set mouse sensitivity
    void Start ()
    {
        mouseSensitivityX = PlayerPrefs.GetFloat("MouseSensitivityX", 15.0f);
        mouseSensitivityY = PlayerPrefs.GetFloat("MouseSensitivityY", 15.0f);

        mouseInversion = PlayerPrefs.GetInt("MouseInversion", 1);
    }
	
	// Rotate based on mouse movement
	void Update ()
    {
        if (!pauseManager.isPaused)
        {
            if (isRotatingHorizontally)
            {
                // Check if running in the editor or standalone build
                #if UNITY_STANDALONE || UNITY_WEBPLAYER

                rotationHorizontal += Input.GetAxis("Mouse X") * mouseSensitivityX;

                // Check if running on mobile
                #elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

                if(Input.touchCount > 0)
                {
                    foreach(Touch fingerTouch in Input.touches)
                    {
                        if(fingerTouch.fingerId != joystickController.joystickTouchID)
                        {
                            if (EventSystem.current.IsPointerOverGameObject(fingerTouch.fingerId))
                            {
                                // Empty
                            }
                            else if (!EventSystem.current.IsPointerOverGameObject(fingerTouch.fingerId))
                            {
                                if (fingerTouch.phase == TouchPhase.Began)
                                {
                                    cameraTouchID = fingerTouch.fingerId;
                                }
                            }

                            if (!EventSystem.current.IsPointerOverGameObject(cameraTouchID))
                            {
                                if (Input.touches[cameraTouchID].phase == TouchPhase.Moved)
                                {
                                    rotationHorizontal += Input.touches[cameraTouchID].deltaPosition.x * mouseSensitivityX * 0.1f;
                                }
                            }
                        }
                    }
                }
                
                #endif

                Quaternion quaternionHorizontal = Quaternion.AngleAxis(rotationHorizontal, Vector3.up);

                transform.localRotation = Quaternion.identity * quaternionHorizontal;
            }
            else
            {
                // Check if running in the editor or standalone build
                #if UNITY_STANDALONE || UNITY_WEBPLAYER

                rotationVertical += Input.GetAxis("Mouse Y") * mouseSensitivityY * mouseInversion;

                // Check if running on mobile
                #elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

                if (Input.touchCount > 0)
                {
                    foreach (Touch fingerTouch in Input.touches)
                    {
                        if (fingerTouch.fingerId != joystickController.joystickTouchID)
                        {
                            if (EventSystem.current.IsPointerOverGameObject(fingerTouch.fingerId))
                            {
                                // empty
                            }
                            else if (!EventSystem.current.IsPointerOverGameObject(fingerTouch.fingerId))
                            {
                                if (fingerTouch.phase == TouchPhase.Began)
                                {
                                    cameraTouchID = fingerTouch.fingerId;
                                }
                            }

                            if (!EventSystem.current.IsPointerOverGameObject(cameraTouchID))
                            {
                                if (Input.touches[cameraTouchID].phase == TouchPhase.Moved)
                                {
                                    rotationVertical += Input.touches[cameraTouchID].deltaPosition.y * mouseSensitivityY * 0.1f;
                                }
                            }
                        }
                    }
                }

                #endif

                if (rotationVertical < -45.0f)
                {
                    rotationVertical = -45.0f;
                }
                if (rotationVertical > 45.0f)
                {
                    rotationVertical = 45.0f;
                }

                Quaternion quaternionVertical = Quaternion.AngleAxis(rotationVertical, Vector3.left);

                transform.localRotation = Quaternion.identity * quaternionVertical;
            }
        }
	}
}
