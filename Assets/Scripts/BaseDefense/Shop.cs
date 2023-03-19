using UnityEngine;
using BaseDefense.AttackImplemention.Guns;

namespace BaseDefense {

    public sealed class Shop : MonoBehaviour {

        ///<summary>Хранит всё оружие, имеющееся в магазине</summary>
        [Tooltip("Хранит всё оружие, имеющееся в магазине")]
        [SerializeField]
        private Gun[] guns;


        /// <summary>Вызывается для взятия оружия из магазина</summary>
        /// <param name="gunName">Оружие, которое необходимо забрать из магазина</param>
        /// <param name="playerGun">Оружие игрока</param>
        /// <returns>
        /// Если в магазине нет запрашиваемого оружия - возвращается оружие игрока
        /// </returns>
        public Gun TakeGun (string gunName, Gun playerGun) {
            if (playerGun.name == gunName)
                return playerGun;

            for (var i = 0; i < guns.Length; i++)
                if (guns[i].name == gunName) {
                    var gun = guns[i];
                    playerGun.transform.parent = transform;
                    guns[i] = playerGun;

                    return gun;
                }

            Debug.LogError($"Оружие {gunName} не найдено");

            return playerGun;
        }


        private void Start () {
            if (guns.Length == 0)
                Debug.LogWarning("В магазине нет оружия");
        }

    }

}