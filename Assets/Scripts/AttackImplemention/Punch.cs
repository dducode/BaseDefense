using UnityEngine;
using BaseDefense.Characters;
using BaseDefense.Properties;

namespace BaseDefense.AttackImplemention {

    [RequireComponent(typeof(SphereCollider))]
    public class Punch : MonoBehaviour {

        ///<summary>При включении поведения также включается прикреплённый триггер</summary>
        private bool m_enabled;

        ///<inheritdoc cref="m_enabled"/>
        public bool Enabled {
            get => m_enabled;
            set {
                m_enabled = value;
                enabled = m_enabled;
                m_trigger.enabled = m_enabled;
            }
        }

        ///<summary>Урон, наносимый врагом игроку</summary>
        ///<value>Диапазон значений на отрезке [minLimit, maxLimit]</value>
        public MinMaxSliderFloat Damage { get; set; }

        private SphereCollider m_trigger;


        private void Awake () {
            m_trigger = GetComponent<SphereCollider>();
            m_trigger.isTrigger = true;
        }


        private void OnTriggerEnter (Collider other) {
            other.GetComponent<PlayerCharacter>()?.Hit(Random.Range(Damage.minValue, Damage.maxValue));
        }

    }

}