using BaseDefense.BroadcastMessages;
using BaseDefense.Currencies;

namespace BaseDefense.UI.BuyButtons
{
    public class MoneyBuyButton : BuyButton
    {
        protected override void Awake()
        {
            base.Awake();
            Messenger<MoneyCurrency>.AddListener(MessageType.UPDATE_CURRENCY, Check);
        }

        private void OnDestroy()
        {
            Messenger<MoneyCurrency>.RemoveListener(MessageType.UPDATE_CURRENCY, Check);
        }
    }
}