using UnityEngine;
using UnityEngine.SceneManagement;

public class GrenadeLauncher : Gun
{
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
                Vector3 path = (target - muzzles[i].transform.position).normalized;
                grenade.AddImpulse(path * 1000);
            }
            timeOfLastShot = Time.time;
        }
    }
}
