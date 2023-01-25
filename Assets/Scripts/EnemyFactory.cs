using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace BaseDefense
{
    public class EnemyFactory : MonoBehaviour
    {
        ///<summary>Максимально возможное количество врагов на базе</summary>
        ///<value>[1, 100]</value>
        [Tooltip("Максимально возможное количество врагов на базе. [1, 100]")]
        [SerializeField, Range(1, 100)] int maxEnemiesCount = 10;

        ///<summary>Начальное количество врагов на базе</summary>
        ///<value>[0, 100]</value>
        [Tooltip("Начальное количество врагов на базе. [0, 100]")]
        [SerializeField, Range(0, 100)] int startEnemiesCount = 5;

        ///<summary>Временной интервал между порождением новых врагов</summary>
        ///<value>[0, infinity]</value>
        [Tooltip("Временной интервал между порождением новых врагов. [0, infinity]")]
        [SerializeField, Min(0)] float timeSpawn = 3f;

        ///<summary>Вражеские станции, находящиеся на базе</summary>
        [Tooltip("Вражеские станции, находящиеся на базе")]
        [SerializeField] List<EnemyStation> enemyStations;

        ///<summary>Целевые точки для патруля врагами</summary>
        [Tooltip("Целевые точки для патруля врагами")]
        [SerializeField] Transform[] targetPoints;

        ///<summary>Переходы между уровнями</summary>
        [Tooltip("Переходы между уровнями")]
        [SerializeField] Transitions _transitions;

        ///<inheritdoc cref="_transitions"/>
        public Transitions transitions => _transitions;

        ///<summary>Содержит всех врагов, находящихся на базе</summary>
        ///<remarks>Мёртвые враги удаляются из списка</remarks>
        List<EnemyCharacter> enemies;

        ///<summary>Время, прошедшее с момента последнего порождения врага</summary>
        float timeOfLastSpawn;
        int stationIndex;
        [Inject] Game game;

        void Start()
        {
            enemies = new List<EnemyCharacter>();
            for (int i = 0; i < startEnemiesCount; i++)
                SpawnEnemy();
        }

        void Update()
        {
            // Удаление уничтоженных станций
            for (int i = 0; i < enemyStations.Count; i++)
                if (enemyStations[i] == null)
                    enemyStations.RemoveAt(i--);

            if (enemyStations.Count != 0)
            {
                if (enemies.Count < maxEnemiesCount && timeOfLastSpawn + (timeSpawn / enemyStations.Count) < Time.time)
                {
                    SpawnEnemy();
                    timeOfLastSpawn = Time.time;
                }
            }
            else if (enemies.Count == 0)
            // Уровень завершён только когда все вражеские базы и все враги уничтожены
            {
                game.NextLevel();
                enabled = false;
            }

            // Обновление всех имеющихся врагов. Удаление мёртвых врагов
            for (int i = 0; i < enemies.Count; i++)
                if (!enemies[i].EnemyUpdate())
                    enemies.RemoveAt(i--);
        }

        void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (!transitions.backTransition.gameObject.activeSelf)
                {
                    transitions.backTransition.gameObject.SetActive(true);
                    game.DestroyOldBase();
                }
                foreach (EnemyCharacter enemy in enemies)
                    enemy.SetTrigger(true);
            }
        }
        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                foreach (EnemyCharacter enemy in enemies)
                    enemy.SetTrigger(false);
            }
        }

        void SpawnEnemy()
        {
            stationIndex++;
            if (stationIndex >= enemyStations.Count)
                stationIndex = 0;
            enemies.Add(enemyStations[stationIndex].SpawnEnemy(targetPoints));
        }

        [System.Serializable]
        public struct Transitions
        {
            public Transform backTransition;
            public Transform frontTransition;
        }

        public class Factory : PlaceholderFactory<Object, EnemyFactory> {}
    }
}


