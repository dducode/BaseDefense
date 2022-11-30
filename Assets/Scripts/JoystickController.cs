using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoystickController : MonoBehaviour
{
    [SerializeField] RectTransform joystick;
    [SerializeField] RectTransform circle;

    public Vector2 JoystickPosition { get; private set; }

    void Start()
    {
        circle.gameObject.SetActive(false);
        JoystickPosition = Vector2.zero;
    }
    void OnDisable() => circle.gameObject.SetActive(false);

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) BeginTouch();
        if (Input.GetMouseButton(0)) Touch();
        if (Input.GetMouseButtonUp(0)) EndTouch();
    }
#elif UNITY_ANDROID
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) BeginTouch();
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) Touch();
            if (touch.phase == TouchPhase.Ended) EndTouch();
        }
    }
#endif

    public void BeginTouch()
    {
        circle.gameObject.SetActive(true);
        circle.position = Input.mousePosition;
    }
    public void Touch()
    {
        var size = circle.rect.size;
        Vector2 joystickPos = new Vector2();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            circle.transform as RectTransform,
            new Vector2(Input.mousePosition.x, Input.mousePosition.y),
            null,
            out joystickPos
        );
        JoystickPosition = Vector2.ClampMagnitude(joystickPos, size.x / 2);
        joystick.localPosition = JoystickPosition;
        JoystickPosition = JoystickPosition.normalized * (JoystickPosition.magnitude / (size.x / 2));
    }
    public void EndTouch()
    {
        circle.gameObject.SetActive(false);
        joystick.localPosition = Vector2.zero;
        JoystickPosition = Vector2.zero;
    }
}
