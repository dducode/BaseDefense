using System.Collections.Generic;
using UnityEngine;

namespace BaseDefense {

    public sealed class Ragdoll : MonoBehaviour {

        private Collider[] m_colliders;
        private Rigidbody[] m_rigidbodies;
        private Rigidbody m_mainRigidbody;

        private bool m_enabled;

        public bool Enabled {
            get => m_enabled;
            set {
                m_enabled = value;
                enabled = m_enabled;
                foreach (var ragdollPart in m_colliders)
                    ragdollPart.enabled = m_enabled;
                foreach (var rb in m_rigidbodies)
                    rb.isKinematic = !m_enabled;
            }
        }


        private void Awake () {
            var colliders = new List<Collider>(GetComponentsInChildren<Collider>());

            foreach (var ragdollPart in colliders)
                if (ragdollPart is CharacterController) {
                    colliders.Remove(ragdollPart);
                    m_colliders = colliders.ToArray();

                    break;
                }

            m_rigidbodies = GetComponentsInChildren<Rigidbody>();

            foreach (var rb in m_rigidbodies)
                if (rb.gameObject.name.ToLower().Contains("pelvis")) {
                    m_mainRigidbody = rb;

                    break;
                }

            Enabled = false;
        }


        public void AddImpulse (Vector3 force) {
            m_mainRigidbody.AddForce(force, ForceMode.VelocityChange);
        }

    }

}