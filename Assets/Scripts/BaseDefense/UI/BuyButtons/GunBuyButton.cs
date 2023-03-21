using BaseDefense.BroadcastMessages;
using BaseDefense.BroadcastMessages.Messages.UpdateCurrencyMessages;
using BaseDefense.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BaseDefense.UI.BuyButtons {

    public class GunBuyButton : MonoBehaviour {

        [SerializeField]
        private TextMeshProUGUI priceView;
        
        [Inject]
        private Inventory m_inventory;

        private Button m_thisButton;
        private PurchasingGunWindow m_purchasingWindow;
        private GunSlot m_gunSlot;


        public void PurchaseGun () {
            m_inventory.PurchaseGun(m_gunSlot.gun.Id, m_gunSlot.price);
            m_purchasingWindow.Disable();
        }


        private void Awake () {
            m_thisButton = GetComponent<Button>();
            m_purchasingWindow = GetComponentInParent<PurchasingGunWindow>();
            m_gunSlot = m_purchasingWindow.GunSlot;
            Messenger.SubscribeTo<UpdateMoneysMessage>(Check);
        }


        private void Start () {
            priceView.text = "Buy " + m_gunSlot.price.ToStringWithSeparator();
        }


        private void OnDestroy () {
            Messenger.UnsubscribeFrom<UpdateMoneysMessage>(Check);
        }


        private void Check (UpdateMoneysMessage message) {
            m_thisButton.interactable = message.Value > m_gunSlot.price;
        }

    }

}