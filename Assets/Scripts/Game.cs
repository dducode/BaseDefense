using UnityEngine;
using System.Collections.Generic;
using System.IO;
using BaseDefense.BroadcastMessages;
using BaseDefense.BroadcastMessages.Messages;
using BaseDefense.Characters;
using BaseDefense.SaveSystem;
using Zenject;
using DG.Tweening;
using UnityEngine.Assertions;

namespace BaseDefense {

    public class Game : MonoBehaviour {

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
                Application.quitting += SaveGame;
                Application.quitting += UpgradablePropertyContainer.Save;
            }
        }


        private void OnApplicationPause (bool pauseStatus) {
            if (saving && pauseStatus) {
                SaveGame();
                UpgradablePropertyContainer.Save();
            }
        }


        private void Start () {
            if (!LoadGame()) {
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


        private const string DATA_FILE_NAME = "saveData.dat";


        private void SaveGame () {
            var path = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
            using var binaryWriter = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));
            var writer = new GameDataWriter(binaryWriter);
            writer.Write(m_currentLevel);
            m_playerCharacter.Save(writer);
            SaveBase(writer);
        }


        private void SaveBase (GameDataWriter writer) {
            writer.Write(m_bases.Count);
            foreach (var enemyBase in m_bases)
                enemyBase.Save(writer);
        }


        private bool LoadGame () {
            var path = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
            if (!File.Exists(path))
                return false;

            var binaryData = File.ReadAllBytes(path);
            using var binaryReader = new BinaryReader(new MemoryStream(binaryData));
            var reader = new GameDataReader(binaryReader);

            m_currentLevel = reader.ReadInteger();
            m_playerCharacter.Load(reader);
            LoadBase(reader);

            return true;
        }


        private void LoadBase (GameDataReader reader) {
            var basesCount = reader.ReadInteger();

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