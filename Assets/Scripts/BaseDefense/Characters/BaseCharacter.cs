using BaseDefense.AttackImplemention;
using UnityEngine;

namespace BaseDefense.Characters
{
    ///<summary>Базовый класс для всех типов персонажей</summary>
    [Icon("Assets/EditorUI/ghost.png")]
    [RequireComponent(typeof(CharacterController), 
        typeof(Animator), 
        typeof(ParticleSystem))]
    public abstract class BaseCharacter : Object, IAttackable
    {
        [Tooltip("Отображает в сцене радиус атаки персонажа")]
        [SerializeField] Color gizmosView = Color.white;

        ///<summary>Максимально возможное количество очков здоровья для данного персонажа</summary>
        ///<value>[1, infinity]</value>
        [Header("Общие характеристики персонажа")]
        [Tooltip("Максимально возможное количество очков здоровья для данного персонажа. [1, infinity]")] 
        [SerializeField, Min(1)] protected float maxHealthPoints = 100;

        ///<summary>Цвет, который персонаж принимает после смерти</summary>
        [Tooltip("Цвет, который персонаж принимает после смерти")]
        [SerializeField] protected Color deathColor;

        ///<summary>Максимально развиваемая персонажем скорость</summary>
        ///<value>[0, infinity]</value>
        [Tooltip("Максимально развиваемая персонажем скорость. [0, infinity]")] 
        [SerializeField, Min(0)] protected float maxSpeed = 5;

        ///<summary>Расстояние, с которого персонаж начинает атаковать</summary>
        ///<value>[0, infinity]</value>
        [Tooltip("Расстояние, с которого персонаж начинает атаковать. [0, infinity]")] 
        [SerializeField, Min(0)] protected float attackDistance;

        ///<summary>Анимация, проигрываемая при получении урона персонажем</summary>
        public ParticleSystem HitEffect { get; protected set; }

        ///<summary>Стандартный цвет персонажа. Выставляется при респавне вместо deathColor</summary>
        public Color DefaultColor { get; protected set; }

        public CharacterController Controller { get; protected set; }
        public Animator Animator { get; protected set; }
        public SkinnedMeshRenderer MeshRenderer { get; protected set; }

        ///<summary>При включении поведения персонажа также включается его контроллёр</summary>
        private bool m_enabledCharacter;
        ///<inheritdoc cref="m_enabledCharacter"/>
        public new bool enabled
        {
            get => m_enabledCharacter;
            set
            {
                m_enabledCharacter = value;
                base.enabled = m_enabledCharacter;
                Controller.enabled = m_enabledCharacter;
            }
        }

        ///<summary>Текущее количество здоровья персонажа</summary>
        ///<value>[0, maxHealthPoints]</value>
        float m_currentHealthPoints;
        ///<inheritdoc cref="m_currentHealthPoints"/>
        public float CurrentHealthPoints
        {
            get => m_currentHealthPoints;
            protected set
            {
                m_currentHealthPoints = value;
                m_currentHealthPoints = Mathf.Clamp(m_currentHealthPoints, 0, maxHealthPoints);
                if (!IsAlive)
                    OnDeath();
            }
        }

        ///<summary>Состояние персонажа "жив/мёртв"</summary>
        ///<returns>Возвращает true, если текущий показатель здоровья больше 0, иначе false</returns>
        public bool IsAlive => m_currentHealthPoints > 0;

        ///<summary>Вызывается для нанесения урона персонажу</summary>
        ///<param name ="damage">Количество нанесённого урона</param>
        public abstract void Hit(float damage);

        ///<summary>Вызывается автоматически, когда персонаж мёртв</summary>
        protected abstract void OnDeath();

        protected override void Awake()
        {
            base.Awake();
            Controller = GetComponent<CharacterController>();
            Animator = GetComponent<Animator>();
            HitEffect = GetComponent<ParticleSystem>();
            MeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            CurrentHealthPoints = maxHealthPoints;
            DefaultColor = MeshRenderer.material.color;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = gizmosView;
            Gizmos.DrawWireSphere(transform.position + (Vector3.up * transform.localScale.y), attackDistance);
        }
    }
}


