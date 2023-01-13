using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour
{
    ///<summary>Урон, наносимый врагом игроку</summary>
    ///<value>Диапазон значений на отрезке [minLimit, maxLimit]</value>
    MinMaxSliderFloat damage;

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
