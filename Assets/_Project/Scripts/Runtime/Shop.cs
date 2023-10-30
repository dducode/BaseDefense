using BaseDefense.AttackImplemention.Guns;
using UnityEngine;

namespace BaseDefense {

    public sealed class Shop : MonoBehaviour {

        ///<summary>Хранит всё оружие, имеющееся в магазине</summary>
        [Tooltip("Хранит всё оружие, имеющееся в магазине")]
        [SerializeField]
        private Gun[] guns;


        /// <summary>Вызывается для взятия оружия из магазина</summary>
        /// <param name="gunId">Оружие, которое необходимо забрать из магазина</param>
        /// <param name="playerGun">Оружие игрока</param>
        /// <returns>
        /// Если в магазине нет запрашиваемого оружия - возвращается оружие игрока
        /// </returns>
        public Gun TakeGun (int gunId, Gun playerGun) {
            if (playerGun.Id == gunId)
                return playerGun;

            for (var i = 0; i < guns.Length; i++)
                if (guns[i].Id == gunId) {
                    var gun = guns[i];
                    playerGun.transform.parent = transform;
                    guns[i] = playerGun;

                    return gun;
                }

            Debug.LogError($"Оружие {gunId} не найдено");

            return playerGun;
        }


        private void Start () {
            if (guns.Length == 0)
                Debug.LogWarning("В магазине нет оружия");
        }

    }

}