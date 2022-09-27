using UnityEngine;

public class Running : State
{
    float speed;
    float attackDistance;

    public Running
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
        animator.SetBool("running", true);
        EnemyCharacter enemy = agent.GetComponent<EnemyCharacter>();
        speed = enemy.getMaxSpeed;
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
        controller.Move(agent.forward * speed * Time.smoothDeltaTime);
        
        RaycastHit hit;
        int layerMask = 1<<7;
        layerMask = ~layerMask;
        Physics.Raycast(
            agent.position + Vector3.up, 
            player.position - agent.position,
            out hit,
            Mathf.Infinity,
            layerMask);

        if (!attackTrigger)
        {
            nextState = new Walking(animator, controller, agent, player);
            stage = Exit;
        }
        if (hit.transform.CompareTag("Player"))
        {
            if (hit.distance < attackDistance)
            {
                nextState = new Attack(animator, controller, agent, player);
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
        animator.SetBool("running", false);
    }
}
