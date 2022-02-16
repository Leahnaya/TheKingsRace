using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NitroBaseState
{
    public abstract void EnterState(NitroStateManager nSM, NitroBaseState previousState);
    public abstract void ExitState(NitroStateManager nSM, NitroBaseState nextState);
    public abstract void UpdateState(NitroStateManager nSM);
    public abstract void FixedUpdateState(NitroStateManager nSM);
}
