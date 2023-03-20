using BaseDefense.BroadcastMessages;
using BaseDefense.BroadcastMessages.Messages;
using UnityEngine;

namespace BaseDefense.UI {

    public class PurchasingGunWindow : MonoBehaviour {

        private GunSlot m_gunSlot;


        private void Awake () {
            m_gunSlot = GetComponentInParent<GunSlot>();
            Messenger.SubscribeTo<UnlockedGunsMessage>(Check);
        }


        private void Start () => m_gunSlot.DisableButton();


        private void Check (UnlockedGunsMessage message) {
            foreach (var unlockedGunId in message.unlockedGuns)
                if (unlockedGunId == m_gunSlot.GunId) {
                    m_gunSlot.EnableButton();
                    gameObject.SetActive(false);
                    Messenger.UnsubscribeFrom<UnlockedGunsMessage>(Check);
                    return;
                }
        }

    }

}