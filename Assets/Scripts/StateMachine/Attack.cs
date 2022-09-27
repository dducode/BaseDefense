using UnityEngine;

public class Attack : State
{
    float attackDistance;

    public Attack
    (
        Animator _animator, CharacterController _controller, 
        Transform _agent, Transform _player
    )
    {
        stage = Enter;
        animator = _animator;
        controller = _controller;
        agent = _agent;
        player = _player;
    }
    protected override void Enter()
    {
        animator.SetBool("attack", true);
        EnemyCharacter enemy = agent.GetComponent<EnemyCharacter>();
        attackDistance = enemy.getAttackDistance;
        stage = Update;
    }
    protected override void Update()
    {
        agent.rotation = Quaternion.Slerp(
            agent.rotation, 
            Quaternion.LookRotation(player.position - agent.position), 
            Time.smoothDeltaTime * 15f
        );

        RaycastHit hit;
        int layerMask = 1<<7;
        layerMask = ~layerMask;
        Physics.Raycast(
            agent.position + Vector3.up, 
            player.position - agent.position,
            out hit,
            Mathf.Infinity,
            layerMask);

        if (hit.transform.CompareTag("Player"))
        {   
            if (hit.distance > attackDistance + 0.5f)
            {
                nextState = new Running(animator, controller, agent, player);
                stage = Exit;
            }
        }
        else
        {
            nextState = new Walking(animator, controller, agent, player);
            stage = Exit;
        }
    }
    protected override void Exit()
    {
        animator.SetBool("attack", false);
    }
}
