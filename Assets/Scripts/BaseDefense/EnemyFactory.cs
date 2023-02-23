using System.Collections.Generic;
using UnityEngine;
using Zenject;
using BaseDefense.Characters;

namespace BaseDefense
{
    public class EnemyFactory : MonoBehaviour
    {
        ///<summary>Максимально возможное количество врагов на базе</summary>
        ///<value>[1, 100]</value>
        [Tooltip("Максимально возможное количество врагов на базе. [1, 100]")]
        [SerializeField, Range(1, 100)]
        private int maxEnemiesCount = 10;

        ///<summary>Начальное количество врагов на базе</summary>
        ///<value>[0, 100]</value>
        [Tooltip("Начальное количество врагов на базе. [0, 100]")]
        [SerializeField, Range(0, 100)]
        private int startEnemiesCount = 5;

        ///<summary>Временной интервал между порождением новых врагов</summary>
        ///<value>[0, infinity]</value>
        [Tooltip("Временной интервал между порождением новых врагов. [0, infinity]")]
        [SerializeField, Min(0)]
        private float timeSpawn = 3f;

        ///<summary>Вражеские станции, находящиеся на базе</summary>
        [Tooltip("Вражеские станции, находящиеся на базе")]
        [SerializeField]
        private List<EnemyStation> enemyStations;

        ///<summary>Целевые точки для патруля врагами</summary>
        [Tooltip("Целевые точки для патруля врагами")]
        [SerializeField]
        private Transform[] targetPoints;

        ///<summary>Переходы между уровнями</summary>
        [Tooltip("Переходы между уровнями")]
        [SerializeField]
        private Transitions _transitions;

        ///<inheritdoc cref="_transitions"/>
        // ReSharper disable once InconsistentNaming
        public Transitions transitions => _transitions;

        ///<summary>Содержит всех врагов, находящихся на базе</summary>
        ///<remarks>Мёртвые враги удаляются из списка</remarks>
        private List<EnemyCharacter> m_enemies;

        private void Start()
        {
            m_enemies = new List<EnemyCharacter>();
            for (int i = 0; i < startEnemiesCount; i++)
                SpawnEnemy();
        }

        #region FactoryUpdate

        ///<summary>Время, прошедшее с момента последнего порождения врага</summary>
        private float m_timeOfLastSpawn;
        
        [Inject] private Game m_game;

        private void Update()
        {
            RemoveDestroyedStations();
            UpdateEnemies();

            if (enemyStations.Count != 0)
            {
                if (m_enemies.Count < maxEnemiesCount && m_timeOfLastSpawn + (timeSpawn / enemyStations.Count) < Time.time)
                {
                    SpawnEnemy();
                    m_timeOfLastSpawn = Time.time;
                }
            }
            else if (m_enemies.Count == 0)
            {
                // Уровень завершён только когда все вражеские базы и все враги уничтожены
                m_game.NextLevel();
                enabled = false;
            }
        }
        
        private void RemoveDestroyedStations()
        {
            for (int i = 0; i < enemyStations.Count; i++)
                if (enemyStations[i] == null)
                    enemyStations.RemoveAt(i--);
        }

        private void UpdateEnemies()
        {
            for (int i = 0; i < m_enemies.Count; i++)
                if (!m_enemies[i].EnemyUpdate())
                    m_enemies.RemoveAt(i--);
        }
        
        private int m_stationIndex;
        
        private void SpawnEnemy()
        {
            m_stationIndex++;
            if (m_stationIndex >= enemyStations.Count)
                m_stationIndex = 0;
            m_enemies.Add(enemyStations[m_stationIndex].SpawnEnemy(targetPoints));
        }

        #endregion

        #region TriggerEvents

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            
            if (!transitions.backTransition.gameObject.activeSelf)
            {
                transitions.backTransition.gameObject.SetActive(true);
                m_game.DestroyOldBase();
            }
            foreach (var enemy in m_enemies)
                enemy.AttackPlayer();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            
            foreach (var enemy in m_enemies)
                enemy.Patrol();
        }

        #endregion
        
        [System.Serializable]
        public struct Transitions
        {
            public Transform backTransition;
            public Transform frontTransition;
        }

        public class Factory : PlaceholderFactory<UnityEngine.Object, EnemyFactory> {}
    }
}


