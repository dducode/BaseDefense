using UnityEngine;

namespace BaseDefense.UI
{
    public class PlayerUpgradesUI : MonoBehaviour
    {
        [SerializeField] UpgradableProperties upgradableProperty;
        public UpgradableProperties UpgradableProperty => upgradableProperty;
    }
}


