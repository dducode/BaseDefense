using UnityEngine;

public class Walking : State
{
    float speed;
    Vector3 targetPoint;

    public Walking
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
        speed = agent.getWalkingSpeed;
        targetPoint = agent.getPoint;
        transform = agent.transform;
    }
    protected override void Enter()
    {
        animator.SetBool("walking", true);
        stage = Update;
    }
    protected override void Update()
    {
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            Quaternion.LookRotation(targetPoint - transform.position), 
            Time.smoothDeltaTime * 15f
        );
        controller.Move(transform.forward * speed * Time.smoothDeltaTime);
        
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
