using System.Collections;
using BaseDefense.BroadcastMessages;
using BaseDefense.BroadcastMessages.Messages;
using BaseDefense.BroadcastMessages.Messages.UpdateCurrencyMessages;
using UnityEngine;
using TMPro;
using BaseDefense.Extensions;

namespace BaseDefense.UI {

    public class DisplayingUI : MonoBehaviour {

        ///<summary>Отображает количество денег у игрока</summary>
        [Tooltip("Отображает количество денег у игрока")]
        [SerializeField]
        private TextMeshProUGUI moneys;

        ///<summary>Отображает количество кристаллов у игрока</summary>
        [Tooltip("Отображает количество кристаллов у игрока")]
        [SerializeField]
        private TextMeshProUGUI gems;

        ///<summary>Окно, выводимое при смерти игрока</summary>
        [Tooltip("Окно, выводимое при смерти игрока")]
        [SerializeField]
        private Canvas deathWindow;

        [SerializeField]
        private Canvas shopWindow;

        [SerializeField]
        private Canvas playerUpgradesWindow;


        public void Restart () {
            Messenger.SendMessage<RestartMessage>();
            deathWindow.enabled = false;
        }


        public void OpenShop () => shopWindow.enabled = true;
        public void CloseShop () => shopWindow.enabled = false;
        public void OpenUpgrades () => playerUpgradesWindow.enabled = true;
        public void CloseUpgrades () => playerUpgradesWindow.enabled = false;


        private void OnEnable () {
            Messenger.SubscribeTo<DeathPlayerMessage>(DisplayDeathWindow);
            Messenger.SubscribeTo<UpdateGemsMessage>(UpdateGems);
            Messenger.SubscribeTo<UpdateMoneysMessage>(UpdateMoneys);
        }


        private void OnDisable () {
            Messenger.UnsubscribeFrom<DeathPlayerMessage>(DisplayDeathWindow);
            Messenger.UnsubscribeFrom<UpdateGemsMessage>(UpdateGems);
            Messenger.UnsubscribeFrom<UpdateMoneysMessage>(UpdateMoneys);
        }


        private void Start () {
            deathWindow.enabled = false;
            shopWindow.enabled = false;
            playerUpgradesWindow.enabled = false;
        }


        private void UpdateMoneys (UpdateMoneysMessage message) =>
            moneys.text = message.Value.ToStringWithSeparator();


        private void UpdateGems (UpdateGemsMessage message) =>
            gems.text = message.Value.ToStringWithSeparator();


        private void DisplayDeathWindow () => StartCoroutine(Await());


        private IEnumerator Await () {
            const float awaitTime = 2;

            yield return new WaitForSeconds(awaitTime);
            deathWindow.enabled = true;
            var canvasGroup = deathWindow.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            const float speed = 2f;

            while (canvasGroup.alpha < 1) {
                canvasGroup.alpha += Time.smoothDeltaTime * speed;
                yield return null;
            }
        }

    }

}