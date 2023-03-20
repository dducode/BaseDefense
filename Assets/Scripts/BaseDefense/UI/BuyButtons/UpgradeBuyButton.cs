using BaseDefense.BroadcastMessages;
using BaseDefense.BroadcastMessages.Messages.UpdateCurrencyMessages;
using BaseDefense.Extensions;

namespace BaseDefense.UI.BuyButtons {

    public class UpgradeBuyButton : BuyButton {

        protected override void Awake () {
            base.Awake();
            Messenger.SubscribeTo<UpdateGemsMessage>(Check);
        }


        private void Start () {
            priceView.text = "Upgrade " + price.ToStringWithSeparator();
        }


        private void OnDestroy () {
            Messenger.UnsubscribeFrom<UpdateGemsMessage>(Check);
        }

    }

}