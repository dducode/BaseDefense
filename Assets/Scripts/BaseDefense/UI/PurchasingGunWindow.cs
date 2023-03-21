using BaseDefense.BroadcastMessages;
using BaseDefense.BroadcastMessages.Messages;
using UnityEngine;

namespace BaseDefense.UI {

    public class PurchasingGunWindow : MonoBehaviour {

        [SerializeField]
        private GunSlot gunSlot;

        public GunSlot GunSlot => gunSlot;

        private GunSelectButton m_gunSelectButton;


        public void Disable () {
            m_gunSelectButton.EnableButton();
            gameObject.SetActive(false);
        }


        private void Awake () {
            m_gunSelectButton = GetComponentInParent<GunSelectButton>();
            Messenger.SubscribeTo<UnlockedGunsMessage>(OnUnlockGun);
        }


        private void Start () {
            m_gunSelectButton.DisableButton();
        }


        private void OnUnlockGun (UnlockedGunsMessage message) {
            foreach (var unlockedGunId in message.unlockedGuns) {
                if (unlockedGunId == gunSlot.gun.Id) {
                    Disable();
                    Messenger.UnsubscribeFrom<UnlockedGunsMessage>(OnUnlockGun);
                    return;
                }
            }
        }

    }

}