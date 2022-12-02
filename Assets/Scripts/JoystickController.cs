using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoystickController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] RectTransform joystick;
    [SerializeField] RectTransform circle;

    public Vector2 JoystickPosition { get; private set; }

    void Awake()
    {
        BroadcastMessages.AddListener(MessageType.DEATH_PLAYER, DisableJoystick);
        BroadcastMessages.AddListener(MessageType.RESTART, EnableJoystick);
    }
    void OnDestroy()
    {
        BroadcastMessages.RemoveListener(MessageType.DEATH_PLAYER, DisableJoystick);
        BroadcastMessages.RemoveListener(MessageType.RESTART, EnableJoystick);
    }

    void Start()
    {
        circle.gameObject.SetActive(false);
        JoystickPosition = Vector2.zero;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        circle.gameObject.SetActive(true);
        circle.position = eventData.position;
    }
    public void OnDrag(PointerEventData eventData)
    {
        var size = circle.rect.size;
        JoystickPosition = circle.InverseTransformPoint(eventData.position);
        JoystickPosition = Vector2.ClampMagnitude(JoystickPosition, size.x / 2);
        joystick.localPosition = JoystickPosition;
        JoystickPosition = JoystickPosition.normalized * (JoystickPosition.magnitude / (size.x / 2));
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        circle.gameObject.SetActive(false);
        joystick.localPosition = Vector2.zero;
        JoystickPosition = Vector2.zero;
    }

    void EnableJoystick() => this.enabled = true;
    void DisableJoystick()
    {
        this.enabled = false;
        circle.gameObject.SetActive(false);
    }
}
