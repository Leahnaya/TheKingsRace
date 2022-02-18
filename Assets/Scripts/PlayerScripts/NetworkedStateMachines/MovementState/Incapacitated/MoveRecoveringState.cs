using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRecoveringState : MoveBaseState
{
    ////// ADD SOMETHING THAT CHECKS ANIMATION FINISH BEFORE GO TO IDLE

    public override void EnterState(MoveStateManager mSM, MoveBaseState previousState){
        mSM.CancelMomentum(); // reset player variables
    }

    public override void ExitState(MoveStateManager mSM, MoveBaseState nextState){

    }

    public override void UpdateState(MoveStateManager mSM){
        
        //swap to idle state
        mSM.SwitchState(mSM.IdleState);
    }

    public override void FixedUpdateState(MoveStateManager mSM){

    }
}
