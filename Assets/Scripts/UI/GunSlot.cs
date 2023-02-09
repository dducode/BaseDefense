using UnityEngine;
using BaseDefense.AttackImplemention.Guns;

namespace BaseDefense.UI
{
    public class GunSlot : MonoBehaviour
    {
        [SerializeField] Gun gun;
        public string GunName => gun.name;
    }
}


