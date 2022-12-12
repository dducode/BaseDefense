using UnityEngine;

public class Shop : MonoBehaviour
{
    Gun[] guns;

    void Start()
    {
        if (transform.childCount == 0)
        {
            Debug.LogWarning("No guns installed in shop");
            return;
        }
        int childCount = transform.childCount;
        guns = new Gun[childCount];
        for (int i = 0; i < childCount; i++)
            guns[i] = transform.GetChild(i).GetComponent<Gun>();
    }

    public Gun Select(GunSlot slot, Gun playerGun)
    {
        for (int i = 0; i < guns.Length; i++)
            if (guns[i].GunType == slot.GunType)
            {
                Gun gun = guns[i];
                playerGun.transform.parent = transform;
                guns[i] = playerGun;
                return gun;
            }
        Debug.LogError($"Not found gun of type {slot.GunType}");
        return playerGun;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Game.UI.OpenShop();
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            Game.UI.CloseShop();
    }
}
