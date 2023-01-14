using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>Базовый класс для всех типов персонажей</summary>
[RequireComponent(typeof(CharacterController), typeof(Animator), typeof(ParticleSystem))]
public abstract class BaseCharacter : MonoBehaviour
{
    [Tooltip("Отображает в сцене радиус атаки персонажа")]
    [SerializeField] Color gizmosView = Color.white;

    ///<summary>Максимально возможное количество очков здоровья для данного персонажа</summary>
    ///<value>[1, infinity]</value>
    [Header("Общие характеристики персонажа")]
    [Tooltip("Максимально возможное количество очков здоровья для данного персонажа. [1, infinity]")] 
    [SerializeField, Min(1)] protected float maxHealthPoints = 100;

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

    public CharacterController Controller { get; protected set; }
    public Animator Animator { get; protected set; }

    ///<summary>Текущее количество здоровья персонажа</summary>
    ///<value>[0, maxHealthPoints]</value>
    float currentHealthPoints;
    ///<inheritdoc cref="currentHealthPoints"/>
    public float CurrentHealthPoints
    {
        get { return currentHealthPoints; }
        protected set
        {
            currentHealthPoints = value;
            currentHealthPoints = Mathf.Clamp(currentHealthPoints, 0, maxHealthPoints);
        }
    }

    ///<summary>Состояние персонажа "жив/мёртв"</summary>
    ///<returns>Возвращает true, если текущий показатель здоровья больше 0, иначе false</returns>
    public bool IsAlive { get { return currentHealthPoints > 0; } }

    ///<summary>Вызывается для нанесения урона персонажу</summary>
    ///<param name="damage">Количество урона, наносимого персонажу</param>
    public abstract void Hit(float damage);

    public virtual void Awake()
    {
        Controller = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        HitEffect = GetComponent<ParticleSystem>();
        CurrentHealthPoints = maxHealthPoints;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmosView;
        Gizmos.DrawWireSphere(transform.position + (Vector3.up * transform.localScale.y), attackDistance);
    }
}
