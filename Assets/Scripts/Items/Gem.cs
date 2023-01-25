using System.Collections;
using BroadcastMessages;
using UnityEngine;

namespace BaseDefense
{
    public class Gem : Item
    {
        void OnEnable() => Messenger.AddListener(MessageType.PUSH_UNUSED_ITEMS, Remove);
        void OnDisable() => Messenger.RemoveListener(MessageType.PUSH_UNUSED_ITEMS, Remove);

        public override void Destroy()
        {
            StartCoroutine(DestroyGem());
        }

        public override void Drop(Vector3 force, Vector3 torque = default)
        {
            enabled = true;
            rb.AddForce(force, ForceMode.Impulse);
            rb.AddTorque(torque, ForceMode.Impulse);
        }

        IEnumerator DestroyGem()
        {
            enabled = false;
            yield return Collapse();
            ObjectsPool<Gem>.Push(this);
            transform.localScale = Vector3.one;
        }

        // Удаляет неиспользованные кристаллы со сцены
        void Remove()
        {
            enabled = false;
            ObjectsPool<Gem>.Push(this);
        }
    }
}


