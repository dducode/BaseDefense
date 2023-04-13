using UnityEngine;
using System.Collections.Generic;
using BaseDefense.Characters;
using BaseDefense.Messages;
using BroadcastMessages;
using Zenject;
using DG.Tweening;
using SaveSystem;
using UnityEngine.Assertions;

namespace BaseDefense {

    public class Game : MonoBehaviour, IPersistentObject {

        [SerializeField]
        private EnemyBase basePrefab;

        [SerializeField]
        private bool saving;

        [SerializeField]
        private BaseTemplate[] baseTemplates;

        private EnemyBase.Factory m_enemyFactory;
        private List<EnemyBase> m_bases;
        private int m_currentLevel;
        private PlayerCharacter m_playerCharacter;
        private readonly Vector3 m_initialPosition = new(0, 0, 20);
        private const int FIRST_BASE = 0;


        [Inject]
        public void Constructor (EnemyBase.Factory enemyFactory, PlayerCharacter playerCharacter) {
            m_enemyFactory = enemyFactory;
            m_playerCharacter = playerCharacter;
        }


        public void DestroyOldBase () {
            m_bases[FIRST_BASE].Destroy();
            m_bases.RemoveAt(FIRST_BASE);
            Messenger.SendMessage<DestroyUnusedItemsMessage>();
        }


        private void Awake () {
            m_bases = new List<EnemyBase>();
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            DOTween.SetTweensCapacity(300, 150);
            Messenger.SubscribeTo<NextLevelMessage>(NextLevel);

            if (saving) {
                Application.quitting += () => {
                    DataManager.SaveObjects(DATA_FILE_NAME, this);
                };
            }
        }


        private const string DATA_FILE_NAME = "saveData";


        private void OnApplicationPause (bool pauseStatus) {
            if (saving && pauseStatus) 
                DataManager.SaveObjects(DATA_FILE_NAME, this);
        }


        private void Start () {
            if (!DataManager.LoadObjects(DATA_FILE_NAME, this)) {
                var newBase = CreateNewBase();
                newBase.transform.position = m_initialPosition;
                newBase.Initialize(baseTemplates[m_currentLevel]);
                m_bases.Add(newBase);
            }
        }


        private void OnDestroy () {
            Messenger.UnsubscribeFrom<NextLevelMessage>(NextLevel);
        }


        private void NextLevel () {
            m_currentLevel++;
            if (m_currentLevel == baseTemplates.Length)
                m_currentLevel = 0;

            var newBase = CreateNewBase();

            var backTransition = newBase.TransitionsBetweenBases.backTransition;
            backTransition.gameObject.SetActive(false);

            var frontTransition = m_bases[FIRST_BASE].TransitionsBetweenBases.frontTransition;
            frontTransition.gameObject.SetActive(false);

            newBase.transform.position = frontTransition.position - backTransition.localPosition;
            newBase.Initialize(baseTemplates[m_currentLevel]);
            m_bases.Add(newBase);
        }


        private EnemyBase CreateNewBase () {
            var newBase = Object.CreateFromFactory(basePrefab, m_enemyFactory);
            newBase.TransitionsBetweenBases.frontTransition.gameObject.SetActive(true);
            newBase.enabled = true;
            return newBase;
        }


        public void Save (UnityWriter writer) {
            writer.Write(m_currentLevel);
            m_playerCharacter.Save(writer);
            SaveBase(writer);
        }


        private void SaveBase (UnityWriter writer) {
            writer.Write(m_bases.Count);
            foreach (var enemyBase in m_bases)
                enemyBase.Save(writer);
        }


        public void Load (UnityReader reader) {
            m_currentLevel = reader.ReadInt();
            m_playerCharacter.Load(reader);
            LoadBase(reader);
        }


        private void LoadBase (UnityReader reader) {
            var basesCount = reader.ReadInt();

            for (var i = 0; i < basesCount; i++) {
                var enemyBase = Object.CreateFromFactory(basePrefab, m_enemyFactory);
                const string message = "Не удалось загрузить базу";
                Assert.IsNotNull(enemyBase, message);
                enemyBase.Load(reader);
                m_bases.Add(enemyBase);
            }
        }

    }

}