using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour
{
    [SerializeField] protected float maxHealthPoint;
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected float attackDistance;
    protected CharacterController controller;
    protected Animator animator;
    protected float currentHP;
    protected bool alive = true;
    public abstract void GetDamage(float damage);
}
