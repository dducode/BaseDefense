using BaseDefense.Characters;
using UnityEngine;

namespace BaseDefense.StateMachine {

    public class Running : State {

        private readonly float m_speed;
        private readonly float m_attackDistance;
        private static readonly int RunningID = Animator.StringToHash("running");


        public Running (EnemyCharacter agent, Transform player) {
            Stage = Enter;
            Agent = agent;
            Player = player;
            Animator = agent.Animator;
            Controller = agent.Controller;
            m_speed = agent.MaxSpeed;
            m_attackDistance = agent.AttackDistance;
            Transform = agent.transform;
        }


        protected override void Enter () {
            Animator.SetBool(RunningID, true);
            Stage = Update;
        }


        protected override void Update () {
            var position = Transform.position;
            var playerPosition = Player.position;
            Transform.rotation = Quaternion.Slerp(
                Transform.rotation,
                Quaternion.LookRotation(playerPosition - position),
                Time.smoothDeltaTime * 15f
            );
            Controller.Move(Transform.forward * (m_speed * Time.smoothDeltaTime));

            const int layerMask = 1 << 6;
            Physics.Raycast(
                position + (Vector3.up * Agent.transform.localScale.y),
                playerPosition - position,
                out var hit,
                Mathf.Infinity,
                layerMask);

            if (!AttackTrigger) {
                NextState = new Walking(Agent, Player);
                Stage = Exit;
            }

            if (hit.transform && hit.transform.CompareTag("Player")) {
                if (hit.distance < m_attackDistance) {
                    NextState = new Attack(Agent, Player);
                    Stage = Exit;
                }
            }
            else {
                NextState = new Walking(Agent, Player);
                Stage = Exit;
            }
        }


        protected override void Exit () {
            Animator.SetBool(RunningID, false);
        }

    }

}