using System.Collections.Generic;
using BaseDefense.Characters;
using BaseDefense.Items;
using BaseDefense.Messages;
using BroadcastMessages;
using DG.Tweening;
using SaveSystem;
using SaveSystem.UnityHandlers;
using UnityEngine;
using Zenject;

namespace BaseDefense {

    public class Game : MonoBehaviour, IPersistentObject {

        [SerializeField]
        private EnemyBase basePrefab;

        [SerializeField]
        private bool saving;

        [SerializeField]
        private BaseTemplate[] baseTemplates;

        public const string MoneyPath = "Prefabs/Items/Money";
        public const string GemPath = "Prefabs/Items/Gem";

        private EnemyBase.Factory m_enemyFactory;
        private List<EnemyBase> m_bases;
        private int m_currentLevel;
        private PlayerCharacter m_playerCharacter;
        private readonly Vector3 m_initialPosition = new(0, 0, 20);
        private const int FirstBase = 0;


        [Inject]
        public void Constructor (EnemyBase.Factory enemyFactory, PlayerCharacter playerCharacter) {
            m_enemyFactory = enemyFactory;
            m_playerCharacter = playerCharacter;
        }


        public void DestroyOldBase () {
            m_bases[FirstBase].Destroy();
            m_bases.RemoveAt(FirstBase);
            Messenger.SendMessage<DestroyUnusedItemsMessage>();
        }


        private void Awake () {
            m_bases = new List<EnemyBase>();
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            DOTween.SetTweensCapacity(300, 150);
            Messenger.SubscribeTo<NextLevelMessage>(NextLevel);

            if (saving) {
                Application.quitting += () => {
                    DataManager.SaveObjects(DataFileName, new IPersistentObject[] {this, m_playerCharacter});
                };
            }
        }


        private const string DataFileName = "saveData.bytes";


        private void OnApplicationPause (bool pauseStatus) {
            if (saving && pauseStatus)
                DataManager.SaveObjects(DataFileName, new IPersistentObject[] {this, m_playerCharacter});
        }


        private void Start () {
            if (!DataManager.LoadObjects(DataFileName, new IPersistentObject[] {this, m_playerCharacter})) {
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

            var frontTransition = m_bases[FirstBase].TransitionsBetweenBases.frontTransition;
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
            SaveBases(writer);
            SaveItems(writer);
        }


        public void Load (UnityReader reader) {
            m_currentLevel = reader.ReadInt();
            LoadBases(reader);
            LoadItems(reader);
        }


        private void SaveBases (UnityWriter writer) {
            writer.Write(m_bases.Count);
            foreach (var enemyBase in m_bases)
                enemyBase.Save(writer);
        }


        private void SaveItems (UnityWriter writer) {
            var moneys = FindObjectsByType<Money>(FindObjectsSortMode.None);
            var moneysList = new List<Money>();

            foreach (var money in moneys)
                if (money.transform.parent is null)
                    moneysList.Add(money);

            writer.Write(moneysList.Count);
            foreach (var money in moneysList)
                money.Save(writer);

            var gems = FindObjectsByType<Gem>(FindObjectsSortMode.None);

            writer.Write(gems.Length);
            foreach (var gem in gems)
                gem.Save(writer);
        }


        private void LoadBases (UnityReader reader) {
            var basesCount = reader.ReadInt();

            for (var i = 0; i < basesCount; i++) {
                var enemyBase = Object.CreateFromFactory(basePrefab, m_enemyFactory);
                enemyBase.Load(reader);
                m_bases.Add(enemyBase);
            }
        }


        private void LoadItems (UnityReader reader) {
            var moneysCount = reader.ReadInt();

            for (var i = 0; i < moneysCount; i++) {
                var money = Object.Create(Resources.Load<Money>(MoneyPath));
                money.Load(reader);
            }

            var gemsCount = reader.ReadInt();

            for (var i = 0; i < gemsCount; i++) {
                var gem = Object.Create(Resources.Load<Gem>(GemPath));
                gem.Load(reader);
            }
        }

    }

}