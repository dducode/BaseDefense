using UnityEngine;
using BaseDefense.Characters;

namespace BaseDefense.StateMachine {

    public class Attack : State {

        private readonly float m_attackDistance;
        private static readonly int AttackID = Animator.StringToHash("attack");


        public Attack (EnemyCharacter agent, Transform player) {
            Stage = Enter;
            Agent = agent;
            Player = player;
            Animator = agent.Animator;
            Controller = agent.Controller;
            m_attackDistance = agent.AttackDistance;
            Transform = agent.transform;
        }


        protected override void Enter () {
            Animator.SetBool(AttackID, true);
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

            const int layerMask = 1 << 6;
            Physics.Raycast(
                position + (Vector3.up * Agent.transform.localScale.y),
                playerPosition - position,
                out var hit,
                Mathf.Infinity,
                layerMask);

            if (hit.transform && hit.transform.CompareTag("Player")) {
                if (hit.distance > m_attackDistance + 0.5f) {
                    NextState = new Running(Agent, Player);
                    Stage = Exit;
                }
            }
            else {
                NextState = new Walking(Agent, Player);
                Stage = Exit;
            }
        }


        protected override void Exit () {
            Animator.SetBool(AttackID, false);
        }

    }

}