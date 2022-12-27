using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public abstract class BaseCharacter : MonoBehaviour
{
    [Header("Общие характеристики персонажа")]
    [SerializeField] protected float maxHealthPoint;
    [SerializeField] protected float maxSpeed;
    [SerializeField, Tooltip("Расстояние, с которого персонаж начинает атаковать")] 
    protected float attackDistance;
    protected CharacterController controller;
    protected Animator animator;
    protected float currentHP;
    protected bool alive;
    public abstract void GetDamage(float damage);

    public virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        currentHP = maxHealthPoint;
        alive = true;
    }
}
