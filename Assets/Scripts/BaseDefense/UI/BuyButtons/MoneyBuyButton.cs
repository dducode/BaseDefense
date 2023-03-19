using System;
using BaseDefense.BroadcastMessages;
using BaseDefense.BroadcastMessages.Messages.UpdateCurrencyMessages;
using BaseDefense.Extensions;

namespace BaseDefense.UI.BuyButtons {

    public class MoneyBuyButton : BuyButton {

        protected override void Awake () {
            base.Awake();
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