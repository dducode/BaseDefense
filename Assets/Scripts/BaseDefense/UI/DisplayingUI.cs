using System.Collections;
using BaseDefense.BroadcastMessages;
using UnityEngine;
using TMPro;
using BaseDefense.Currencies;
using BaseDefense.Extensions;

namespace BaseDefense.UI
{
    public class DisplayingUI : MonoBehaviour
    {
        ///<summary>Отображает количество денег у игрока</summary>
        [SerializeField, Tooltip("Отображает количество денег у игрока")]
        private TextMeshProUGUI moneys;

        ///<summary>Отображает количество кристаллов у игрока</summary>
        [SerializeField, Tooltip("Отображает количество кристаллов у игрока")]
        private TextMeshProUGUI gems;

        ///<summary>Окно, выводимое при смерти игрока</summary>
        [SerializeField, Tooltip("Окно, выводимое при смерти игрока")]
        private Canvas deathWindow;

        [SerializeField] private Canvas shopWindow;
        [SerializeField] private Canvas playerUpgradesWindow;

        public void Restart()
        {
            Messenger.SendMessage(MessageType.RESTART);
            deathWindow.enabled = false;
        }

        public void OpenShop() => shopWindow.enabled = true;
        public void CloseShop() => shopWindow.enabled = false;
        public void OpenUpgrades() => playerUpgradesWindow.enabled = true;
        public void CloseUpgrades() => playerUpgradesWindow.enabled = false;

        private void OnEnable()
        {
            Messenger.AddListener(MessageType.DEATH_PLAYER, DisplayDeathWindow);
            Messenger<GemCurrency>.AddListener(MessageType.UPDATE_CURRENCY, UpdateGems);
            Messenger<MoneyCurrency>.AddListener(MessageType.UPDATE_CURRENCY, UpdateMoneys);
        }

        private void OnDisable()
        {
            Messenger.RemoveListener(MessageType.DEATH_PLAYER, DisplayDeathWindow);
            Messenger<GemCurrency>.RemoveListener(MessageType.UPDATE_CURRENCY, UpdateGems);
            Messenger<MoneyCurrency>.RemoveListener(MessageType.UPDATE_CURRENCY, UpdateMoneys);
        }

        private void Start()
        {
            deathWindow.enabled = false;
            shopWindow.enabled = false;
            playerUpgradesWindow.enabled = false;
        }

        private void DisplayDeathWindow() => StartCoroutine(Await());
        private void UpdateMoneys(Currency currency) => gems.text = currency.Value.ToStringWithSeparator();
        private void UpdateGems(Currency currency) => moneys.text = currency.Value.ToStringWithSeparator();
        private IEnumerator Await()
        {
            const float awaitTime = 2;
            yield return new WaitForSeconds(awaitTime);
            deathWindow.enabled = true;
            var canvasGroup = deathWindow.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            const float speed = 2f;
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.smoothDeltaTime * speed;
                yield return null;
            }
        }
    }
}



