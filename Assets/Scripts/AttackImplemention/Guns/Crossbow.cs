using UnityEngine;
using UnityEngine.SceneManagement;

public class Crossbow : Gun
{
    ///<summary>Урон от яда наносится врагу в течение определённого времени</summary>
    ///<value>[0, infinity]</value>
    [Header("Характеристики арбалета")]
    [Tooltip("Урон от яда наносится врагу в течение определённого времени. [0, infinity]")]
    [SerializeField, Min(0)] float poisonDamage;

    ///<summary>Время, в течение которого яд наносит урон врагу</summary>
    ///<value>[0, infinity]</value>
    [Tooltip("Время, в течение которого яд наносит урон врагу. [0, infinity]")]
    [SerializeField, Min(0)] float damageTime;

    ///<summary>Сила выстрела арбалета - определяет, с какой скоростью будет лететь стрела после выстрела</summary>
    [Tooltip("Сила выстрела арбалета - определяет, с какой скоростью будет лететь стрела после выстрела")]
    [SerializeField] MinMaxSliderFloat shotPower = new MinMaxSliderFloat(0, 25);

    public override void Shot(Vector3 target)
    {
        if (timeOfLastShot + intervalOfShots < Time.time)
        {
            for (int i = 0; i < muzzles.Length; i++)
            {
                Arrow arrow;
                if (ObjectsPool<Arrow>.IsEmpty())
                {
                    arrow = Instantiate(projectilePrefab) as Arrow;
                    SceneManager.MoveGameObjectToScene(arrow.gameObject, Game.ProjectilesScene);
                }
                else
                    arrow = ObjectsPool<Arrow>.Pop();
                arrow.transform.SetLocalPositionAndRotation(
                    muzzles[i].transform.position, muzzles[i].transform.rotation
                );
                arrow.PoisonDamage = poisonDamage;
                arrow.DamageTime = damageTime;
                Vector3 path = muzzles[i].transform.forward;
                Vector3 force = path * Random.Range(shotPower.minValue, shotPower.maxValue);
                arrow.AddImpulse(force);
            }
            timeOfLastShot = Time.time;
        }
    }
}
