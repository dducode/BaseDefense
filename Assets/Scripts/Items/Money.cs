using System;
using BaseDefense.Messages;
using BroadcastMessages;
using DG.Tweening;
using UnityEngine;

namespace BaseDefense.Items {

    public class Money : Item {

        ///<summary>Время, необходимое для проигрывания анимации сброса предмета на базу</summary>
        ///<value>[0, infinity]</value>
        [Tooltip("Время, необходимое для проигрывания анимации сброса предмета на базу. [0, infinity]")]
        [SerializeField, Min(0)]
        private float collectionTime = 3;


        public override void Drop (Vector3 force, Vector3 torque = default) {
            transform.localScale = Vector3.one;
            Enabled = true;
            rb.AddForce(force, ForceMode.Impulse);
            rb.AddTorque(torque, ForceMode.Impulse);
        }


        public override void DestroyItem () {
            DestroyMoney();
        }


        private void OnEnable () => Messenger.SubscribeTo<DestroyUnusedItemsMessage>(Remove);


        private void DestroyMoney () {
            // Заранее делаем отписку для того, чтобы анимация сброса денег не прерывалась
            Messenger.UnsubscribeFrom<DestroyUnusedItemsMessage>(Remove);
            trigger.enabled = false;
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(collectionTime);
            sequence.AppendCallback(() => Enabled = false);
            sequence.Append(Collapse());
            Destroy(sequence);
        }


        // Удаляет неиспользованные деньги со сцены
        private void Remove () {
            Enabled = false;
            Destroy();
        }



        [Serializable]
        private struct MoneyData {

            public Transform parent;

        }

    }

}