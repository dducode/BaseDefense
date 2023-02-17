using UnityEngine;
using BaseDefense.Characters;

namespace BaseDefense.AttackImplemention
{
    [RequireComponent(typeof(SphereCollider))]
    public class Punch : MonoBehaviour
    {
        ///<summary>При включении поведения также включается прикреплённый триггер</summary>
        bool _enabled;
        ///<inheritdoc cref="_enabled"/>
        public new bool enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                base.enabled = _enabled;
                trigger.enabled = _enabled;
            }
        }

        public MinMaxSliderFloat Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        ///<summary>Урон, наносимый врагом игроку</summary>
        ///<value>Диапазон значений на отрезке [minLimit, maxLimit]</value>
        MinMaxSliderFloat damage;
        SphereCollider trigger;

        void Awake()
        {
            trigger = GetComponent<SphereCollider>();
            trigger.isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            other.GetComponent<PlayerCharacter>()?.Hit(Random.Range(damage.minValue, damage.maxValue));
        }
    }
}

