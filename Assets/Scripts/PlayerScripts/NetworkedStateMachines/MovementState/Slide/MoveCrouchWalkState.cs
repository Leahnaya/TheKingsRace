using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCrouchWalkState : MoveBaseState
{
    public override void EnterState(MoveStateManager mSM, MoveBaseState previousState){

    }

    public override void ExitState(MoveStateManager mSM, MoveBaseState nextState){

    }
    
    public override void UpdateState(MoveStateManager mSM){

    }

    public override void FixedUpdateState(MoveStateManager mSM){

        /*
        if(mSM.aSM.currentState == mSM.aSM.FallingState){
             //Determine which state to go into based on player speed
                if(mSM.calculatedCurVel < mSM.walkLimit){
                    SlideToMoveState(mSM);
                    mSM.SwitchState(mSM.WalkState);
                }
                else if(mSM.calculatedCurVel < mSM.runLimit){
                    SlideToMoveState(mSM);
                    mSM.SwitchState(mSM.JogState);
                }
                else{
                    SlideToMoveState(mSM);
                    mSM.SwitchState(mSM.RunState);
                }
        }
        */
    }

}
