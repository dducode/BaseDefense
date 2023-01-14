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

    ///<summary>Обычный урон при попадании. Зависит от скорости стрелы</summary>
    float damage;

    Collider coll;

    public override void Awake()
    {
        base.Awake();
        coll = GetComponent<Collider>();
        Vector3 position = new Vector3(0, 0, 0.25f);
    }

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
            StartCoroutine(HitEnemyWithPoison(enemy));
        }
        else
            ObjectsPool<Arrow>.Push(this);
        rb.SetVelocityAndAngularVelocity(Vector3.zero, Vector3.zero);
    }

    IEnumerator HitEnemyWithPoison(EnemyCharacter enemy)
    {
        SetParams(true);
        float time = Time.time + damageTime;
        while (time > Time.time)
        {
            enemy.Hit(poisonDamage);
            if (!enemy.IsAlive)
                break;
            yield return new WaitForSeconds(1);
        }
        SetParams(false);
        SceneManager.MoveGameObjectToScene(gameObject, Game.ProjectilesScene);
        ObjectsPool<Arrow>.Push(this);

        /*
        Устанавливает параметры для нормальной работы сопрограммы.
        Пока стрела поражает врага ядом, рендер пути, коллайдер стрелы и физика жёсткого тела будут отключены.
        Также на время стрела "прилипает" к врагу и проигрывается эффект поражения ядом.
        */
        void SetParams(bool value)
        {
            transform.parent = value ? enemy.transform : null;
            trailRenderer.emitting = !value;
            rb.isKinematic = value;
            coll.enabled = !value;
        }
    }
}
