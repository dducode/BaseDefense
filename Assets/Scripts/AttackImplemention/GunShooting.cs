using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GunShooting : MonoBehaviour
{
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] float intervalShots;
    [SerializeField, Tooltip("Сила выстрела оружия - определяет, с какой скоростью будет лететь пуля после выстрела")] 
    MinMaxSliderFloat muzzleEnergy = new MinMaxSliderFloat(0, 25);
    float timeOfLastShot;
    List<GameObject> muzzles;

    void Start()
    {
        muzzles = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
            muzzles.Add(transform.GetChild(i).gameObject);
        timeOfLastShot = Time.time;
    }

    public void Shot(Vector3 target)
    {
        if (timeOfLastShot + intervalShots <= Time.time)
        {
            for (int i = 0; i < muzzles.Count; i++)
            {
                Bullet bullet;
                if (Pools.BulletsCount == 0)
                {
                    bullet = Instantiate(bulletPrefab);
                    SceneManager.MoveGameObjectToScene(bullet.gameObject, Game.BulletsScene);
                }
                else
                    bullet = Pools.PopBullet();
                bullet.transform.localPosition = muzzles[i].transform.position;
                bullet.transform.localRotation = muzzles[i].transform.rotation;
                Vector3 dispertion = new Vector3(Random.Range(-.1f, .1f), 0, 0);
                Vector3 path = (target - muzzles[i].transform.position).normalized;
                Vector3 force = (path + dispertion) * Random.Range(muzzleEnergy.minValue, muzzleEnergy.maxValue);
                bullet.Shot(force);
            }
            timeOfLastShot = Time.time;
        }
    }
}
