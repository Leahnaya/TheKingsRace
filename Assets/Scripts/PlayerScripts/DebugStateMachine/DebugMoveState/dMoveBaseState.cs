using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class dMoveBaseState
{
    public abstract void EnterState(dMoveStateManager mSM, dMoveBaseState previousState);
    public abstract void ExitState(dMoveStateManager mSM, dMoveBaseState nextState);
    public abstract void UpdateState(dMoveStateManager mSM);
    public abstract void FixedUpdateState(dMoveStateManager mSM);
}
