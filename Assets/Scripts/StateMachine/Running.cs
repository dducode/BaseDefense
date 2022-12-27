using UnityEngine;

public class Running : State
{
    float speed;
    float attackDistance;

    public Running
    (
        Animator animator, CharacterController controller, 
        EnemyCharacter agent, Transform player
    )
    {
        stage = Enter;
        this.animator = animator;
        this.controller = controller;
        this.agent = agent;
        this.player = player;
        speed = agent.getMaxSpeed;
        attackDistance = agent.getAttackDistance;
        transform = agent.transform;
    }
    protected override void Enter()
    {
        animator.SetBool("running", true);
        stage = Update;
    }
    protected override void Update()
    {
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            Quaternion.LookRotation(player.position - transform.position), 
            Time.smoothDeltaTime * 15f
        );
        controller.Move(transform.forward * speed * Time.smoothDeltaTime);
        
        RaycastHit hit;
        int layerMask = 1<<6;
        Physics.Raycast(
            transform.position + (Vector3.up * agent.transform.localScale.y), 
            player.position - transform.position,
            out hit,
            Mathf.Infinity,
            layerMask);

        if (!attackTrigger)
        {
            nextState = new Walking(animator, controller, agent, player);
            stage = Exit;
        }
        if (hit.transform && hit.transform.CompareTag("Player"))
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
