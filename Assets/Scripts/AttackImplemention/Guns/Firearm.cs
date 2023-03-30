using UnityEngine;
using BaseDefense.AttackImplemention.Projectiles;
using BaseDefense.Properties;
using UnityEngine.Assertions;

namespace BaseDefense.AttackImplemention.Guns {

    public class Firearm : Gun {

        ///<summary>Разброс пуль при выстреле. Чем меньше - тем точнее выстрел</summary>
        ///<value>[0, 1]</value>
        [Header("Характеристики огнестрельного оружия")]
        [Tooltip("Разброс пуль при выстреле. Чем меньше - тем точнее выстрел. [0, 1]")]
        [SerializeField, Range(0, 1)]
        private float dispersionScalar = 0.1f;

        ///<summary>Сила выстрела оружия - определяет, с какой скоростью будет лететь пуля после выстрела</summary>
        [Tooltip("Сила выстрела оружия - определяет, с какой скоростью будет лететь пуля после выстрела")]
        [SerializeField]
        private MinMaxSliderFloat shotPowerRange = new MinMaxSliderFloat(0, 25);


        public override void Shot () {
            if (!(TimeOfLastShot + intervalOfShots < Time.time)) return;

            foreach (var muzzle in muzzles) {
                var bullet = Create(projectilePrefab);

                bullet.transform.localPosition = muzzle.transform.position;
                var dispersion = new Vector3(
                    Random.Range(-dispersionScalar, dispersionScalar),
                    Random.Range(-dispersionScalar, dispersionScalar),
                    0
                );
                var path = muzzle.transform.forward;
                path.y = 0;
                var force = (path.normalized + dispersion) *
                            Random.Range(shotPowerRange.minValue, shotPowerRange.maxValue);
                bullet.AddImpulse(force);
            }

            TimeOfLastShot = Time.time;
        }

    }

}