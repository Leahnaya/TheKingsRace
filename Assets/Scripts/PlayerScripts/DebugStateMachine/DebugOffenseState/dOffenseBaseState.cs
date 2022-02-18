using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class dOffenseBaseState
{
    public abstract void EnterState(dOffenseStateManager oSM, dOffenseBaseState previousState);
    public abstract void ExitState(dOffenseStateManager oSM, dOffenseBaseState nextState);
    public abstract void UpdateState(dOffenseStateManager oSM);
    public abstract void FixedUpdateState(dOffenseStateManager oSM);
}
