using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRecoveringState : MoveBaseState
{
    ////// ADD SOMETHING THAT CHECKS ANIMATION FINISH BEFORE GO TO IDLE

    public override void EnterState(MoveStateManager mSM){
        mSM.CancelMomentum();
    }

    public override void UpdateState(MoveStateManager mSM){
        mSM.SwitchState(mSM.IdleState);
    }

    public override void FixedUpdateState(MoveStateManager mSM){

    }
    
    public override void OnCollisionEnter(MoveStateManager mSM){

    }
}
