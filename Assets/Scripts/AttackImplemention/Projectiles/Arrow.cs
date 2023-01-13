using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Arrow : Projectile
{
    ///<summary>Урон от яда наносится врагу в течение определённого времени</summary>
    ///<value>[0, infinity]</value>
    float poisonDamage;

    ///<summary>Время, в течение которого яд наносит урон врагу</summary>
    ///<value>[0, infinity]</value>
    float damageTime;

    ///<inheritdoc cref="poisonDamage"/>
    public float PoisonDamage
    {
        get { return poisonDamage; }
        set
        {
            poisonDamage = value;
            if (poisonDamage < 0) poisonDamage = 0;
        }
    }

    ///<inheritdoc cref="damageTime"/>
    public float DamageTime
    {
        get { return damageTime; }
        set
        {
            damageTime = value;
            if (damageTime < 0) damageTime = 0;
        }
    }

    ///<summary>Обычный урон. Зависит от скорости стрелы</summary>
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
            transform.parent = enemy.transform;
            rb.SetVelocityAndAngularVelocity(Vector3.zero, Vector3.zero);
            trailRenderer.emitting = false;
            rb.isKinematic = true;
            StartCoroutine(HitEnemyWithPoison(enemy));
        }
    }

    IEnumerator HitEnemyWithPoison(EnemyCharacter enemy)
    {
        float time = Time.time + damageTime;
        while (time > Time.time)
        {
            enemy.Hit(poisonDamage * Time.smoothDeltaTime);
            if (!enemy.IsAlive)
                break;
            yield return null;
        }
        transform.parent = null;
        trailRenderer.emitting = true;
        rb.isKinematic = false;
        SceneManager.MoveGameObjectToScene(gameObject, Game.ProjectilesScene);
        ObjectsPool<Arrow>.Push(this);
    }
}
