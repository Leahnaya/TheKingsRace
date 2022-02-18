using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class dDashBaseState
{
    public abstract void EnterState(dDashStateManager dSM, dDashBaseState previousState);
    public abstract void ExitState(dDashStateManager dSM, dDashBaseState nextState);
    public abstract void UpdateState(dDashStateManager dSM);
    public abstract void FixedUpdateState(dDashStateManager dSM);
}
