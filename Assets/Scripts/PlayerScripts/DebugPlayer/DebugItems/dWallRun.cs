using UnityEngine;
using System.Linq;
using MLAPI;
using UnityEngine.Rendering;

[RequireComponent (typeof(dPlayerMovement))]
public class dWallRun : NetworkBehaviour
{

    public float wallMaxDistance = 3f;
    public float wallSpeedMultiplier = 1.2f;
    public float minimumHeight = .1f;
    public float maxAngleRoll = 20;
    [Range(0.0f, 1.0f)]
    public float normalizedAngleThreshold = 0.1f;
    
    public float jumpDuration = .02f;
    public float wallBouncing = 3;
    public float cameraTransitionDuration = 1;

    public float wallGravityDownForce = 5f;

    [Space]
    dPlayerMovement playerMovementController;

    Vector3[] directions;
    RaycastHit[] hits;

    bool isWallRunning = false;
    Vector3 lastWallPosition;
    Vector3 lastWallNormal;
    float elapsedTimeSinceJump = 0;
    float elapsedTimeSinceWallAttach = 0;
    float elapsedTimeSinceWallDetatch = 0;
    bool jumping;

    bool isPlayergrounded() => playerMovementController.isGrounded;

    public bool IsWallRunning() => isWallRunning;

    bool CanWallRun()
    {
        float verticalAxis = Input.GetAxisRaw("Vertical");
        return !isPlayergrounded() && verticalAxis > 0 && VerticalCheck();
    }

    bool VerticalCheck()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumHeight);
    }


    void Start()
    {
        playerMovementController = GetComponent<dPlayerMovement>();

         directions = new Vector3[]{ 
            Vector3.right, 
            Vector3.right + Vector3.forward,
            Vector3.forward, 
            Vector3.left + Vector3.forward, 
            Vector3.left
        };
    }


    public void WallRunRoutine()
    { 
        //if (!IsLocalPlayer) { return; }

        isWallRunning = false;

        hits = new RaycastHit[directions.Length];

        if(playerMovementController.GetJumpPressed())
        {
            jumping = true;
        }

        if(CanAttach())
        {
            for(int i=0; i<directions.Length; i++)
            {
                Vector3 dir = transform.TransformDirection(directions[i]);
                Physics.Raycast(transform.position, dir, out hits[i], wallMaxDistance);
                if(hits[i].collider != null)
                {
                    Debug.DrawRay(transform.position, dir * hits[i].distance, Color.green);
                }
                else
                {
                    Debug.DrawRay(transform.position, dir * wallMaxDistance, Color.red);
                }
            }

            if(CanWallRun())
            {
                hits = hits.ToList().Where(h => h.collider != null).OrderBy(h => h.distance).ToArray();
                if(hits.Length > 0)
                {
                    if(hits[0].collider.tag == "WallRun")
                    {
                        OnWall(hits[0]);
                        lastWallPosition = hits[0].point;
                        lastWallNormal = hits[0].normal;
                    }
                }
            }
        }

        if(isWallRunning)
        {
            elapsedTimeSinceWallDetatch = 0;
            elapsedTimeSinceWallAttach += Time.deltaTime;
        }
        else
        {   
            elapsedTimeSinceWallAttach = 0;
            elapsedTimeSinceWallDetatch += Time.deltaTime;
        }
    }

    bool CanAttach()
    {
        if(jumping)
        {
            elapsedTimeSinceJump += Time.deltaTime;
            if(elapsedTimeSinceJump > jumpDuration)
            {
                elapsedTimeSinceJump = 0;
                jumping = false;
            }
            return false;
        }
        return true;
    }

    void OnWall(RaycastHit hit){
        float d = Vector3.Dot(hit.normal, Vector3.up);
        if(d >= -normalizedAngleThreshold && d <= normalizedAngleThreshold)
        {
            Vector3 alongWall = Vector3.Cross(hit.normal, Vector3.up);
            float vertical = Input.GetAxisRaw("Vertical");
            //Vector3 alongWall = transform.TransformDirection(Vector3.forward);

            // Debug.DrawRay(transform.position, alongWall.normalized * 10, Color.green);
            // Debug.DrawRay(transform.position, lastWallNormal * 10, Color.magenta);

            Vector3 moveToSet = alongWall * vertical * playerMovementController.PlayerSpeed() * Time.deltaTime;// * wallSpeedMultiplier;
            Vector3 velNorm = playerMovementController.vel;
            velNorm.Normalize();

            moveToSet = new Vector3(moveToSet.x * -velNorm.x, moveToSet.y, moveToSet.z * -velNorm.z);

            Vector3 moveToSetNorm = moveToSet;
            moveToSetNorm.Normalize();

            if((moveToSetNorm.x < 0 && velNorm.x > 0)){
                moveToSet.x = (moveToSet.x * -velNorm.x);
            }
            else if((moveToSetNorm.x > 0 && velNorm.x < 0) ){
                moveToSet.x = (-moveToSet.x * -velNorm.x);
            }

            if((moveToSetNorm.z < 0 && velNorm.z > 0)){
                moveToSet.z =  (moveToSet.z * -velNorm.z);
            }
            else if((moveToSetNorm.z > 0 && velNorm.z < 0)){
                moveToSet.z =  (-moveToSet.z * -velNorm.z);
            }

            moveToSet.y = 0;
            //

            playerMovementController.SetPlayerVelocity(moveToSet);
            isWallRunning = true;
            if(playerMovementController.curJumpNum == playerMovementController.pStats.JumpNum){
                playerMovementController.curJumpNum = 0;
            }
            
        }
    }

    float CalculateSide()
    {
        if(isWallRunning)
        {
            Vector3 heading = lastWallPosition - transform.position;
            Vector3 perp = Vector3.Cross(transform.forward, heading);
            float dir = Vector3.Dot(perp, transform.up);
            return dir;
        }
        return 0;
    }

    public Vector3 GetWallJumpDirection() //Add call in jump where if we are wallrunning and jump, the jump vector is multiplied by this
    {
        return lastWallNormal * wallBouncing + (transform.up);
    }
}
