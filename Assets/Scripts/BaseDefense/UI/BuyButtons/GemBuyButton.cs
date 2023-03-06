using System;
using BaseDefense.BroadcastMessages;
using BaseDefense.Currencies;

namespace BaseDefense.UI.BuyButtons
{
    public class GemBuyButton : BuyButton
    {
        protected override void Awake()
        {
            base.Awake();
            Messenger<GemCurrency>.AddListener(MessageType.UPDATE_CURRENCY, Check);
        }

        private void OnDestroy()
        {
            Messenger<GemCurrency>.RemoveListener(MessageType.UPDATE_CURRENCY, Check);
        }
    }
}