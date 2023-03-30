using UnityEngine;
using BaseDefense.Extensions;

// ReSharper disable Unity.PreferNonAllocApi

namespace BaseDefense.AttackImplemention.Projectiles {

    public class Grenade : Projectile {

        ///<summary>Эффект, который необходимо воспроизвести после попадания гранаты</summary>
        [Tooltip("Эффект, который необходимо воспроизвести после попадания гранаты")]
        [SerializeField]
        private ParticleSystem explosion;

        ///<summary>Определяет радиус поражения при взрыве гранаты</summary>
        ///<value>[0.001, infinity]</value>
        private float m_damageRadius;

        ///<summary>Урон зависит от дальности от эпицентра взрыва</summary>
        ///<value>[0, infinity]</value>
        private float m_maxDamage;

        ///<inheritdoc cref="m_damageRadius"/>
        public float DamageRadius {
            get => m_damageRadius;
            set {
                m_damageRadius = value;
                if (m_damageRadius < 0.001f) m_damageRadius = 0.001f;
            }
        }

        ///<inheritdoc cref="m_maxDamage"/>
        public float MaxDamage {
            get => m_maxDamage;
            set {
                m_maxDamage = value;
                if (m_maxDamage < 0) m_maxDamage = 0;
            }
        }


        public override void AddImpulse (Vector3 force) {
            trailRenderer.Clear();
            rb.AddForce(force);
        }


        protected override void OnCollisionEnter (Collision collision) {
            var position = transform.position;
            var colliders = Physics.OverlapSphere(position, m_damageRadius);
            Instantiate(explosion, position, Quaternion.identity);

            foreach (var attackableCollider in colliders) {
                if (attackableCollider.GetComponent<IAttackable>() is { } attackable) {
                    var distance = m_damageRadius;
                    var direction = attackableCollider.transform.position - transform.position;
                    if (Physics.Raycast(transform.position, direction, out var raycastHit))
                        distance = raycastHit.distance;
                    var damage = m_maxDamage * (1 - distance / m_damageRadius);
                    if (damage < 0)
                        damage = 0;
                    attackable.Hit(damage);
                }
            }

            colliders = Physics.OverlapSphere(transform.position, m_damageRadius);
            foreach (var rbCollider in colliders)
                if (rbCollider.TryGetComponent(out Rigidbody rb))
                    rb.AddExplosionForce(m_maxDamage, transform.position, m_damageRadius);
            Destroy();
            rb.SetVelocityAndAngularVelocity(Vector3.zero, Vector3.zero);
        }

    }

}