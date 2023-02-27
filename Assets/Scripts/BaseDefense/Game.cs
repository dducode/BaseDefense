using UnityEngine;
using System.Collections.Generic;
using System.IO;
using BaseDefense.Broadcast_messages;
using BaseDefense.SaveSystem;
using Zenject;
using BroadcastMessages;
using DG.Tweening;

namespace BaseDefense
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private EnemyFactory[] basePrefabs;
        [Inject] private EnemyFactory.Factory m_enemyFactory;
        private List<EnemyFactory> m_bases;
        private int m_currentLevel;
        private const int FIRST_BASE = 0;

        public void NextLevel()
        {
            m_currentLevel++;
            if (m_currentLevel == basePrefabs.Length)
                m_currentLevel = 0;

            var newBase = m_enemyFactory.Create(basePrefabs[m_currentLevel]);
            newBase.name = basePrefabs[m_currentLevel].name;

            var backTransition = newBase.transitions.backTransition;
            backTransition.gameObject.SetActive(false);
            
            var frontTransition = m_bases[FIRST_BASE].transitions.frontTransition;
            frontTransition.gameObject.SetActive(false);

            newBase.transform.position = frontTransition.position - backTransition.localPosition;
            m_bases.Add(newBase);
        }

        public void DestroyOldBase()
        {
            Destroy(m_bases[FIRST_BASE].gameObject);
            m_bases.RemoveAt(FIRST_BASE);
            Messenger.SendMessage(MessageType.PUSH_UNUSED_ITEMS);
        }
        
        private void Awake()
        {
            Load();
            
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            DOTween.SetTweensCapacity(300, 150);
            m_bases = new List<EnemyFactory>();
            var enemyBase = FindObjectOfType<EnemyFactory>();
            if (enemyBase is not null)
                Destroy(enemyBase);
            
            var initialPosition = new Vector3(0, 0, 20);
            enemyBase = m_enemyFactory.Create(basePrefabs[m_currentLevel]);
            enemyBase.transform.SetPositionAndRotation(initialPosition, Quaternion.identity);
            enemyBase.name = basePrefabs[m_currentLevel].name;
            m_bases.Add(enemyBase);

            Application.quitting += Save;
        }
        
        private const string DATA_FILE_NAME = "saveData.dat";

        private void Save()
        {
            var path = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
            using var writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));
            writer.Write(m_currentLevel);
        }

        private void Load()
        {
            var path = Path.Combine(Application.persistentDataPath, DATA_FILE_NAME);
            if (!File.Exists(path))
                return;
            
            var data = File.ReadAllBytes(path);
            var binaryReader = new BinaryReader(new MemoryStream(data));
            var reader = new GameDataReader(binaryReader);
            
            m_currentLevel = reader.ReadInteger();
        }
    }
}