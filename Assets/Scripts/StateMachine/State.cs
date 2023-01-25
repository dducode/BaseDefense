using UnityEngine;
using System;

namespace BaseDefense
{
    public abstract class State
    {   
        protected Action stage;
        protected State nextState;
        protected EnemyCharacter agent;
        protected Animator animator;
        protected CharacterController controller;
        protected Transform player;
        protected Transform transform;
        protected bool attackTrigger;

        protected abstract void Enter();
        protected abstract void Update();
        protected abstract void Exit();

        public void SetTrigger(bool value) => attackTrigger = value;

        public State Process()
        {
            stage();
            if (stage == Exit)
            {
                stage();
                return nextState;
            }
            return this;
        }
    }
}


