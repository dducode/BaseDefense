using BaseDefense.AttackImplemention;
using UnityEngine;
using Zenject;
using BaseDefense.UI;
using BaseDefense.Characters;
using BaseDefense.Items;
using SaveSystem.UnityHandlers;

namespace BaseDefense {

    [RequireComponent(typeof(DisplayHealthPoints))]
    public class EnemyStation : Object, IAttackable {

        ///<summary>Максимальное количество здоровья станции</summary>
        ///<value>[1, infinity]</value>
        [Tooltip("Максимальное количество здоровья станции. [1, infinity]")]
        [SerializeField, Min(1)]
        private float maxHealthPoints = 300;

        ///<summary>Анимация, воспроизводимая при уничтожении станции</summary>
        [Tooltip("Анимация, воспроизводимая при уничтожении станции")]
        [SerializeField]
        private ParticleSystem destroyEffect;

        ///<summary>Максимальное расстояние от центра спавна для порождения новых врагов</summary>
        ///<value>[0, infinity]</value>
        [Tooltip("Максимальное расстояние от центра спавна для порождения новых врагов. [0, infinity]")]
        [SerializeField, Min(0)]
        private float radiusSpawn = 10f;

        [SerializeField]
        private EnemyCharacter enemyPrefab;

        [SerializeField]
        private Transform[] spawnPoints;

        [Inject]
        private EnemyCharacter.Factory m_enemyFactory;

        private ItemDrop[] m_itemDropComponents;


        public override void Save (UnityWriter writer) {
            base.Save(writer);
            writer.Write(CurrentHealthPoints);
        }


        public override void Load (UnityReader reader) {
            base.Load(reader);
            CurrentHealthPoints = reader.ReadFloat();
        }


        public EnemyCharacter SpawnEnemy (Transform[] targetPoints) {
            var index = Random.Range(0, spawnPoints.Length - 1);
            var position = spawnPoints[index].position + Random.insideUnitSphere * radiusSpawn;
            position.y = 0;
            var rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
            var enemy = CreateFromFactory(enemyPrefab, m_enemyFactory);
            enemy.Initialize(targetPoints, position, rotation);

            return enemy;
        }


        private DisplayHealthPoints m_displayHealthPoints;

        ///<summary>Текущее количество здоровья базы</summary>
        ///<value>[0, maxHealthPoints]</value>
        private float m_currentHealthPoints;

        ///<inheritdoc cref="m_currentHealthPoints"/>
        public float CurrentHealthPoints {
            get => m_currentHealthPoints;
            private set {
                m_currentHealthPoints = value;
                m_currentHealthPoints = Mathf.Clamp(m_currentHealthPoints, 0, maxHealthPoints);
                if (m_currentHealthPoints == 0)
                    DestroyStation();
            }
        }


        public void Initialize () {
            CurrentHealthPoints = maxHealthPoints;
            UpdateHealthPointsView();
        }


        public void Hit (float damage) {
            CurrentHealthPoints -= damage;
            UpdateHealthPointsView();
        }


        protected override void Awake () {
            base.Awake();
            CurrentHealthPoints = maxHealthPoints;
            m_displayHealthPoints = GetComponent<DisplayHealthPoints>();
            m_itemDropComponents = GetComponents<ItemDrop>();
        }


        private void Start () {
            m_displayHealthPoints.SetMaxValue((int) maxHealthPoints);
            UpdateHealthPointsView();
        }


        private void UpdateHealthPointsView () {
            m_displayHealthPoints.UpdateView((int) CurrentHealthPoints);
        }


        private void OnDrawGizmosSelected () {
            if (spawnPoints == null || spawnPoints.Length == 0)
                return;
            Gizmos.color = Color.green;
            foreach (var spawnPoint in spawnPoints)
                Gizmos.DrawWireSphere(spawnPoint.position, radiusSpawn);
        }


        private void DestroyStation () {
            if (IsDestroyed)
                return;

            foreach (var itemDropComponent in m_itemDropComponents)
                itemDropComponent.DropItems();
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
            Destroy();
        }


        public class Factory : PlaceholderFactory<UnityEngine.Object, EnemyStation> { }

    }

}