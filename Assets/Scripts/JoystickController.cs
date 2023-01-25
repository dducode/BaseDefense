using UnityEngine;
using UnityEngine.EventSystems;
using BroadcastMessages;

namespace BaseDefense
{
    public class JoystickController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] RectTransform joystick;
        [SerializeField] RectTransform circle;

        public Vector2 JoystickPosition { get; private set; }

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

        [Listener(MessageType.RESTART)]
        void EnableJoystick() => this.enabled = true;

        [Listener(MessageType.DEATH_PLAYER)]
        public void DisableJoystick()
        {
            this.enabled = false;
            circle.gameObject.SetActive(false);
            JoystickPosition = Vector3.zero;
        }

        public Vector3 GetInput()
        {
            Vector2 m = JoystickPosition;
            Vector3 move = new Vector3(m.x, 0, m.y);
            return move;
        }
    }
}


