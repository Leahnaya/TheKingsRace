using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveBaseState
{
    public abstract void EnterState(MoveStateManager mSM);
    public abstract void UpdateState(MoveStateManager mSM);
    public abstract void FixedUpdateState(MoveStateManager mSM);
    public abstract void OnCollisionEnter(MoveStateManager mSM);
}
