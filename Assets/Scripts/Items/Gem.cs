using BaseDefense.BroadcastMessages;
using BaseDefense.BroadcastMessages.Messages;
using UnityEngine;

namespace BaseDefense.Items {

    public class Gem : Item {

        public override void DestroyItem () {
            Enabled = false;
            Destroy(Collapse());
        }


        public override void Drop (Vector3 force, Vector3 torque = default) {
            transform.localScale = Vector3.one;
            Enabled = true;
            rb.AddForce(force, ForceMode.Impulse);
            rb.AddTorque(torque, ForceMode.Impulse);
        }


        private void OnEnable () => Messenger.SubscribeTo<DestroyUnusedItemsMessage>(Remove);
        private void OnDisable () => Messenger.UnsubscribeFrom<DestroyUnusedItemsMessage>(Remove);


        // Удаляет неиспользованные кристаллы со сцены
        private void Remove () {
            Enabled = false;
            Destroy();
        }

    }

}