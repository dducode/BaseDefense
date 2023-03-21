using UnityEngine;
using UnityEngine.Serialization;

namespace BaseDefense.UI {

    public class PlayerUpgradesUI : MonoBehaviour {

        [FormerlySerializedAs("upgradableProperty")]
        [SerializeField]
        private UpgradablePropertyType upgradablePropertyType;

        public UpgradablePropertyType UpgradablePropertyType => upgradablePropertyType;

    }

}