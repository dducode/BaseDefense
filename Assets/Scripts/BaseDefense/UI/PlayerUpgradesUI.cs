using UnityEngine;

namespace BaseDefense.UI {

    public class PlayerUpgradesUI : MonoBehaviour {

        [SerializeField]
        private UpgradableProperties upgradableProperty;

        public UpgradableProperties UpgradableProperty => upgradableProperty;

    }

}