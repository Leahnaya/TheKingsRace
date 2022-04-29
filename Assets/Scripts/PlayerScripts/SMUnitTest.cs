using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMUnitTest : MonoBehaviour
{

    //Players State Managers
    public dAerialStateManager aSM;
    public dMoveStateManager mSM;
    public dNitroStateManager nSM;
    public dOffenseStateManager oSM;
    public dDashStateManager dSM;

    //MoveState current and next state
    dMoveBaseState curMoveState;
    dMoveBaseState nextMoveState;
    //Legal move state transisitions
    List<dMoveBaseState> moveIdleTransitions = new List<dMoveBaseState>();
    List<dMoveBaseState> moveWalkTransitions = new List<dMoveBaseState>();
    List<dMoveBaseState> moveJogTransitions = new List<dMoveBaseState>();
    List<dMoveBaseState> moveRunTransitions = new List<dMoveBaseState>();
    List<dMoveBaseState> moveSlideTransitions = new List<dMoveBaseState>();
    List<dMoveBaseState> moveCrouchTransitions = new List<dMoveBaseState>();
    List<dMoveBaseState> moveCrouchWalkTransitions = new List<dMoveBaseState>();
    List<dMoveBaseState> moveRagdollTransitions = new List<dMoveBaseState>();
    List<dMoveBaseState> moveRecoverTransitions = new List<dMoveBaseState>();

    //Aerial current and next state
    dAerialBaseState curAerialState;
    dAerialBaseState nextAerialState;
    //Legal aerial state transisitions
    List<dAerialBaseState> aerialGroundedTransitions = new List<dAerialBaseState>();
    List<dAerialBaseState> aerialFallTransitions = new List<dAerialBaseState>();
    List<dAerialBaseState> aerialJumpTransitions = new List<dAerialBaseState>();
    List<dAerialBaseState> aerialWallrunTransitions = new List<dAerialBaseState>();
    List<dAerialBaseState> aerialGlideTransitions = new List<dAerialBaseState>();
    List<dAerialBaseState> aerialGrappleTransitions = new List<dAerialBaseState>();

    //Nitro current and next state
    dNitroBaseState curNitroState;
    dNitroBaseState nextNitroState;
    //Legal nitro state transisitions
    List<dNitroBaseState> nitroNoneTransitions = new List<dNitroBaseState>();
    List<dNitroBaseState> nitroNitroingTransitions = new List<dNitroBaseState>();
    List<dNitroBaseState> nitroIncapacitatedTransitions = new List<dNitroBaseState>();
    List<dNitroBaseState> nitroCooldownTransitions = new List<dNitroBaseState>();


    //Offense current and next state
    dOffenseBaseState curOffenseState;
    dOffenseBaseState nextOffenseState;
    //Legal offense state transisitions
    List<dOffenseBaseState> offenseNoneTransitions = new List<dOffenseBaseState>();
    List<dOffenseBaseState> offenseKickTransitions = new List<dOffenseBaseState>();
    List<dOffenseBaseState> offenseAirKickTransitions = new List<dOffenseBaseState>();
    List<dOffenseBaseState> offenseIncapacitatedTransitions = new List<dOffenseBaseState>();
    List<dOffenseBaseState> offenseCooldownTransitions = new List<dOffenseBaseState>();

    //Dash current and next state
    dDashBaseState curDashState;
    dDashBaseState nextDashState;
    //Legal dash state transisitions
    List<dDashBaseState> dashNoneTransitions = new List<dDashBaseState>();
    List<dDashBaseState> dashDashingTransitions = new List<dDashBaseState>();
    List<dDashBaseState> dashIncapacitatedTransitions = new List<dDashBaseState>();
    List<dDashBaseState> dashCooldownTransitions = new List<dDashBaseState>();

    // Start is called before the first frame update
    void Awake()
    {

        PopulateTransitionLists();

        //Intitial States for each state manager
        curMoveState = mSM.currentState;
        curAerialState = aSM.currentState;
        curNitroState = nSM.currentState;
        curOffenseState = oSM.currentState;
        curDashState = dSM.currentState;
    }

    // Update is called once per frame
    void Update()
    {
        if(!UpdateMoveStates()){
            Debug.LogAssertion("An Illegal Move State transition has occured " + curMoveState + " tried to transition into " + nextMoveState);
        }
        
        if(!UpdateAerialStates()){
            Debug.LogAssertion("An Illegal Aerial State transition has occured " + curAerialState + " tried to transition into " + nextAerialState);
        }

        if(!UpdateNitroStates()){
            Debug.LogAssertion("An Illegal Nitro State transition has occured " + curNitroState + " tried to transition into " + nextNitroState);
        }

        if(!UpdateDashStates()){
            Debug.LogAssertion("An Illegal Dash State transition has occured " + curDashState + " tried to transition into " + nextDashState);
        }

        if(!UpdateOffenseStates()){
            Debug.LogAssertion("An Illegal Offense State transition has occured " + curOffenseState + " tried to transition into " + nextOffenseState);
        }
    }

    //Populates All State Manager Transition Lists
    private void PopulateTransitionLists(){

        ////Populate MoveStateManager Lists
        moveIdleTransitions.Add(mSM.CrouchState);
        moveIdleTransitions.Add(mSM.WalkState);
        moveIdleTransitions.Add(mSM.RagdollState);

        moveWalkTransitions.Add(mSM.SlideState);
        moveWalkTransitions.Add(mSM.JogState);
        moveWalkTransitions.Add(mSM.IdleState);
        moveWalkTransitions.Add(mSM.RagdollState);
        
        moveJogTransitions.Add(mSM.WalkState);
        moveJogTransitions.Add(mSM.SlideState);
        moveJogTransitions.Add(mSM.RunState);
        moveJogTransitions.Add(mSM.RagdollState);

        moveRunTransitions.Add(mSM.SlideState);
        moveRunTransitions.Add(mSM.JogState);
        moveRunTransitions.Add(mSM.RagdollState);

        moveSlideTransitions.Add(mSM.CrouchState);
        moveSlideTransitions.Add(mSM.WalkState);
        moveSlideTransitions.Add(mSM.JogState);
        moveSlideTransitions.Add(mSM.RunState);
        moveSlideTransitions.Add(mSM.RagdollState);

        moveCrouchTransitions.Add(mSM.CrouchWalkState);
        moveCrouchTransitions.Add(mSM.WalkState);
        moveCrouchTransitions.Add(mSM.RagdollState);

        moveCrouchWalkTransitions.Add(mSM.CrouchState);
        moveCrouchWalkTransitions.Add(mSM.WalkState);
        moveCrouchWalkTransitions.Add(mSM.RagdollState);

        moveRagdollTransitions.Add(mSM.RecoveringState);

        moveRecoverTransitions.Add(mSM.IdleState);
        ////

        ////Populate AerialStateManager Lists
        aerialGroundedTransitions.Add(aSM.FallingState);
        aerialGroundedTransitions.Add(aSM.JumpingState);
        aerialGroundedTransitions.Add(aSM.WallRunState);
        aerialGroundedTransitions.Add(aSM.GrappleAirState);

        aerialFallTransitions.Add(aSM.GroundedState);
        aerialFallTransitions.Add(aSM.GlidingState);
        aerialFallTransitions.Add(aSM.JumpingState);
        aerialFallTransitions.Add(aSM.WallRunState);
        aerialFallTransitions.Add(aSM.GrappleAirState);

        aerialJumpTransitions.Add(aSM.GroundedState);
        aerialJumpTransitions.Add(aSM.FallingState);
        aerialJumpTransitions.Add(aSM.WallRunState);
        aerialJumpTransitions.Add(aSM.GrappleAirState);

        aerialWallrunTransitions.Add(aSM.GroundedState);
        aerialWallrunTransitions.Add(aSM.FallingState);
        aerialWallrunTransitions.Add(aSM.JumpingState);
        aerialWallrunTransitions.Add(aSM.GrappleAirState);

        aerialGlideTransitions.Add(aSM.GroundedState);
        aerialGlideTransitions.Add(aSM.FallingState);
        aerialGlideTransitions.Add(aSM.WallRunState);
        aerialGlideTransitions.Add(aSM.GrappleAirState);

        aerialGrappleTransitions.Add(aSM.GroundedState);
        aerialGrappleTransitions.Add(aSM.FallingState);
        aerialGrappleTransitions.Add(aSM.WallRunState);
        ////

        ////Populate NitroStateManager Lists
        nitroNoneTransitions.Add(nSM.NitroingState);
        nitroNoneTransitions.Add(nSM.IncapacitatedState);

        nitroNitroingTransitions.Add(nSM.CooldownState);
        nitroNitroingTransitions.Add(nSM.IncapacitatedState);

        nitroIncapacitatedTransitions.Add(nSM.NoneState);

        nitroCooldownTransitions.Add(nSM.NoneState);
        nitroCooldownTransitions.Add(nSM.IncapacitatedState);
        ////

        ////Populate DashStateManager Lists
        dashNoneTransitions.Add(dSM.DashingState);
        dashNoneTransitions.Add(dSM.IncapacitatedState);

        dashDashingTransitions.Add(dSM.CooldownState);
        dashDashingTransitions.Add(dSM.IncapacitatedState);

        dashIncapacitatedTransitions.Add(dSM.NoneState);

        dashCooldownTransitions.Add(dSM.NoneState);
        dashCooldownTransitions.Add(dSM.IncapacitatedState);
        ////

        ////Populate OffenseStateManager Lists
        offenseNoneTransitions.Add(oSM.KickState);
        offenseNoneTransitions.Add(oSM.AirKickState);
        offenseNoneTransitions.Add(oSM.IncapacitatedState);

        offenseKickTransitions.Add(oSM.CooldownState);
        offenseKickTransitions.Add(oSM.IncapacitatedState);

        offenseAirKickTransitions.Add(oSM.CooldownState);
        offenseAirKickTransitions.Add(oSM.IncapacitatedState);

        offenseIncapacitatedTransitions.Add(oSM.NoneState);

        offenseCooldownTransitions.Add(oSM.NoneState);
        offenseCooldownTransitions.Add(oSM.IncapacitatedState);
        ////
    }

    //Move State Testing
    private bool UpdateMoveStates(){
        if(curMoveState != mSM.currentState){
            if(ValidateMoveState(mSM.currentState)){
                curMoveState = mSM.currentState;
                return true;
            }
            else{
                curMoveState = mSM.currentState;
                nextMoveState = mSM.currentState;
                return false;
            }
        }
        return true;       
    }
    
    private bool ValidateMoveState(dMoveBaseState nextState){
        //Checks if The next state exists within the current transitions list
        if(curMoveState == mSM.IdleState && moveIdleTransitions.Contains(nextState)){
            return true;
        }
        else if(curMoveState == mSM.WalkState && moveWalkTransitions.Contains(nextState)){
            return true;
        }
        else if(curMoveState == mSM.JogState && moveJogTransitions.Contains(nextState)){
            return true;
        }
        else if(curMoveState == mSM.RunState && moveRunTransitions.Contains(nextState)){
            return true;
        }
        else if(curMoveState == mSM.SlideState && moveSlideTransitions.Contains(nextState)){
            return true;
        }
        else if(curMoveState == mSM.CrouchState && moveCrouchTransitions.Contains(nextState)){
            return true;
        }
        else if(curMoveState == mSM.CrouchWalkState && moveCrouchWalkTransitions.Contains(nextState)){
            return true;
        }
        else if(curMoveState == mSM.RagdollState && moveRagdollTransitions.Contains(nextState)){
            return true;
        }
        else if(curMoveState == mSM.RecoveringState && moveRecoverTransitions.Contains(nextState)){
            return true;
        }
        else{
            return false;
        }
    }

    //Aerial State Testing
    private bool UpdateAerialStates(){
        if(curAerialState != aSM.currentState){
            if(ValidateAerialState(aSM.currentState)){
                curAerialState = aSM.currentState;
                return true;
            }
            else{
                curAerialState = aSM.currentState;
                nextAerialState = aSM.currentState;
                return false;
            }
        }
        return true;       
    }
    
    private bool ValidateAerialState(dAerialBaseState nextState){
        //Checks if The next state exists within the current transitions list
        if(curAerialState == aSM.GroundedState && aerialGroundedTransitions.Contains(nextState)){
            return true;
        }
        else if(curAerialState == aSM.FallingState && aerialFallTransitions.Contains(nextState)){
            return true;
        }
        else if(curAerialState == aSM.JumpingState && aerialJumpTransitions.Contains(nextState)){
            return true;
        }
        else if(curAerialState == aSM.WallRunState && aerialWallrunTransitions.Contains(nextState)){
            return true;
        }
        else if(curAerialState == aSM.GlidingState && aerialGlideTransitions.Contains(nextState)){
            return true;
        }
        else if(curAerialState == aSM.GrappleAirState && aerialGrappleTransitions.Contains(nextState)){
            return true;
        }
        else{
            return false;
        }
    }

    //Nitro State Testing
    private bool UpdateNitroStates(){
        if(curNitroState != nSM.currentState){
            if(ValidateNitroState(nSM.currentState)){
                curNitroState = nSM.currentState;
                return true;
            }
            else{
                curNitroState = nSM.currentState;
                nextNitroState = nSM.currentState;
                return false;
            }
        }
        return true;       
    }
    
    private bool ValidateNitroState(dNitroBaseState nextState){
        //Checks if The next state exists within the current transitions list
        if(curNitroState == nSM.NoneState && nitroNoneTransitions.Contains(nextState)){
            return true;
        }
        else if(curNitroState == nSM.NitroingState && nitroNitroingTransitions.Contains(nextState)){
            return true;
        }
        else if(curNitroState == nSM.IncapacitatedState && nitroIncapacitatedTransitions.Contains(nextState)){
            return true;
        }
        else if(curNitroState == nSM.CooldownState && nitroCooldownTransitions.Contains(nextState)){
            return true;
        }
        else{
            return false;
        }
    }

    //Dash State Testing
    private bool UpdateDashStates(){
        if(curDashState != dSM.currentState){
            if(ValidateDashState(dSM.currentState)){
                curDashState = dSM.currentState;
                return true;
            }
            else{
                curDashState = dSM.currentState;
                nextDashState = dSM.currentState;
                return false;
            }
        }
        return true;       
    }
    
    private bool ValidateDashState(dDashBaseState nextState){
        //Checks if The next state exists within the current transitions list
        if(curDashState == dSM.NoneState && dashNoneTransitions.Contains(nextState)){
            return true;
        }
        else if(curDashState == dSM.DashingState && dashDashingTransitions.Contains(nextState)){
            return true;
        }
        else if(curDashState == dSM.IncapacitatedState && dashIncapacitatedTransitions.Contains(nextState)){
            return true;
        }
        else if(curDashState == dSM.CooldownState && dashCooldownTransitions.Contains(nextState)){
            return true;
        }
        else{
            return false;
        }
    }

    //Offense State Testing
    private bool UpdateOffenseStates(){
        if(curOffenseState != oSM.currentState){
            if(ValidateOffenseState(oSM.currentState)){
                curOffenseState = oSM.currentState;
                return true;
            }
            else{
                curOffenseState = oSM.currentState;
                nextOffenseState = oSM.currentState;
                return false;
            }
        }
        
        return true;       
    }
    
    private bool ValidateOffenseState(dOffenseBaseState nextState){
        //Checks if The next state exists within the current transitions list
        if(curOffenseState == oSM.NoneState && offenseNoneTransitions.Contains(nextState)){
            return true;
        }
        else if(curOffenseState == oSM.KickState && offenseKickTransitions.Contains(nextState)){
            return true;
        }
        else if(curOffenseState == oSM.AirKickState && offenseAirKickTransitions.Contains(nextState)){
            return true;
        }
        else if(curOffenseState == oSM.IncapacitatedState && offenseIncapacitatedTransitions.Contains(nextState)){
            return true;
        }
        else if(curOffenseState == oSM.CooldownState && offenseCooldownTransitions.Contains(nextState)){
            return true;
        }
        else{
            return false;
        }
    }

}
