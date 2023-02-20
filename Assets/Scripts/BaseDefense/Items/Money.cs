using System.Collections;
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

        void OnEnable() => Messenger.AddListener(MessageType.PUSH_UNUSED_ITEMS, Remove);
        void OnDisable() => Messenger.RemoveListener(MessageType.PUSH_UNUSED_ITEMS, Remove);

        public override void Destroy()
        {
            DestroyMoney();
        }

        public override void Drop(Vector3 force, Vector3 torque = default)
        {
            enabled = true;
            Rigidbody.AddForce(force, ForceMode.Impulse);
            Rigidbody.AddTorque(torque, ForceMode.Impulse);
        }

        private void DestroyMoney()
        {
            // Заранее делаем отписку для того, чтобы анимация сброса денег не прерывалась
            Messenger.RemoveListener(MessageType.PUSH_UNUSED_ITEMS, Remove);
            trigger.enabled = false;
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(collectionTime);
            sequence.AppendCallback(() => enabled = false);
            sequence.Append(Collapse());
            sequence.OnComplete(() =>
            {
                ObjectsPool<Money>.Push(this);
                transform.localScale = Vector3.one;
            });
        }

        // Удаляет неиспользованные деньги со сцены
        private void Remove()
        {
            enabled = false;
            ObjectsPool<Money>.Push(this);
        }
    }
}


