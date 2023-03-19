using UnityEngine;
using System;
using BaseDefense.Characters;

namespace BaseDefense.StateMachine {

    public abstract class State {

        protected Action Stage;
        protected State NextState;
        protected EnemyCharacter Agent;
        protected Animator Animator;
        protected CharacterController Controller;
        protected Transform Player;
        protected Transform Transform;
        protected bool AttackTrigger;

        protected abstract void Enter ();
        protected abstract void Update ();
        protected abstract void Exit ();

        public void SetTrigger (bool value) => AttackTrigger = value;


        public State Process () {
            Stage();

            if (Stage == Exit) {
                Stage();

                return NextState;
            }

            return this;
        }

    }

}