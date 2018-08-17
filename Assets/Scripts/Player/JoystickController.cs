using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler{

    // The joystick area and joystick itself
    private Image joystickBase;
    private Image joystickBody;

    // The direction detected from input
    public Vector3 detectedInputDirection;

    public int joystickTouchID = 100;
    
    // Set alpha hit test and initialise variables
    void Start()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 1;

        joystickBase = GetComponent<Image>();

        joystickBody = transform.GetChild(0).GetComponent<Image>();

        detectedInputDirection = Vector3.zero;
    }

    // Move joystick based on area of prolonged input
    public void OnDrag(PointerEventData ped)
    {
        Vector2 position = Vector2.zero;

        // Get the position of input relative to the joystick area
        RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBase.rectTransform, ped.position, ped.pressEventCamera, out position);

        // Set the joystick position relative to the size of the joystick area
        position.x = (position.x / joystickBase.rectTransform.sizeDelta.x);
        position.y = (position.y / joystickBase.rectTransform.sizeDelta.y);

        // From the position of the joystick, determine movement values
        float x = (joystickBase.rectTransform.pivot.x == 1f) ? position.x * 2 + 1 : position.x * 2 - 1;
        float y = (joystickBase.rectTransform.pivot.y == 1f) ? position.y * 2 + 1 : position.y * 2 - 1;

        // Set the value to public variable and manipulate it
        detectedInputDirection = new Vector3(x, y, 0);
        detectedInputDirection = (detectedInputDirection.magnitude > 1) ? detectedInputDirection.normalized : detectedInputDirection;

        // Restrict joystick to the area
        joystickBody.rectTransform.anchoredPosition = new Vector3(detectedInputDirection.x * (joystickBase.rectTransform.sizeDelta.x / 2.5f),
            detectedInputDirection.y * (joystickBase.rectTransform.sizeDelta.y) / 2.5f);
    }

    // Call drag on input
    public void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);

        joystickTouchID = ped.pointerId;

        print(joystickTouchID);
    }

    // Reset joystick on release
    public void OnPointerUp(PointerEventData ped)
    {
        detectedInputDirection = Vector3.zero;

        joystickBody.rectTransform.anchoredPosition = Vector3.zero;

        joystickTouchID = 100;

        print(joystickTouchID);
    }
}