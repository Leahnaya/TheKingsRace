using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveBaseState
{
    public abstract void EnterState(MoveStateManager mSM, MoveBaseState previousState);
    public abstract void ExitState(MoveStateManager mSM, MoveBaseState nextState);
    public abstract void UpdateState(MoveStateManager mSM);
    public abstract void FixedUpdateState(MoveStateManager mSM);
}
