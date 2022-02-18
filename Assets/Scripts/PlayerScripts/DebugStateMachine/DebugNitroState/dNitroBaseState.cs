using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class dNitroBaseState
{
    public abstract void EnterState(dNitroStateManager nSM, dNitroBaseState previousState);
    public abstract void ExitState(dNitroStateManager nSM, dNitroBaseState nextState);
    public abstract void UpdateState(dNitroStateManager nSM);
    public abstract void FixedUpdateState(dNitroStateManager nSM);
}
