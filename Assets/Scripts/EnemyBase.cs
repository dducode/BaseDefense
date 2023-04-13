using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using BaseDefense.Characters;
using BaseDefense.Messages;
using BroadcastMessages;
using SaveSystem;
using TMPro;
using UnityEngine.Serialization;

namespace BaseDefense {

    public class EnemyBase : Object {

        [SerializeField]
        private Transform enemyField;

        [SerializeField]
        private TextMeshPro baseName;

        ///<summary>Максимально возможное количество врагов на базе</summary>
        ///<value>[1, 100]</value>
        [Tooltip("Максимально возможное количество врагов на базе. [1, 100]")]
        [SerializeField, Range(1, 100)]
        private int maxEnemiesCount = 10;

        [SerializeField]
        private int startEnemiesCount = 5;

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
        [FormerlySerializedAs("_transitions")]
        [Tooltip("Переходы между уровнями")]
        [SerializeField]
        private Transitions transitionsBetweenBases;

        ///<inheritdoc cref="transitionsBetweenBases"/>
        public Transitions TransitionsBetweenBases => transitionsBetweenBases;

        [Inject]
        private EnemyStation.Factory m_enemyStationFactory;

        [Inject]
        private EnemyCharacter.Factory m_enemyFactory;


        ///<summary>Содержит всех врагов, находящихся на базе</summary>
        ///<remarks>Мёртвые враги удаляются из списка</remarks>
        private List<EnemyCharacter> m_enemies;

        private List<EnemyStation> m_enemyStations;
        private List<Crystal> m_crystals;


        public override void Save (UnityWriter writer) {
            base.Save(writer);

            writer.Write(enabled);
            writer.Write(baseName.text);
            writer.Write(TransitionsBetweenBases.frontTransition.gameObject.activeSelf);
            writer.Write(TransitionsBetweenBases.backTransition.gameObject.activeSelf);
            SaveEnemies(writer);
            SaveEnemyStations(writer);
            SaveCrystals(writer);
        }


        public override void Load (UnityReader reader) {
            base.Load(reader);

            enabled = reader.ReadBool();
            baseName.text = reader.ReadString();
            TransitionsBetweenBases.frontTransition.gameObject.SetActive(reader.ReadBool());
            TransitionsBetweenBases.backTransition.gameObject.SetActive(reader.ReadBool());
            LoadEnemies(reader);
            LoadEnemyStations(reader);
            LoadCrystals(reader);
        }


        public void Initialize (BaseTemplate baseTemplate) {
            baseName.text = baseTemplate.name;
            m_enemyStations = CreateStations(baseTemplate.EnemyStations.ToList());
            m_crystals = CreateCrystals(baseTemplate.Crystals.ToList());
            m_enemies = new List<EnemyCharacter>();
            for (int i = 0; i < startEnemiesCount; i++)
                m_enemies.Add(SpawnEnemy());
        }



        #region SavingObjects

        private void SaveEnemies (UnityWriter writer) {
            writer.Write(m_enemies.Count);

            foreach (var enemy in m_enemies) {
                writer.Write(enemy.Id);
                enemy.Save(writer);
            }
        }


        private void SaveEnemyStations (UnityWriter writer) {
            writer.Write(m_enemyStations.Count);

            foreach (var enemyStation in m_enemyStations) {
                writer.Write(enemyStation.Id);
                enemyStation.Save(writer);
            }
        }


        private void SaveCrystals (UnityWriter writer) {
            writer.Write(m_crystals.Count);

            foreach (var crystal in m_crystals) {
                writer.Write(crystal.Id);
                crystal.Save(writer);
            }
        }

        #endregion



        #region LoadingObjects

        private void LoadEnemies (UnityReader reader) {
            m_enemies = new List<EnemyCharacter>();
            var enemiesCount = reader.ReadInt();

            for (var i = 0; i < enemiesCount; i++) {
                var enemyId = reader.ReadInt();
                var enemy = CreateFromFactory(enemyId, m_enemyFactory);
                enemy.Load(reader);
                var enemyTransform = enemy.transform;
                enemy.Initialize(targetPoints, enemyTransform.position, enemyTransform.rotation);
                m_enemies.Add(enemy);
            }
        }


        private void LoadEnemyStations (UnityReader reader) {
            m_enemyStations = new List<EnemyStation>();
            var enemyStationsCount = reader.ReadInt();

            for (var i = 0; i < enemyStationsCount; i++) {
                var enemyStationId = reader.ReadInt();
                var enemyStation = CreateFromFactory(enemyStationId, m_enemyStationFactory, enemyField);
                enemyStation.Load(reader);
                m_enemyStations.Add(enemyStation);
            }
        }


        private void LoadCrystals (UnityReader reader) {
            m_crystals = new List<Crystal>();
            var crystalsCount = reader.ReadInt();

            for (var i = 0; i < crystalsCount; i++) {
                var crystalId = reader.ReadInt();
                var crystal = Create<Crystal>(crystalId, enemyField);
                crystal.Load(reader);
                m_crystals.Add(crystal);
            }
        }

        #endregion



        #region Initialization

        private List<EnemyStation> CreateStations (List<EnemyStation> stationsOriginal) {
            var stations = new List<EnemyStation>(stationsOriginal.Count);

            foreach (var stationOriginal in stationsOriginal) {
                var position = stationOriginal.transform.position + transform.position;
                var station = CreateFromFactory(
                    stationOriginal, m_enemyStationFactory, enemyField, position, Quaternion.identity);
                station.Initialize();
                stations.Add(station);
            }

            return stations;
        }


        private List<Crystal> CreateCrystals (List<Crystal> crystalsOriginal) {
            var crystals = new List<Crystal>(crystalsOriginal.Count);

            foreach (var crystalOriginal in crystalsOriginal) {
                var position = crystalOriginal.transform.position + transform.position;
                var crystal = Create(crystalOriginal, enemyField, position, Quaternion.identity);
                crystal.Initialize();
                crystals.Add(crystal);
            }

            return crystals;
        }

        #endregion



        #region FactoryUpdate

        ///<summary>Время, прошедшее с момента последнего порождения врага</summary>
        private float m_timeOfLastSpawn;

        [Inject]
        private Game m_game;


        private void Update () {
            RemoveDestroyedStations();
            RemoveDestroyedCrystals();
            UpdateEnemies();

            if (AnyStationNotDestroyed()) {
                if (m_enemies.Count < maxEnemiesCount &&
                    m_timeOfLastSpawn + (timeSpawn / m_enemyStations.Count) < Time.time) {
                    m_enemies.Add(SpawnEnemy());
                    m_timeOfLastSpawn = Time.time;
                }
            }
            else if (AllEnemiesDead()) {
                // Уровень завершён только когда все вражеские базы и все враги уничтожены
                Messenger.SendMessage<NextLevelMessage>();
                enabled = false;
            }
        }


        private void RemoveDestroyedStations () {
            for (var i = 0; i < m_enemyStations.Count; i++)
                if (m_enemyStations[i].IsDestroyed)
                    m_enemyStations.RemoveAt(i--);
        }


        private void RemoveDestroyedCrystals () {
            for (var i = 0; i < m_crystals.Count; i++)
                if (m_crystals[i].IsDestroyed)
                    m_crystals.RemoveAt(i--);
        }


        private bool AnyStationNotDestroyed () {
            return m_enemyStations.Count != 0;
        }


        private void UpdateEnemies () {
            for (var i = 0; i < m_enemies.Count; i++)
                if (!m_enemies[i].EnemyUpdate())
                    m_enemies.RemoveAt(i--);
        }


        private bool AllEnemiesDead () {
            return m_enemies.Count == 0;
        }


        private int m_stationIndex;


        private EnemyCharacter SpawnEnemy () {
            m_stationIndex++;
            if (m_stationIndex >= m_enemyStations.Count)
                m_stationIndex = 0;

            return m_enemyStations[m_stationIndex].SpawnEnemy(targetPoints);
        }

        #endregion



        #region TriggerEvents

        private void OnTriggerStay (Collider other) {
            if (!other.CompareTag("Player")) return;

            if (!TransitionsBetweenBases.backTransition.gameObject.activeSelf) {
                TransitionsBetweenBases.backTransition.gameObject.SetActive(true);
                m_game.DestroyOldBase();
            }

            foreach (var enemy in m_enemies)
                enemy.AttackPlayer();
        }


        private void OnTriggerExit (Collider other) {
            if (!other.CompareTag("Player")) return;

            foreach (var enemy in m_enemies)
                enemy.Patrol();
        }

        #endregion



        [Serializable]
        public struct Transitions {

            public Transform backTransition;
            public Transform frontTransition;

        }



        public class Factory : PlaceholderFactory<UnityEngine.Object, EnemyBase> { }

    }

}