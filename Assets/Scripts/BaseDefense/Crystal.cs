using BaseDefense.AttackImplemention;
using UnityEngine;
using BaseDefense.Items;
using BaseDefense.SaveSystem;

namespace BaseDefense
{
    [RequireComponent(typeof(ItemDrop))]
    public class Crystal : Object, IAttackable
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
                    DestroyCrystal();
            }
        }

        public override void Save(GameDataWriter writer)
        {
            base.Save(writer);
            writer.Write(CurrentHealthPoints);
        }

        public override void Load(GameDataReader reader)
        {
            base.Load(reader);
            CurrentHealthPoints = reader.ReadFloat();
        }

        private ItemDrop m_itemDrop;

        ///<summary>Вызывается для нанесения повреждений кристаллу</summary>
        ///<param name="damage">Количество нанесённых повреждений</param>
        public void Hit(float damage)
        {
            CurrentHealthPoints -= damage;
        }

        protected override void Awake()
        {
            base.Awake();
            CurrentHealthPoints = maxHealthPoints;
            m_itemDrop = GetComponent<ItemDrop>();
        }

        private void DestroyCrystal()
        {
            m_itemDrop.DropItems();
            Destroy();
        }
    }
}

