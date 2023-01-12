using UnityEngine;

public class Grenade : Projectile
{
    ///<summary>Определяет радиус поражения при взрыве гранаты</summary>
    ///<value>[0.001, infinity]</value>
    float damageRadius;

    ///<summary>Урон зависит от дальности от эпицентра взрыва</summary>
    ///<value>[0, infinity]</value>
    float maxDamage;

    ///<inheritdoc cref="damageRadius"/>
    public float DamageRadius
    {
        get { return damageRadius; }
        set
        {
            damageRadius = value;
            if (damageRadius < 0.001f) damageRadius = 0.001f;
        }
    }

    ///<inheritdoc cref="maxDamage"/>
    public float MaxDamage
    {
        get { return maxDamage; }
        set
        {
            maxDamage = value;
            if (maxDamage < 0) maxDamage = 0;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }

    public override void AddImpulse(Vector3 force)
    {
        trailRenderer.Clear();
        rb.AddForce(force);
    }

    void OnCollisionEnter(Collision other)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        ParticleSystem explosion = Instantiate(effect, transform.position, Quaternion.identity);
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<BaseCharacter>() is BaseCharacter character)
            {
                Vector3 characterPosition = character.transform.position + character.Controller.center;
                float distance = (characterPosition - transform.position).magnitude;
                distance -= character.Controller.radius;
                character.Hit(maxDamage * (1 - distance / damageRadius));
            }
        }
        ObjectsPool<Grenade>.Push(this);
        rb.SetVelocityAndAngularVelocity(Vector3.zero, Vector3.zero);
    }
}
