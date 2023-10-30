using System.Collections;
using BaseDefense.Extensions;
using BaseDefense.Messages;
using BaseDefense.Messages.UpdateCurrencyMessages;
using BroadcastMessages;
using DG.Tweening;
using TMPro;
using UnityEngine;

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
        private GunSelectWindow gunSelectWindow;

        [SerializeField]
        private PlayerUpgradesWindow playerUpgradesWindow;

        private IEnumerator m_fadingWindow;


        public void Restart () {
            Messenger.SendMessage<RestartMessage>();
            deathWindow.enabled = false;
        }


        private const float ANIMATION_DURATION = 0.2f;
        private readonly Vector3 m_smallCanvasScale = new(0.95f, 0.95f, 1);


        public void OpenShop () {
            gunSelectWindow.Canvas.enabled = true;
            if (m_fadingWindow is not null)
                StopCoroutine(m_fadingWindow);
            m_fadingWindow = FadingWindow(gunSelectWindow.CanvasGroup, 1, ANIMATION_DURATION);
            StartCoroutine(m_fadingWindow);
            gunSelectWindow.Canvas.transform
                .DOScale(Vector3.one, ANIMATION_DURATION)
                .SetEase(Ease.Linear);
        }


        public void CloseShop () {
            if (m_fadingWindow is not null)
                StopCoroutine(m_fadingWindow);
            m_fadingWindow = FadingWindow(gunSelectWindow.CanvasGroup, 0, ANIMATION_DURATION);
            StartCoroutine(m_fadingWindow);
            gunSelectWindow.Canvas.transform
                .DOScale(m_smallCanvasScale, ANIMATION_DURATION)
                .SetEase(Ease.Linear)
                .OnComplete(() => gunSelectWindow.Canvas.enabled = false);
        }


        public void OpenUpgrades () {
            playerUpgradesWindow.Canvas.enabled = true;
            if (m_fadingWindow is not null)
                StopCoroutine(m_fadingWindow);
            m_fadingWindow = FadingWindow(playerUpgradesWindow.CanvasGroup, 1, ANIMATION_DURATION);
            StartCoroutine(m_fadingWindow);
            playerUpgradesWindow.Canvas.transform
                .DOScale(Vector3.one, ANIMATION_DURATION)
                .SetEase(Ease.Linear);
        }


        public void CloseUpgrades () {
            if (m_fadingWindow is not null)
                StopCoroutine(m_fadingWindow);
            m_fadingWindow = FadingWindow(playerUpgradesWindow.CanvasGroup, 0, ANIMATION_DURATION);
            StartCoroutine(m_fadingWindow);
            playerUpgradesWindow.Canvas.transform
                .DOScale(m_smallCanvasScale, ANIMATION_DURATION)
                .SetEase(Ease.Linear)
                .OnComplete(() => playerUpgradesWindow.Canvas.enabled = false);
        }


        private IEnumerator FadingWindow (CanvasGroup target, float endValue, float duration) {
            const float tolerance = 0.02f;
            var alpha = target.alpha;
            var difference = Mathf.Abs(alpha - endValue);
            var scalar = endValue - alpha;

            while (difference > tolerance) {
                alpha += Time.smoothDeltaTime * scalar / duration;
                difference = Mathf.Abs(alpha - endValue);
                target.alpha = alpha;
                yield return null;
            }
        }


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
            playerUpgradesWindow.Canvas.transform.localScale = m_smallCanvasScale;
            gunSelectWindow.Canvas.transform.localScale = m_smallCanvasScale;
        }


        private void UpdateMoneys (UpdateMoneysMessage message) {
            moneys.text = message.Value.ToStringWithSeparator();
        }


        private void UpdateGems (UpdateGemsMessage message) {
            gems.text = message.Value.ToStringWithSeparator();
        }


        private void DisplayDeathWindow () {
            StartCoroutine(Await());
        }


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