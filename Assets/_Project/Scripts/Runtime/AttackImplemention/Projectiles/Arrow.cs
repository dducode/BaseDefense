using System.Collections;
using BaseDefense.Characters;
using BaseDefense.Extensions;
using UnityEngine;

namespace BaseDefense.AttackImplemention.Projectiles {

    public class Arrow : Projectile {

        ///<summary>Урон от яда наносится врагу в течение определённого времени</summary>
        ///<value>[0, infinity]</value>
        private float m_poisonDamage;

        ///<summary>Время, в течение которого яд наносит урон врагу</summary>
        ///<value>[0, infinity]</value>
        private float m_damageTime;

        ///<inheritdoc cref="m_poisonDamage"/>
        public float PoisonDamage {
            get => m_poisonDamage;
            set {
                m_poisonDamage = value;
                if (m_poisonDamage < 0) m_poisonDamage = 0;
            }
        }

        ///<inheritdoc cref="m_damageTime"/>
        public float DamageTime {
            get => m_damageTime;
            set {
                m_damageTime = value;
                if (m_damageTime < 0) m_damageTime = 0;
            }
        }

        ///<summary>Обычный урон при попадании. Зависит от скорости стрелы</summary>
        private float m_damage;

        private Collider m_coll;


        protected override void Awake () {
            base.Awake();
            m_coll = GetComponent<Collider>();
        }


        public override void AddImpulse (Vector3 force) {
            trailRenderer.Clear();
            rb.AddForce(force);
            m_damage = force.magnitude;
        }


        protected override void OnCollisionEnter (Collision collision) {
            if (collision.gameObject.GetComponent<IAttackable>() is { } attackable) {
                attackable.Hit(m_damage);
                if (attackable is EnemyCharacter enemy)
                    Destroy(HitEnemyWithPoison(enemy));
                else
                    Destroy();
            }
            else
                Destroy();

            rb.SetVelocityAndAngularVelocity(Vector3.zero, Vector3.zero);
        }


        private IEnumerator HitEnemyWithPoison (EnemyCharacter enemy) {
            SetParams(true);
            var time = Time.time + m_damageTime;

            while (time > Time.time) {
                if (!enemy.IsAlive)
                    break;
                enemy.Hit(m_poisonDamage);

                yield return new WaitForSeconds(1);
            }

            SetParams(false);
            ObjectsPool.MoveObjectToHisScene(this);

            /*
            Устанавливает параметры для нормальной работы сопрограммы.
            Пока стрела поражает врага ядом, рендер пути, коллайдер стрелы и физика жёсткого тела будут отключены.
            Также на время стрела "прилипает" к врагу и проигрывается эффект поражения ядом.
            */
            void SetParams (bool value) {
                transform.parent = value ? enemy.transform : null;
                trailRenderer.emitting = !value;
                rb.isKinematic = value;
                m_coll.enabled = !value;
            }
        }

    }

}