using BaseDefense.AttackImplemention;
using UnityEngine;
using Zenject;
using BaseDefense.UI;
using BaseDefense.Characters;
using UnityEngine.Assertions;

namespace BaseDefense
{
    [RequireComponent(typeof(DisplayHealthPoints))]
    public class EnemyStation : MonoBehaviour, IAttackable
    {
        ///<summary>Максимальное количество здоровья станции</summary>
        ///<value>[1, infinity]</value>
        [Tooltip("Максимальное количество здоровья станции. [1, infinity]")]
        [SerializeField, Min(1)] private float maxHealthPoints = 300;

        ///<summary>Анимация, воспроизводимая при уничтожении станции</summary>
        [Tooltip("Анимация, воспроизводимая при уничтожении станции")]
        [SerializeField] private ParticleSystem destroyEffect;

        ///<summary>Максимальное расстояние от центра спавна для порождения новых врагов</summary>
        ///<value>[0, infinity]</value>
        [Tooltip("Максимальное расстояние от центра спавна для порождения новых врагов. [0, infinity]")]
        [SerializeField, Min(0)] private float radiusSpawn = 10f;

        [SerializeField] private EnemyCharacter enemyPrefab;
        [SerializeField] private Transform[] spawnPoints;

        [Inject] private EnemyCharacter.Factory m_enemyFactory;
        
        public EnemyCharacter SpawnEnemy(Transform[] targetPoints)
        {
            var index = Random.Range(0, spawnPoints.Length - 1);
            var position = spawnPoints[index].position + Random.insideUnitSphere * radiusSpawn;
            position.y = 0;
            var rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
            var enemy = Object.CreateFromFactory(enemyPrefab, m_enemyFactory) as EnemyCharacter;

            const string message = "Враг не был создан";
            Assert.IsNotNull(enemy, message);
            
            enemy.Spawn(targetPoints, position, rotation);

            return enemy;
        }
        
        private DisplayHealthPoints m_displayHealthPoints;
        ///<summary>Текущее количество здоровья базы</summary>
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
                    DestroyStation();
            }
        }

        public void Hit(float damage)
        {
            CurrentHealthPoints -= damage;
            m_displayHealthPoints.UpdateView((int)CurrentHealthPoints);
        }

        private void Awake()
        {
            CurrentHealthPoints = maxHealthPoints;
            m_displayHealthPoints = GetComponent<DisplayHealthPoints>();
        }

        private void Start()
        {
            m_displayHealthPoints.SetMaxValue((int)maxHealthPoints);
            m_displayHealthPoints.UpdateView((int)CurrentHealthPoints);
        }

        private void OnDrawGizmosSelected()
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
                return;
            Gizmos.color = Color.green;
            for (int i = 0; i < spawnPoints.Length; i++)
                Gizmos.DrawWireSphere(spawnPoints[i].position, radiusSpawn);
        }

        private void DestroyStation()
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}

