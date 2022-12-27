using UnityEngine;
using System;

public abstract class State
{   
    protected Action stage;
    protected State nextState;
    protected Animator animator;
    protected CharacterController controller;
    protected EnemyCharacter agent;
    protected Transform player;
    protected Transform transform;
    protected bool attackTrigger;

    protected abstract void Enter();
    protected abstract void Update();
    protected abstract void Exit();

    public void Trigger(bool attackTrigger) => this.attackTrigger = attackTrigger;

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
