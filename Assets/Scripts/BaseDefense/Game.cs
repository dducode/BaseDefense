using UnityEngine;
using System.Collections.Generic;
using BaseDefense.Broadcast_messages;
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

        private void Awake()
        {
            // currentLevel = Load();
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            DOTween.SetTweensCapacity(200, 100);
            m_bases = new List<EnemyFactory>();
            var enemyBase = FindObjectOfType<EnemyFactory>();
            if (enemyBase == null)
            {
                var initialPosition = new Vector3(0, 0, 20);
                enemyBase = Instantiate(basePrefabs[m_currentLevel], initialPosition, Quaternion.identity);
                enemyBase.name = basePrefabs[m_currentLevel].name;
            }
            m_bases.Add(enemyBase);
        }

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
    }
}