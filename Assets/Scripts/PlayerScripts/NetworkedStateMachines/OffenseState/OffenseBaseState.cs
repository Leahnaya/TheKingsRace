using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OffenseBaseState
{
    public abstract void EnterState(OffenseStateManager oSM, OffenseBaseState previousState);
    public abstract void ExitState(OffenseStateManager oSM, OffenseBaseState nextState);
    public abstract void UpdateState(OffenseStateManager oSM);
    public abstract void FixedUpdateState(OffenseStateManager oSM);
}
