using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : BaseCharacter
{
    EnemyBaseContainer enemyBase;
    [SerializeField] float walkingSpeed;
    [SerializeField] Collider hand;
    Transform[] targetPoints;
    State state;
    public float getWalkingSpeed { get { return walkingSpeed; } }
    public float getMaxSpeed { get { return maxSpeed; } }
    public float getAttackDistance { get { return attackDistance; } }
    public Vector3 getPoint { get { return targetPoints[Random.Range(0, targetPoints.Length)].position; } }
    public State getCurrentState { get { return state; } }

    public void Initialize(EnemyBaseContainer enemyBase, Transform[] targetPoints)
    {
        gameObject.SetActive(true);
        this.enemyBase = enemyBase;
        this.targetPoints = targetPoints;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        controller.enabled = true;
        alive = true;
        currentHP = maxHealthPoint;
        state = new Walking(animator, controller, transform, Game.Player.transform);
    }

    public bool EnemyUpdate()
    {
        if (alive)
        {
            state = state.Process();
            hand.enabled = state is Attack;
            return true;
        }
        else return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + (Vector3.up * transform.localScale.y), attackDistance);
    }

    public override void GetDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            controller.enabled = false;
            animator.SetBool("alive", false);
            alive = false;
            StartCoroutine(Await());
        }
    }

    IEnumerator Await()
    {
        yield return new WaitWhile(() => IsDeath());
        GetComponent<ItemDrop>().DropItems();
        Pools.Push(this);
        gameObject.SetActive(false);
    }
    bool IsDeath()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        return !(info.IsName("Death") && info.normalizedTime >= 0.9f);
    }
}
