using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class dAerialBaseState
{
    public abstract void EnterState(dAerialStateManager aSM, dAerialBaseState previousState);
    public abstract void ExitState(dAerialStateManager aSM, dAerialBaseState nextState);
    public abstract void UpdateState(dAerialStateManager aSM);
    public abstract void FixedUpdateState(dAerialStateManager aSM);
    
}
