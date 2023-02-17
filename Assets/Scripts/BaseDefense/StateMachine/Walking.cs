using UnityEngine;
using BaseDefense.Characters;

namespace BaseDefense.StateMachine
{
    public class Walking : State
    {
        float speed;
        Vector3 targetPoint;

        public Walking (EnemyCharacter agent, Transform player)
        {
            stage = Enter;
            this.agent = agent;
            this.player = player;
            animator = agent.Animator;
            controller = agent.Controller;
            speed = agent.WalkingSpeed;
            targetPoint = agent.GetRandomPoint();
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
                nextState = new Running(agent, player);
                stage = Exit;
            }
        }
        protected override void Exit()
        {
            animator.SetBool("walking", false);
        }
    }
}


