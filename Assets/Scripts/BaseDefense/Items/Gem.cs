using System.Collections;
using BaseDefense.Broadcast_messages;
using BroadcastMessages;
using DG.Tweening;
using UnityEngine;

namespace BaseDefense.Items
{
    public class Gem : Item
    {
        public override void DestroyItem()
        {
            DestroyGem();
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

        private void DestroyGem()
        {
            enabled = false;
            Destroy(Collapse());
        }

        // Удаляет неиспользованные кристаллы со сцены
        private void Remove()
        {
            enabled = false;
            Destroy();
        }
    }
}


