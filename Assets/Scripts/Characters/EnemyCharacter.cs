using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : BaseCharacter
{
    EnemyBaseContainer enemyBase;
    [SerializeField] float walkingSpeed;
    Transform[] targetPoints;
    State state;
    public float getWalkingSpeed { get { return walkingSpeed; } }
    public float getMaxSpeed { get { return maxSpeed; } }
    public float getAttackDistance { get { return attackDistance; } }
    public Vector3 getPoint { get { return targetPoints[Random.Range(0, targetPoints.Length)].position; } }
    public State getCurrentState { get { return state; } }

    void Start()
    {
        currentHP = maxHealthPoint;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        state = new Walking(animator, controller, transform, FindObjectOfType<PlayerCharacter>().transform);
    }

    void Update()
    {
        state = state.Process();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, attackDistance);
    }

    public override void GetDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            this.enabled = false;
            controller.enabled = false;
            animator.SetBool("alive", false);
            enemyBase.RemoveEnemyFromList(gameObject);
            StartCoroutine(Await());
        }
    }

    public void GetData(EnemyBaseContainer _base, Transform[] points)
    {
        enemyBase = _base;
        targetPoints = points;
    }

    IEnumerator Await()
    {
        yield return new WaitWhile(() => IsDeath());
        GetComponent<ItemDrop>().Drop();
        Destroy(gameObject);
    }
    bool IsDeath()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        return !(info.IsName("Death") && info.normalizedTime >= 0.9f);
    }
}
