using UnityEngine;

public class Grenade : Projectile
{
    ///<summary>Эффект, который необходимо воспроизвести после попадания гранаты</summary>
    [Tooltip("Эффект, который необходимо воспроизвести после попадания гранаты")]
    [SerializeField] ParticleSystem explosion;

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

    public override void AddImpulse(Vector3 force)
    {
        trailRenderer.Clear();
        rb.AddForce(force);
    }

    public override void OnCollisionEnter(Collision collision)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        Instantiate(explosion, transform.position, Quaternion.identity);
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<IAttackable>() is IAttackable attackable)
            {
                float distance = damageRadius;
                float damage = 0;
                Vector3 direction = collider.transform.position - transform.position;
                if (Physics.Raycast(transform.position, direction, out RaycastHit raycastHit))
                    distance = raycastHit.distance;
                damage = maxDamage * (1 - distance / damageRadius);
                if (damage < 0)
                    damage = 0;
                attackable.Hit(damage);
            }
        }

        colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider collider in colliders)
            if (collider.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
                rigidbody.AddExplosionForce(maxDamage, transform.position, damageRadius);
        ObjectsPool<Grenade>.Push(this);
        rb.SetVelocityAndAngularVelocity(Vector3.zero, Vector3.zero);
    }
}
