using UnityEngine;
using UnityEngine.SceneManagement;

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

    public override void Shot(Vector3 target)
    {
        if (timeOfLastShot + intervalOfShots < Time.time)
        {
            for (int i = 0; i < muzzles.Length; i++)
            {
                Grenade grenade;
                if (ObjectsPool<Grenade>.IsEmpty())
                {
                    grenade = Instantiate(projectilePrefab) as Grenade;
                    SceneManager.MoveGameObjectToScene(grenade.gameObject, Game.ProjectilesScene);
                }
                else
                    grenade = ObjectsPool<Grenade>.Pop();
                grenade.transform.SetLocalPositionAndRotation(
                    muzzles[i].transform.position, muzzles[i].transform.rotation
                );
                grenade.DamageRadius = damageRadius;
                grenade.MaxDamage = maxDamage;
                Vector3 path = (target - muzzles[i].transform.position).normalized;
                grenade.AddImpulse(path * 1000);
            }
            timeOfLastShot = Time.time;
        }
    }
}
