using UnityEngine;
using BaseDefense.Characters;

namespace BaseDefense.StateMachine
{
    public class Running : State
    {
        float speed;
        float attackDistance;

        public Running(EnemyCharacter agent, Transform player)
        {
            stage = Enter;
            this.agent = agent;
            this.player = player;
            animator = agent.Animator;
            controller = agent.Controller;
            speed = agent.MaxSpeed;
            attackDistance = agent.AttackDistance;
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
                nextState = new Walking(agent, player);
                stage = Exit;
            }
            if (hit.transform && hit.transform.CompareTag("Player"))
            {
                if (hit.distance < attackDistance)
                {
                    nextState = new Attack(agent, player);
                    stage = Exit;
                }
            }
            else
            {
                nextState = new Walking(agent, player);
                stage = Exit;
            }
        }
        protected override void Exit()
        {
            animator.SetBool("running", false);
        }
    }
}


