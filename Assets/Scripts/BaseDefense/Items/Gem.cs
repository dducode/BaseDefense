using System.Collections;
using BroadcastMessages;
using DG.Tweening;
using UnityEngine;

namespace BaseDefense.Items
{
    public class Gem : Item
    {
        void OnEnable() => Messenger.AddListener(MessageType.PUSH_UNUSED_ITEMS, Remove);
        void OnDisable() => Messenger.RemoveListener(MessageType.PUSH_UNUSED_ITEMS, Remove);

        public override void Destroy()
        {
            DestroyGem();
        }

        public override void Drop(Vector3 force, Vector3 torque = default)
        {
            enabled = true;
            Rigidbody.AddForce(force, ForceMode.Impulse);
            Rigidbody.AddTorque(torque, ForceMode.Impulse);
        }

        private void DestroyGem()
        {
            enabled = false;
            var sequence = Collapse();
            sequence.OnComplete(() =>
            {
                ObjectsPool<Gem>.Push(this);
                transform.localScale = Vector3.one;
            });
        }

        // Удаляет неиспользованные кристаллы со сцены
        private void Remove()
        {
            enabled = false;
            ObjectsPool<Gem>.Push(this);
        }
    }
}


