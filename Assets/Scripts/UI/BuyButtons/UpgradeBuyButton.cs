using BaseDefense.BroadcastMessages;
using BaseDefense.BroadcastMessages.Messages.UpdateCurrencyMessages;
using BaseDefense.Characters;
using BaseDefense.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BaseDefense.UI.BuyButtons {

    public class UpgradeBuyButton : MonoBehaviour {

        [SerializeField]
        private TextMeshProUGUI headerView;

        [SerializeField]
        private TextMeshProUGUI priceView;

        [SerializeField]
        private UpgradableProperty property;

        private PlayerCharacter m_player;
        private Inventory m_inventory;
        private Button m_thisButton;
        private int m_price;


        [Inject]
        public void Construct (PlayerCharacter player, Inventory inventory) {
            m_player = player;
            m_inventory = inventory;
        }


        public void UpgradeProperty () {
            property.SetNextStep();
            m_player.Upgrade(property);

            if (property.TryGetNextStep(out var step))
                m_price = step.price;
            else {
                gameObject.SetActive(false);
                Messenger.UnsubscribeFrom<UpdateGemsMessage>(Check);
            }

            m_inventory.PurchaseUpgrade(property.CurrentStep.price);
            UpdateView();
        }


        private void Awake () {
            m_thisButton = GetComponent<Button>();
            Messenger.SubscribeTo<UpdateGemsMessage>(Check);
        }


        private void Start () {
            if (property.TryGetNextStep(out var step)) {
                m_price = step.price;
            }
            else {
                gameObject.SetActive(false);
                Messenger.UnsubscribeFrom<UpdateGemsMessage>(Check);
            }

            UpdateView();
        }


        private void UpdateView () {
            priceView.text = "Upgrade " + m_price.ToStringWithSeparator();
            headerView.text = $"{property.viewName}: {property.CurrentStep.value}";
        }


        private void OnDestroy () {
            if (gameObject.activeSelf)
                Messenger.UnsubscribeFrom<UpdateGemsMessage>(Check);
        }


        private void Check (UpdateCurrencyMessage message) {
            m_thisButton.interactable = message.Value > m_price;
        }

    }

}