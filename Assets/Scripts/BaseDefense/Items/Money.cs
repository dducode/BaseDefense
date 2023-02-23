using System.Collections;
using BaseDefense.Broadcast_messages;
using BroadcastMessages;
using DG.Tweening;
using UnityEngine;

namespace BaseDefense.Items
{
    public class Money : Item
    {
        ///<summary>Время, необходимое для проигрывания анимации сброса предмета на базу</summary>
        ///<value>[0, infinity]</value>
        [Tooltip("Время, необходимое для проигрывания анимации сброса предмета на базу. [0, infinity]")]
        [SerializeField, Min(0)] float collectionTime = 3;

        public override void DestroyItem()
        {
            DestroyMoney();
        }

        public override void Drop(Vector3 force, Vector3 torque = default)
        {
            transform.localScale = Vector3.one;
            enabled = true;
            Rigidbody.AddForce(force, ForceMode.Impulse);
            Rigidbody.AddTorque(torque, ForceMode.Impulse);
        }
        
        private void OnEnable() => Messenger.AddListener(MessageType.PUSH_UNUSED_ITEMS, Remove);
        private void OnDisable() => Messenger.RemoveListener(MessageType.PUSH_UNUSED_ITEMS, Remove);

        private void DestroyMoney()
        {
            // Заранее делаем отписку для того, чтобы анимация сброса денег не прерывалась
            Messenger.RemoveListener(MessageType.PUSH_UNUSED_ITEMS, Remove);
            trigger.enabled = false;
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(collectionTime);
            sequence.AppendCallback(() => enabled = false);
            sequence.Append(Collapse());
            Destroy(sequence);
        }

        // Удаляет неиспользованные деньги со сцены
        private void Remove()
        {
            enabled = false;
            Destroy();
        }
    }
}


