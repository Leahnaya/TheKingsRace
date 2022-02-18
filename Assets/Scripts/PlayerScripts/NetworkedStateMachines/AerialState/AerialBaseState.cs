using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Add generics
public abstract class AerialBaseState
{
    public abstract void EnterState(AerialStateManager aSM, AerialBaseState previousState);
    public abstract void ExitState(AerialStateManager aSM, AerialBaseState nextState);
    public abstract void UpdateState(AerialStateManager aSM);
    public abstract void FixedUpdateState(AerialStateManager aSM);
    
}
