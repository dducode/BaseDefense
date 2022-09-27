using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShooting : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] float intervalShots;
    float _intervalShots;
    GameObject muzzle;

    void Start()
    {
        muzzle = transform.GetChild(0).gameObject;
        _intervalShots = intervalShots;
    }

    public void Shot(Vector3 target)
    {
        if (_intervalShots >= intervalShots)
        {
            GameObject bull = Instantiate(bullet, muzzle.transform.position, muzzle.transform.rotation);
            Rigidbody bulletRB = bull.GetComponent<Rigidbody>();
            Vector3 dispertion = new Vector3(Random.Range(-.1f, .1f), 0, 0);
            Vector3 path = target - muzzle.transform.position;
            path = Vector3.Normalize(path);
            bulletRB.AddForce((path + dispertion) * 7.5f);
            _intervalShots = 0;
        }
        else
            _intervalShots += Time.deltaTime;
    }
}
