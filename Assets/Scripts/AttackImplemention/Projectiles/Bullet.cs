using UnityEngine;
using BaseDefense.Extensions;

namespace BaseDefense.AttackImplemention.Projectiles
{
    public class Bullet : Projectile
    {
        ///<summary>Урон зависит от скорости пули</summary>
        float damage;

        public override void AddImpulse(Vector3 force)
        {
            trailRenderer.Clear();
            rb.AddForce(force);
            damage = force.magnitude;
        }

        public override void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<IAttackable>() is IAttackable attackable)
                attackable.Hit(damage);
            ObjectsPool<Bullet>.Push(this);
            rb.SetVelocityAndAngularVelocity(Vector3.zero, Vector3.zero);
        }
    }
}


