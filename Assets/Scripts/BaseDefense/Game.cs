using UnityEngine;
using System.Collections.Generic;
using Zenject;
using BroadcastMessages;

namespace BaseDefense
{
    public class Game : MonoBehaviour
    {
        [SerializeField] EnemyFactory[] basePrefabs;
        [Inject] EnemyFactory.Factory enemyFactory;
        List<EnemyFactory> bases;
        int currentLevel;

        void Awake()
        {
            // currentLevel = Load();
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            bases = new List<EnemyFactory>();
            EnemyFactory enemyBase = FindObjectOfType<EnemyFactory>();
            if (enemyBase == null)
            {
                enemyBase = Instantiate(basePrefabs[currentLevel], new Vector3(0, 0, 20), Quaternion.identity);
                enemyBase.name = basePrefabs[currentLevel].name;
            }
            bases.Add(enemyBase);
        }

        public void NextLevel()
        {
            currentLevel++;
            if (currentLevel == basePrefabs.Length)
                currentLevel = 0;
            Transform backTransition;
            Transform frontTransition;
            EnemyFactory newBase = enemyFactory.Create(basePrefabs[currentLevel]);
            newBase.name = basePrefabs[currentLevel].name;

            backTransition = newBase.transitions.backTransition;
            backTransition.gameObject.SetActive(false);

            frontTransition = bases[0].transitions.frontTransition;
            frontTransition.gameObject.SetActive(false);

            newBase.transform.position = frontTransition.position - backTransition.localPosition;
            bases.Add(newBase);
        }

        public void DestroyOldBase()
        {
            Destroy(bases[0].gameObject);
            bases.RemoveAt(0);
            Messenger.SendMessage(MessageType.PUSH_UNUSED_ITEMS);
        }
    }
}