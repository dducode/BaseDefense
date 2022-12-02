using UnityEngine;

public class Running : State
{
    float speed;
    float attackDistance;

    public Running
    (
        Animator animator, CharacterController controller, 
        Transform agent, Transform player
    )
    {
        stage = Enter;
        this.animator = animator;
        this.controller = controller;
        this.agent = agent;
        this.player = player;
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
        int layerMask = 1<<6;
        Physics.Raycast(
            agent.position + (Vector3.up * agent.transform.localScale.y), 
            player.position - agent.position,
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
