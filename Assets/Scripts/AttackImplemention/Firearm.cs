using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Firearm : Gun
{
    [Header("Характеристики огнестрельного оружия")]
    [SerializeField] Transform[] muzzles;

    ///<summary>Сила выстрела оружия - определяет, с какой скоростью будет лететь пуля после выстрела</summary>
    [SerializeField, Tooltip("Сила выстрела оружия - определяет, с какой скоростью будет лететь пуля после выстрела")] 
    MinMaxSliderFloat muzzleEnergy = new MinMaxSliderFloat(0, 25);

    float timeOfLastShot;

    void Start()
    {
        timeOfLastShot = Time.time;
    }

    public override void Shot(Vector3 target)
    {
        if (timeOfLastShot + intervalOfShots <= Time.time)
        {
            for (int i = 0; i < muzzles.Length; i++)
            {
                Bullet bullet;
                if (ObjectsPool<Bullet>.IsEmpty())
                {
                    bullet = Instantiate(projectilePrefab) as Bullet;
                    SceneManager.MoveGameObjectToScene(bullet.gameObject, Game.BulletsScene);
                }
                else
                    bullet = ObjectsPool<Bullet>.Pop();
                bullet.transform.SetLocalPositionAndRotation(
                    muzzles[i].transform.position, muzzles[i].transform.rotation
                );
                Vector3 dispertion = new Vector3(Random.Range(-.1f, .1f), 0, 0);
                Vector3 path = (target - muzzles[i].transform.position).normalized;
                Vector3 force = (path + dispertion) * Random.Range(muzzleEnergy.minValue, muzzleEnergy.maxValue);
                bullet.AddImpulse(force);
            }
            timeOfLastShot = Time.time;
        }
    }
}
