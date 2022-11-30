using UnityEngine;

public class Walking : State
{
    float speed;
    Vector3 targetPoint;

    public Walking
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
        animator.SetBool("walking", true);
        EnemyCharacter enemy = agent.GetComponent<EnemyCharacter>();
        targetPoint = enemy.getPoint;
        speed = enemy.getWalkingSpeed;
        stage = Update;
    }
    protected override void Update()
    {
        agent.rotation = Quaternion.Slerp(
            agent.rotation, 
            Quaternion.LookRotation(targetPoint - agent.position), 
            Time.smoothDeltaTime * 15f
        );
        controller.Move(agent.forward * speed * Time.smoothDeltaTime);
        
        if (attackTrigger)
        {
            nextState = new Running(animator, controller, agent, player);
            stage = Exit;
        }
    }
    protected override void Exit()
    {
        animator.SetBool("walking", false);
    }
}
