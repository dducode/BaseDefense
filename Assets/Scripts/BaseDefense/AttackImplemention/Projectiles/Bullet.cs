using BaseDefense.Exceptions;
using UnityEngine;
using BaseDefense.Extensions;

namespace BaseDefense.AttackImplemention.Projectiles
{
    public class Bullet : Projectile
    {
        ///<summary>Урон зависит от скорости пули</summary>
        private float m_damage;

        public override void AddImpulse(Vector3 force)
        {
            TrailRenderer.Clear();
            Rb.AddForce(force);
            transform.rotation = Quaternion.LookRotation(force);
            m_damage = force.magnitude;
        }

        protected override void OnCollisionEnter(Collision collision)
        {
            if (IsDestroyed)
                return;
            
            if (collision.gameObject.GetComponent<IAttackable>() is { } attackable)
                attackable.Hit(m_damage);
            Destroy();
            Rb.SetVelocityAndAngularVelocity(Vector3.zero, Vector3.zero);
        }
    }
}


