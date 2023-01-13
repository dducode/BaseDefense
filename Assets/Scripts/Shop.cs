using UnityEngine;
using Zenject;

public class Shop : MonoBehaviour
{
    ///<summary>Хранит всё оружие, имеющееся в магазине</summary>
    [Tooltip("Хранит всё оружие, имеющееся в магазине")]
    [SerializeField] Gun[] guns;
    [Inject] DisplayingUI UI;

    void Start()
    {
        if (guns.Length == 0)
        {
            Debug.LogWarning("No guns installed in shop");
            return;
        }
    }

    ///<summary>Вызывается для взятия оружия из магазина</summary>
    ///<param name="gunName">Оружие, которое необходимо забрать из магазина</param>
    ///<returns>
    ///Если в магазине нет запрашиваемого оружия - возвращается оружие игрока
    ///</returns>
    public Gun TakeGun(GunName gunName, Gun playerGun)
    {
        if (playerGun.GunName == gunName)
            return playerGun;
            
        for (int i = 0; i < guns.Length; i++)
            if (guns[i].GunName == gunName)
            {
                Gun gun = guns[i];
                playerGun.transform.parent = transform;
                guns[i] = playerGun;
                return gun;
            }
        Debug.LogError($"Not found gun of type {gunName}");
        return playerGun;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            UI.OpenShop();
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            UI.CloseShop();
    }
}
