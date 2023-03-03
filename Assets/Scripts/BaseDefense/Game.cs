using UnityEngine;
using System.Collections.Generic;
using System.IO;
using BaseDefense.BroadcastMessages;
using BaseDefense.SaveSystem;
using Zenject;
using DG.Tweening;
using UnityEngine.Assertions;

namespace BaseDefense
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private EnemyBase basePrefab;
        [SerializeField] private bool saving;
        [SerializeField] private BaseTemplate[] baseTemplates;
        [Inject] private EnemyBase.Factory m_enemyFactory;
        private List<EnemyBase> m_bases;
        private int m_currentLevel;
        private Vector3 m_initialPosition = new(0, 0, 20);
        private const int FIRST_BASE = 0;

        public void DestroyOldBase()
        {
            m_bases[FIRST_BASE].Destroy();
            m_bases.RemoveAt(FIRST_BASE);
            Messenger.SendMessage(MessageType.PUSH_UNUSED_ITEMS);
        }

        private void Awake()
        {
            m_bases = new List<EnemyBase>();
            LoadGame();
            if (m_bases.Count == 0)
                m_bases.Add(CreateNewBase());

            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            DOTween.SetTweensCapacity(300, 150);
            
            if (saving)
                Application.wantsToQuit += () =>
                {
                    SaveGame();
                    return true;
                };
            
            Messenger.AddListener(MessageType.NEXT_LEVEL, NextLevel);
        }

        private void NextLevel()
        {
            m_currentLevel++;
            if (m_currentLevel == baseTemplates.Length)
                m_currentLevel = 0;

            var newBase = CreateNewBase();

            var backTransition = newBase.transitions.backTransition;
            backTransition.gameObject.SetActive(false);

            var frontTransition = m_bases[FIRST_BASE].transitions.frontTransition;
            frontTransition.gameObject.SetActive(false);

            newBase.transform.position = frontTransition.position - backTransition.localPosition;
            m_bases.Add(newBase);
        }

        private EnemyBase CreateNewBase()
        {
            var newBase = Object.CreateFromFactory(basePrefab, m_enemyFactory) as EnemyBase;
            const string message = "Не удалось создать базу";
            Assert.IsNotNull(newBase, message);
            newBase.Initialize(baseTemplates[m_currentLevel]);
            newBase.transform.position = m_initialPosition;
            return newBase;
        }

        private const string DATA_FILE_NAME = "saveData.dat";

        private void SaveGame()
        {
            var path = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
            using var binaryWriter = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));
            var writer = new GameDataWriter(binaryWriter);
            writer.Write(m_currentLevel);
            SaveBase(writer);
        }

        private void SaveBase(GameDataWriter writer)
        {
            writer.Write(m_bases.Count);
            foreach (var enemyBase in m_bases)
                enemyBase.Save(writer);
        }

        private void LoadGame()
        {
            var path = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
            if (!File.Exists(path))
                return;

            var data = File.ReadAllBytes(path);
            var binaryReader = new BinaryReader(new MemoryStream(data));
            var reader = new GameDataReader(binaryReader);

            m_currentLevel = reader.ReadInteger();
            LoadBase(reader);
        }

        private void LoadBase(GameDataReader reader)
        {
            var basesCount = reader.ReadInteger();
            for (int i = 0; i < basesCount; i++)
            {
                var enemyBase = Object.CreateFromFactory(basePrefab, m_enemyFactory) as EnemyBase;
                const string message = "Не удалось загрузить базу";
                Assert.IsNotNull(enemyBase, message);
                enemyBase.Load(reader);
                m_bases.Add(enemyBase);
            }
        }
    }
}