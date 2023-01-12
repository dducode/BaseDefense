using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour
{
    ///<summary>Урон, наносимый врагом игроку</summary>
    ///<value>Диапазон значений на отрезке [0, 100]</value>
    [Tooltip("Урон, наносимый врагом игроку")]
    [SerializeField] MinMaxSliderFloat damage = new MinMaxSliderFloat(0, 100);

    void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PlayerCharacter>()?.Hit(Random.Range(damage.minValue, damage.maxValue));
    }
}
