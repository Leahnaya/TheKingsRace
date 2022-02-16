using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DashBaseState
{
    public abstract void EnterState(DashStateManager dSM, DashBaseState previousState);
    public abstract void ExitState(DashStateManager dSM, DashBaseState nextState);
    public abstract void UpdateState(DashStateManager dSM);
    public abstract void FixedUpdateState(DashStateManager dSM);
}
