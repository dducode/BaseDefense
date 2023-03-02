using System;
using System.Collections.Generic;
using System.Linq;
using BaseDefense.BroadcastMessages;
using UnityEngine;
using Zenject;
using BaseDefense.Characters;
using BaseDefense.SaveSystem;
using TMPro;
using UnityEngine.Assertions;

namespace BaseDefense
{
    public class EnemyBase : Object
    {
        [SerializeField] private Transform enemyField;
        [SerializeField] private TextMeshPro baseName;
        
        ///<summary>Максимально возможное количество врагов на базе</summary>
        ///<value>[1, 100]</value>
        [Tooltip("Максимально возможное количество врагов на базе. [1, 100]")]
        [SerializeField, Range(1, 100)]
        private int maxEnemiesCount = 10;

        ///<summary>Временной интервал между порождением новых врагов</summary>
        ///<value>[0, infinity]</value>
        [Tooltip("Временной интервал между порождением новых врагов. [0, infinity]")]
        [SerializeField, Min(0)]
        private float timeSpawn = 3f;

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

        [Inject] private EnemyStation.Factory m_enemyStationFactory;

        ///<summary>Содержит всех врагов, находящихся на базе</summary>
        ///<remarks>Мёртвые враги удаляются из списка</remarks>
        private List<EnemyCharacter> m_enemies;

        private List<EnemyStation> m_enemyStations;
        private List<Crystal> m_crystals;

        public override void Save(GameDataWriter writer)
        {
            base.Save(writer);
            
            writer.Write(baseName.text);
            SaveEnemies(writer);
            SaveEnemyStations(writer);
            SaveCrystals(writer);
        }

        #region SavingObjects

        private void SaveEnemies(GameDataWriter writer)
        {
            writer.Write(m_enemies.Count);
            
            foreach (var enemy in m_enemies)
            {
                writer.Write(enemy.Id);
                enemy.Save(writer);
            }
        }

        private void SaveEnemyStations(GameDataWriter writer)
        {
            writer.Write(m_enemyStations.Count);
            
            foreach (var enemyStation in m_enemyStations)
            {
                writer.Write(enemyStation.Id);
                enemyStation.Save(writer);
            }
        }

        private void SaveCrystals(GameDataWriter writer)
        {
            writer.Write(m_crystals.Count);
            
            foreach (var crystal in m_crystals)
            {
                writer.Write(crystal.Id);
                crystal.Save(writer);
            }
        }

        #endregion

        public override void Load(GameDataReader reader)
        {
            base.Load(reader);

            baseName.text = reader.ReadString();
            LoadEnemies(reader);
            LoadEnemyStations(reader);
            LoadCrystals(reader);
        }

        #region LoadingObjects

        private void LoadEnemies(GameDataReader reader)
        {
            var enemiesCount = reader.ReadInteger();

            for (int i = 0; i < enemiesCount; i++)
            {
                var enemyId = reader.ReadInteger();
                var enemy = Create(enemyId) as EnemyCharacter;
                const string message = "Враг не был загружен";
                Assert.IsNotNull(enemy, message);
                enemy.Load(reader);
                m_enemies.Add(enemy);
            }
        }

        private void LoadEnemyStations(GameDataReader reader)
        {
            var enemyStationsCount = reader.ReadInteger();

            for (int i = 0; i < enemyStationsCount; i++)
            {
                var enemyStationId = reader.ReadInteger();
                var enemyStation = CreateFromFactory(enemyStationId, m_enemyStationFactory) as EnemyStation;
                const string message = "Вражеская станция не была загружена";
                Assert.IsNotNull(enemyStation, message);
                enemyStation.Load(reader);
                m_enemyStations.Add(enemyStation);
            }
        }

        private void LoadCrystals(GameDataReader reader)
        {
            var crystalsCount = reader.ReadInteger();

            for (int i = 0; i < crystalsCount; i++)
            {
                var crystalId = reader.ReadInteger();
                var crystal = Create(crystalId) as Crystal;
                const string message = "Кристалл не был загружен";
                Assert.IsNotNull(crystal, message);
                crystal.Load(reader);
                m_crystals.Add(crystal);
            }
        }

        #endregion

        public void Initialize(BaseTemplate baseTemplate)
        {
            baseName.text = baseTemplate.name;
            m_enemyStations = CreateStations(baseTemplate.EnemyStations.ToList());
            m_crystals = CreateCrystals(baseTemplate.Crystals.ToList());
            m_enemies = new List<EnemyCharacter>();
            for (int i = 0; i < maxEnemiesCount; i++)
                m_enemies.Add(SpawnEnemy());
        }

        private List<EnemyStation> CreateStations(List<EnemyStation> stationsOriginal)
        {
            var stations = new List<EnemyStation>(stationsOriginal.Count);
            
            foreach (var station in stationsOriginal)
            {
                var position = station.transform.position + transform.position;
                var parent = enemyField;
                stations.Add(CreateFromFactory(
                    station, m_enemyStationFactory, position, Quaternion.identity, parent) as EnemyStation);
            }

            return stations;
        }

        private List<Crystal> CreateCrystals(List<Crystal> crystalsOriginal)
        {
            var crystals = new List<Crystal>(crystalsOriginal.Count);

            foreach (var crystal in crystalsOriginal)
            {
                var position = crystal.transform.position + transform.position;
                var parent = enemyField;
                crystals.Add(Create(crystal, position, Quaternion.identity, parent) as Crystal);
            }

            return crystals;
        }

        #region FactoryUpdate

        ///<summary>Время, прошедшее с момента последнего порождения врага</summary>
        private float m_timeOfLastSpawn;
        
        [Inject] private Game m_game;

        private void Update()
        {
            RemoveDestroyedStations();
            RemoveDestroyedCrystals();
            UpdateEnemies();

            if (AnyStationNotDestroyed())
            {
                if (m_enemies.Count < maxEnemiesCount && m_timeOfLastSpawn + (timeSpawn / m_enemyStations.Count) < Time.time)
                {
                    m_enemies.Add(SpawnEnemy());
                    m_timeOfLastSpawn = Time.time;
                }
            }
            else if (AllEnemiesDead())
            {
                // Уровень завершён только когда все вражеские базы и все враги уничтожены
                Messenger.SendMessage(MessageType.NEXT_LEVEL);
                enabled = false;
            }
        }
        
        private void RemoveDestroyedStations()
        {
            for (int i = 0; i < m_enemyStations.Count; i++)
                if (m_enemyStations[i].IsDestroyed)
                    m_enemyStations.RemoveAt(i--);
        }

        private void RemoveDestroyedCrystals()
        {
            for (int i = 0; i < m_crystals.Count; i++)
                if (m_crystals[i].IsDestroyed)
                    m_crystals.RemoveAt(i--);
        }

        private bool AnyStationNotDestroyed()
        {
            return m_enemyStations.Count != 0;
        }
        
        private void UpdateEnemies()
        {
            for (int i = 0; i < m_enemies.Count; i++)
                if (!m_enemies[i].EnemyUpdate())
                    m_enemies.RemoveAt(i--);
        }

        private bool AllEnemiesDead()
        {
            return m_enemies.Count == 0;
        }
        
        private int m_stationIndex;
        
        private EnemyCharacter SpawnEnemy()
        {
            m_stationIndex++;
            if (m_stationIndex >= m_enemyStations.Count)
                m_stationIndex = 0;
            return m_enemyStations[m_stationIndex].SpawnEnemy(targetPoints);
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
        
        [Serializable]
        public struct Transitions
        {
            public Transform backTransition;
            public Transform frontTransition;
        }

        public class Factory : PlaceholderFactory<UnityEngine.Object, EnemyBase> {}
    }
}


