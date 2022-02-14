using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AerialBaseState
{
    public abstract void EnterState(AerialStateManager aSM);
    public abstract void UpdateState(AerialStateManager aSM);
    public abstract void FixedUpdateState(AerialStateManager aSM);
    public abstract void OnCollisionEnter(AerialStateManager aSM);
}
