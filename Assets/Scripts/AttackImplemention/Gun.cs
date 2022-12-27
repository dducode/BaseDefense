using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gun : MonoBehaviour
{
    [SerializeField] GunType gunType;
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] Transform[] muzzles;
    [SerializeField] float intervalShots;
    [SerializeField, Tooltip("Сила выстрела оружия - определяет, с какой скоростью будет лететь пуля после выстрела")] 
    MinMaxSliderFloat muzzleEnergy = new MinMaxSliderFloat(0, 25);
    float timeOfLastShot;

    public GunType GunType => gunType;

    void Start()
    {
        timeOfLastShot = Time.time;
    }

    public void Shot(Vector3 target)
    {
        if (timeOfLastShot + intervalShots <= Time.time)
        {
            for (int i = 0; i < muzzles.Length; i++)
            {
                Bullet bullet;
                if (ObjectsPool<Bullet>.IsEmpty())
                {
                    bullet = Instantiate(bulletPrefab);
                    SceneManager.MoveGameObjectToScene(bullet.gameObject, Game.BulletsScene);
                }
                else
                    bullet = ObjectsPool<Bullet>.Pop();
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
