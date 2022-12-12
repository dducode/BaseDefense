using UnityEngine;

public class GunSlot : MonoBehaviour
{
    [SerializeField] GunType gunType;

    public GunType GunType => gunType;
}
