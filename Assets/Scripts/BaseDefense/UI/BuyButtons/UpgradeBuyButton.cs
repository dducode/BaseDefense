using BaseDefense.BroadcastMessages;
using BaseDefense.BroadcastMessages.Messages.UpdateCurrencyMessages;
using BaseDefense.Characters;
using BaseDefense.Extensions;
using TMPro;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BaseDefense.UI.BuyButtons {

    public class UpgradeBuyButton : MonoBehaviour {

        [SerializeField]
        private TextMeshProUGUI priceView;

        [SerializeField]
        private UpgradableProperty property;

        [Inject]
        private PlayerCharacter m_player;

        private Button m_thisButton;

        private int m_price;


        public void UpgradeProperty () {
            property.SetNextStep();
            m_player.Upgrade(property);
            m_price = property.GetNextStep().price;
            UpdatePrice();
        }


        private void Awake () {
            m_thisButton = GetComponent<Button>();
            m_price = property.GetNextStep().price;
            Messenger.SubscribeTo<UpdateGemsMessage>(Check);
        }


        private void Start () {
            UpdatePrice();
        }


        private void UpdatePrice () {
            priceView.text = "Upgrade " + m_price.ToStringWithSeparator();
        }


        private void OnDestroy () {
            Messenger.UnsubscribeFrom<UpdateGemsMessage>(Check);
        }


        private void Check (UpdateCurrencyMessage message) {
            m_thisButton.interactable = message.Value > m_price;
        }

    }

}