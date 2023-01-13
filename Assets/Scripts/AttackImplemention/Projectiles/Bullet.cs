using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (collision.gameObject.GetComponent<EnemyCharacter>() is EnemyCharacter enemy)
        {
            enemy.Hit(damage);
            Instantiate(effect, transform.position, Quaternion.identity);
        }
        ObjectsPool<Bullet>.Push(this);
        rb.SetVelocityAndAngularVelocity(Vector3.zero, Vector3.zero);
    }
}
