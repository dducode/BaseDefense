using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] RectTransform frame;
    Canvas canvas;
    GunType activeGunType;

    public GunType ActiveGunType => activeGunType;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }

    public void Select(GunSlot slot)
    {
        frame.localPosition = slot.transform.localPosition;
        activeGunType = slot.GunType;
    }
    public void Open() => canvas.enabled = true;
    public void Close() => canvas.enabled = false;
}
