using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dMoveRecoveringState : dMoveBaseState
{
    ////// ADD SOMETHING THAT CHECKS ANIMATION FINISH BEFORE GO TO IDLE

    public override void EnterState(dMoveStateManager mSM, dMoveBaseState previousState){
        mSM.CancelMomentum(); // reset player variables
    }

    public override void ExitState(dMoveStateManager mSM, dMoveBaseState nextState){

    }

    public override void UpdateState(dMoveStateManager mSM){
        
        //swap to idle state
        mSM.SwitchState(mSM.IdleState);
    }

    public override void FixedUpdateState(dMoveStateManager mSM){

    }
}
