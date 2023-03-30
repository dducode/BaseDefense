using UnityEngine;
using BaseDefense.Characters;

namespace BaseDefense.StateMachine {

    public class Walking : State {

        private readonly float m_speed;
        private readonly Vector3 m_targetPoint;
        private static readonly int WalkingID = Animator.StringToHash("walking");


        public Walking (EnemyCharacter agent, Transform player) {
            Stage = Enter;
            Agent = agent;
            Player = player;
            Animator = agent.Animator;
            Controller = agent.Controller;
            m_speed = agent.WalkingSpeed;
            m_targetPoint = agent.GetRandomPoint();
            Transform = agent.transform;
        }


        protected override void Enter () {
            Animator.SetBool(WalkingID, true);
            Stage = Update;
        }


        protected override void Update () {
            Transform.rotation = Quaternion.Slerp(
                Transform.rotation,
                Quaternion.LookRotation(m_targetPoint - Transform.position),
                Time.smoothDeltaTime * 15f
            );
            Controller.Move(Transform.forward * (m_speed * Time.smoothDeltaTime));

            if (AttackTrigger) {
                NextState = new Running(Agent, Player);
                Stage = Exit;
            }
        }


        protected override void Exit () {
            Animator.SetBool(WalkingID, false);
        }

    }

}