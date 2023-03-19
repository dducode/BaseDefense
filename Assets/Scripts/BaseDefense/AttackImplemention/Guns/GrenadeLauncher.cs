using UnityEngine;
using BaseDefense.AttackImplemention.Projectiles;
using UnityEngine.Assertions;

namespace BaseDefense.AttackImplemention.Guns {

    public class GrenadeLauncher : Gun {

        ///<summary>Определяет радиус поражения при взрыве гранаты</summary>
        ///<value>[0.001, infinity]</value>
        [Header("Характеристики гранатомёта")]
        [Tooltip("Определяет радиус поражения при взрыве гранаты. [0.001, infinity]")]
        [SerializeField, Min(0.001f)]
        private float damageRadius;

        ///<summary>Урон зависит от дальности от эпицентра взрыва</summary>
        ///<value>[0, infinity]</value>
        [Tooltip("Урон зависит от дальности от эпицентра взрыва. [0, infinity]")]
        [SerializeField, Min(0)]
        private float maxDamage;

        public float DamageRadius => damageRadius;


        public override void Shot () {
            if (!(TimeOfLastShot + intervalOfShots < Time.time)) return;

            foreach (var muzzle in muzzles) {
                var grenade = Create(projectilePrefab) as Grenade;

                const string message = "Граната не была создана";
                Assert.IsNotNull(grenade, message);

                var muzzleTransform = muzzle.transform;
                grenade.transform.SetLocalPositionAndRotation(
                    muzzleTransform.position, muzzleTransform.rotation
                );
                grenade.DamageRadius = damageRadius;
                grenade.MaxDamage = maxDamage;
                var path = muzzle.transform.forward;
                path.y = 0;
                grenade.AddImpulse(path.normalized * 1000);
            }

            TimeOfLastShot = Time.time;
        }

    }

}