using BaseDefense.AttackImplemention.Guns;
using UnityEngine;

namespace BaseDefense {

    [CreateAssetMenu(fileName = "Gun Slot", menuName = "Gun Slot", order = 51)]
    public class GunSlot : ScriptableObject {

        public Gun gun;
        public int price;

    }

}