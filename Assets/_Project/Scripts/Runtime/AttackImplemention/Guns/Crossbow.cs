using BaseDefense.AttackImplemention.Projectiles;
using BaseDefense.Properties;
using UnityEngine;
using UnityEngine.Assertions;

namespace BaseDefense.AttackImplemention.Guns {

    public class Crossbow : Gun {

        ///<summary>Урон от яда наносится врагу в течение определённого времени</summary>
        ///<value>[0, infinity]</value>
        [Header("Характеристики арбалета")]
        [Tooltip("Урон от яда наносится врагу в течение определённого времени. [0, infinity]")]
        [SerializeField, Min(0)]
        private float poisonDamage;

        ///<summary>Время, в течение которого яд наносит урон врагу</summary>
        ///<value>[0, infinity]</value>
        [Tooltip("Время, в течение которого яд наносит урон врагу. [0, infinity]")]
        [SerializeField, Min(0)]
        private float damageTime;

        ///<summary>Сила выстрела арбалета - определяет, с какой скоростью будет лететь стрела после выстрела</summary>
        [Tooltip("Сила выстрела арбалета - определяет, с какой скоростью будет лететь стрела после выстрела")]
        [SerializeField]
        private MinMaxSliderFloat shotPower = new MinMaxSliderFloat(0, 25);


        public override void Shot () {
            if (!(TimeOfLastShot + intervalOfShots < Time.time)) return;

            foreach (var muzzle in muzzles) {
                var arrow = Create(projectilePrefab) as Arrow;

                const string message = "Стрела не была создана";
                Assert.IsNotNull(arrow, message);

                var muzzleTransform = muzzle.transform;
                arrow.transform.SetLocalPositionAndRotation(
                    muzzleTransform.position, muzzleTransform.rotation
                );
                arrow.PoisonDamage = poisonDamage;
                arrow.DamageTime = damageTime;
                var path = muzzle.transform.forward;
                path.y = 0;
                var force = path.normalized * Random.Range(shotPower.minValue, shotPower.maxValue);
                arrow.AddImpulse(force);
            }

            TimeOfLastShot = Time.time;
        }

    }

}