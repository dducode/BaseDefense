using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoystickController : MonoBehaviour
{
    [SerializeField] RectTransform joystick;
    [SerializeField] RectTransform circle;

    void Start() => circle.gameObject.SetActive(false);
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
        Vector3 movement = new Vector3();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        circle.transform as RectTransform,
        new Vector2(Input.mousePosition.x, Input.mousePosition.y),
        null,
        out joystickPos
       );
        joystickPos = Vector2.ClampMagnitude(joystickPos, size.x / 2);
        joystick.localPosition = joystickPos;

        movement = joystickPos;
        movement = Vector3.Normalize(movement);

        PlayerCharacter.getInstance.Movement(
            joystickPos.magnitude / (size.x / 2), 
            new Vector3(movement.x, 0, movement.y)
        );
    }
    public void EndTouch()
    {
        circle.gameObject.SetActive(false);
        joystick.localPosition = Vector2.zero;
        PlayerCharacter.getInstance.Movement(0, Vector3.zero);
    }
}
