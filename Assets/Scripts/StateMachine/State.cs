using UnityEngine;
using System;

public abstract class State
{   
    protected Action stage;
    protected State nextState;
    protected Animator animator;
    protected CharacterController controller;
    protected Transform agent;
    protected Transform player;
    protected bool attackTrigger;

    protected abstract void Enter();
    protected abstract void Update();
    protected abstract void Exit();

    public void Trigger(bool _attackTrigger) => attackTrigger = _attackTrigger;

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
