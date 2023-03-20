using System;
using BaseDefense.BroadcastMessages;
using BaseDefense.BroadcastMessages.Messages.UpdateCurrencyMessages;
using BaseDefense.Extensions;

namespace BaseDefense.UI.BuyButtons {

    public class GunBuyButton : BuyButton {
        
        private GunSlot m_gunSlot;

        public void PurchaseGun () => inventory.PurchaseGun(m_gunSlot.GunId, price);


        protected override void Awake () {
            base.Awake();
            m_gunSlot = GetComponentInParent<GunSlot>();
            Messenger.SubscribeTo<UpdateMoneysMessage>(Check);
        }


        private void Start () {
            priceView.text = "Buy " + price.ToStringWithSeparator();
        }


        private void OnDestroy () {
            Messenger.UnsubscribeFrom<UpdateMoneysMessage>(Check);
        }

    }

}