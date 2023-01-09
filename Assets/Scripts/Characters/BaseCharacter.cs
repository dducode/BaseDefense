using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>Базовый класс для всех типов персонажей</summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public abstract class BaseCharacter : MonoBehaviour
{
    ///<summary>
    ///Максимально возможное количество очков здоровья для данного персонажа. [1, infinity]
    ///</summary>
    [Header("Общие характеристики персонажа")]
    [Tooltip("Максимально возможное количество очков здоровья для данного персонажа. [1, infinity]")] 
    [SerializeField, Min(1)]
    protected float maxHealthPoints = 100;

    ///<summary>Максимально развиваемая персонажем скорость. [0, infinity]</summary>
    [Tooltip("Максимально развиваемая персонажем скорость. [0, infinity]")] 
    [SerializeField, Min(0)]
    protected float maxSpeed = 5;

    ///<summary>Расстояние, с которого персонаж начинает атаковать. [0, infinity]</summary>
    [Tooltip("Расстояние, с которого персонаж начинает атаковать. [0, infinity]")] 
    [SerializeField, Min(0)]
    protected float attackDistance;

    public CharacterController Controller { get; protected set; }
    public Animator Animator { get; protected set; }

    ///<summary>Текущее количество здоровья персонажа. [0, maxHealthPoints]</summary>
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
        CurrentHealthPoints = maxHealthPoints;
    }
}
