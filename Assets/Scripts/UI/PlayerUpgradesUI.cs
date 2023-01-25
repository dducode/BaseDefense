using UnityEngine;

namespace BaseDefense
{
    public class PlayerUpgradesUI : MonoBehaviour
    {
        [SerializeField] UpgradableProperties upgradableProperty;
        public UpgradableProperties UpgradableProperty => upgradableProperty;
    }
}


