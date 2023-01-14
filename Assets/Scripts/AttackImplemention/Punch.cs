using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour
{
    ///<summary>Урон, наносимый врагом игроку</summary>
    ///<value>Диапазон значений на отрезке [minLimit, maxLimit]</value>
    MinMaxSliderFloat damage;
    
    SphereCollider trigger;

    void Awake()
    {
        trigger = GetComponent<SphereCollider>();
        trigger.isTrigger = true;
    }

    void OnEnable() => trigger.enabled = true;
    void OnDisable() => trigger.enabled = false;

    public MinMaxSliderFloat Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PlayerCharacter>()?.Hit(Random.Range(damage.minValue, damage.maxValue));
    }
}
