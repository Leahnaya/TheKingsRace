using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveStateManager : MonoBehaviour
{
    MoveBaseState currentState;
    MoveIdleState IdleState = new MoveIdleState();
    MoveWalkState WalkState = new MoveWalkState();
    MoveJogState JogState = new MoveJogState();
    MoveRunState RunState = new MoveRunState();

    // Start is called before the first frame update
    void Start()
    {
        //players starting state
        currentState = IdleState;
        currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        //calls any logic in the update state from current state
        currentState.UpdateState(this);
    }

    void FixedUpdate(){

        //calls any logic in the fixed update state from current state
        currentState.FixedUpdateState(this);
    }

    void SwitchState(MoveBaseState state){
        currentState = state;
        state.EnterState(this);
    }
}
