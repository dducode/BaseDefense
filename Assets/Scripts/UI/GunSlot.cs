using UnityEngine;

namespace BaseDefense
{
    public class GunSlot : MonoBehaviour
    {
        [SerializeField] Gun gun;
        public string GunName => gun.name;
    }
}


