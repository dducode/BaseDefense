using UnityEngine;
using System;

public class StateMachine
{
    Action stage;
    EnemyCharacter agent;
    Animator animator;
    CharacterController controller;
    Transform player;
    Transform transform;
    float walkingSpeed;
    float maxSpeed;
    Vector3 targetPoint;
    float attackDistance;
    bool attackTrigger;

    public StateMachine(EnemyCharacter agent, Transform player)
    {
        this.agent = agent;
        this.player = player;
        animator = agent.Animator;
        controller = agent.Controller;
        transform = agent.transform;
        walkingSpeed = agent.WalkingSpeed;
        maxSpeed = agent.MaxSpeed;
        attackDistance = agent.AttackDistance;
        stage = Walking;
    }

    public Action Stage => stage;

    public bool AttackTrigger
    {
        get => attackTrigger;
        set
        {
            attackTrigger = value;
            if (attackTrigger)
            {
                animator.SetBool("walking", false);
                stage = Running;
            }
            else
            {
                animator.SetBool("running", false);
                stage = Walking;
            }
        }
    }

    void Walking()
    {
        animator.SetBool("walking", true);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            Quaternion.LookRotation(targetPoint - transform.position), 
            Time.smoothDeltaTime * 15f
        );
        controller.Move(transform.forward * walkingSpeed * Time.smoothDeltaTime);
    }
    void Running()
    {
        animator.SetBool("running", true);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            Quaternion.LookRotation(player.position - transform.position), 
            Time.smoothDeltaTime * 15f
        );
        controller.Move(transform.forward * maxSpeed * Time.smoothDeltaTime);
        
        RaycastHit hit;
        int layerMask = 1<<6;
        Physics.Raycast(
            transform.position + (Vector3.up * agent.transform.localScale.y), 
            player.position - transform.position,
            out hit,
            Mathf.Infinity,
            layerMask);

        if (hit.transform && hit.transform.CompareTag("Player"))
        {
            if (hit.distance < attackDistance)
            {
                animator.SetBool("running", false);
                stage = Attack;
            }
        }
        else
        {
            animator.SetBool("running", false);
            stage = Walking;
        }
    }
    void Attack()
    {
        animator.SetBool("attack", true);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            Quaternion.LookRotation(player.position - transform.position), 
            Time.smoothDeltaTime * 15f
        );

        RaycastHit hit;
        int layerMask = 1<<6;
        Physics.Raycast(
            transform.position + (Vector3.up * agent.transform.localScale.y), 
            player.position - transform.position,
            out hit,
            Mathf.Infinity,
            layerMask);

        if (hit.transform && hit.transform.CompareTag("Player"))
        {   
            if (hit.distance > attackDistance + 0.5f)
            {
                animator.SetBool("attack", false);
                stage = Running;
            }
        }
        else
        {
            animator.SetBool("attack", true);
            stage = Walking;
        }
    }
}
