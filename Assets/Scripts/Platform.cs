using UnityEngine;
using Zenject;
using BaseDefense.UI;

namespace BaseDefense
{
    public class Platform : MonoBehaviour
    {
        [SerializeField] PlatformType platformType;
        [Inject] DisplayingUI UI;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                switch (platformType)
                {
                    case PlatformType.Gun_Shop:
                        UI.OpenShop();
                        break;
                    case PlatformType.Player_Upgrades:
                        UI.OpenUpgrades();
                        break;
                    default:
                        Debug.LogError($"{platformType} is not implemented");
                        break;
                }
        }
        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                switch (platformType)
                {
                    case PlatformType.Gun_Shop:
                        UI.CloseShop();
                        break;
                    case PlatformType.Player_Upgrades:
                        UI.CloseUpgrades();
                        break;
                    default:
                        Debug.LogError($"{platformType} is not implemented");
                        break;
                }
        }

        public enum PlatformType
        {
            Gun_Shop, Player_Upgrades
        }
    }
}


