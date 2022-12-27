using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyCharacter : BaseCharacter
{
    [Header("Характеристики врага")]
    [SerializeField] float walkingSpeed;
    [Header("Связанные объекты")]
    [SerializeField, Tooltip("Коллайдер атакующей руки врага")] 
    Collider hand;
    EnemyBaseContainer enemyBase;
    Transform[] targetPoints;
    State state;
    public float getWalkingSpeed { get { return walkingSpeed; } }
    public float getMaxSpeed { get { return maxSpeed; } }
    public float getAttackDistance { get { return attackDistance; } }
    public Vector3 getPoint { get { return targetPoints[Random.Range(0, targetPoints.Length)].position; } }
    public State getCurrentState { get { return state; } }

    [Inject]
    public void Initialize(EnemyBaseContainer enemyBase, Transform[] targetPoints, PlayerCharacter player)
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        this.enemyBase = enemyBase;
        this.targetPoints = targetPoints;
        state = new Walking(animator, controller, this, player.transform);
    }

    public void Spawn(Vector3 position, Quaternion rotation)
    {
        currentHP = maxHealthPoint;
        alive = true;
        controller.enabled = false;
        transform.SetLocalPositionAndRotation(position, rotation);
        controller.enabled = true;
    }

    public bool EnemyUpdate()
    {
        if (alive)
        {
            state = state.Process();
            hand.enabled = state is Attack;
        }
        return alive;
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
        ObjectsPool<EnemyCharacter>.Push(this);
    }
    bool IsDeath()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        return !(info.IsName("Death") && info.normalizedTime >= 0.9f);
    }

    public class Factory : PlaceholderFactory<Transform[], EnemyCharacter> {}
}
