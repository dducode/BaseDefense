using UnityEngine;

public class GunSlot : MonoBehaviour
{
    [SerializeField] Gun gunPrefab;

    public GunType GunType => gunPrefab.GunType;
}
