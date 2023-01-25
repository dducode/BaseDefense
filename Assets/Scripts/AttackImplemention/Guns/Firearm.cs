using UnityEngine;

namespace BaseDefense
{
    public class Firearm : Gun
    {
        ///<summary>Разброс пуль при выстреле. Чем меньше - тем точнее выстрел</summary>
        ///<value>[0, 1]</value>
        [Header("Характеристики огнестрельного оружия")]
        [Tooltip("Разброс пуль при выстреле. Чем меньше - тем точнее выстрел. [0, 1]")]
        [SerializeField, Range(0, 1)] float dispertionScalar = 0.1f;

        ///<summary>Сила выстрела оружия - определяет, с какой скоростью будет лететь пуля после выстрела</summary>
        [Tooltip("Сила выстрела оружия - определяет, с какой скоростью будет лететь пуля после выстрела")]
        [SerializeField] MinMaxSliderFloat shotPower = new MinMaxSliderFloat(0, 25);

        public override void Shot()
        {
            if (timeOfLastShot + intervalOfShots < Time.time)
            {
                for (int i = 0; i < muzzles.Length; i++)
                {
                    Bullet bullet = ObjectsPool<Bullet>.Get(projectilePrefab as Bullet);
                    bullet.transform.SetLocalPositionAndRotation(
                        muzzles[i].transform.position, muzzles[i].transform.rotation
                    );
                    Vector3 dispertion = new Vector3(
                        Random.Range(-dispertionScalar, dispertionScalar), 
                        Random.Range(-dispertionScalar, dispertionScalar), 
                        0
                    );
                    Vector3 path = muzzles[i].transform.forward;
                    path.y = 0;
                    Vector3 force = (path.normalized + dispertion) * Random.Range(shotPower.minValue, shotPower.maxValue);
                    bullet.AddImpulse(force);
                }
                timeOfLastShot = Time.time;
            }
        }
    }
}


