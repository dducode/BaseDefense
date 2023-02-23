using BaseDefense.AttackImplemention;
using UnityEngine;
using BaseDefense.Items;

namespace BaseDefense
{
    [RequireComponent(typeof(ItemDrop))]
    public class Crystal : MonoBehaviour, IAttackable
    {
        ///<summary>Максимально возможное количество очков "здоровья" для кристалла</summary>
        ///<value>[1, infinity]</value>
        [Tooltip("Максимально возможное количество очков 'здоровья' для кристалла. [1, infinity]")]
        [SerializeField, Min(1)] private float maxHealthPoints = 100;

        ///<summary>Текущее количество очков "здоровья" кристалла</summary>
        ///<value>[0, maxHealthPoints]</value>
        private float m_currentHealthPoints;
        ///<inheritdoc cref="m_currentHealthPoints"/>
        public float CurrentHealthPoints
        {
            get => m_currentHealthPoints;
            private set
            {
                m_currentHealthPoints = value;
                m_currentHealthPoints = Mathf.Clamp(m_currentHealthPoints, 0, maxHealthPoints);
                if (m_currentHealthPoints == 0)
                    Destroy();
            }
        }

        private ItemDrop m_itemDrop;

        private void Awake()
        {
            CurrentHealthPoints = maxHealthPoints;
            m_itemDrop = GetComponent<ItemDrop>();
        }

        ///<summary>Вызывается для нанесения повреждений кристаллу</summary>
        ///<param name="damage">Количество нанесённых повреждений</param>
        public void Hit(float damage)
        {
            CurrentHealthPoints -= damage;
        }

        private void Destroy()
        {
            m_itemDrop.DropItems();
            Destroy(gameObject);
        }
    }
}

