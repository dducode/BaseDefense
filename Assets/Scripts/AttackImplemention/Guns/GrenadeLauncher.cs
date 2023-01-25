using UnityEngine;

namespace BaseDefense
{
    public class GrenadeLauncher : Gun
    {
        ///<summary>Определяет радиус поражения при взрыве гранаты</summary>
        ///<value>[0.001, infinity]</value>
        [Header("Характеристики гранатомёта")]
        [Tooltip("Определяет радиус поражения при взрыве гранаты. [0.001, infinity]")]
        [SerializeField, Min(0.001f)] float damageRadius;

        ///<summary>Урон зависит от дальности от эпицентра взрыва</summary>
        ///<value>[0, infinity]</value>
        [Tooltip("Урон зависит от дальности от эпицентра взрыва. [0, infinity]")]
        [SerializeField, Min(0)] float maxDamage;

        public float DamageRadius => damageRadius;

        public override void Shot()
        {
            if (timeOfLastShot + intervalOfShots < Time.time)
            {
                for (int i = 0; i < muzzles.Length; i++)
                {
                    Grenade grenade = ObjectsPool<Grenade>.Get(projectilePrefab as Grenade);
                    grenade.transform.SetLocalPositionAndRotation(
                        muzzles[i].transform.position, muzzles[i].transform.rotation
                    );
                    grenade.DamageRadius = damageRadius;
                    grenade.MaxDamage = maxDamage;
                    Vector3 path = muzzles[i].transform.forward;
                    path.y = 0;
                    grenade.AddImpulse(path.normalized * 1000);
                }
                timeOfLastShot = Time.time;
            }
        }
    }
}


