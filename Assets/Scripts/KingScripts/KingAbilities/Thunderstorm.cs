using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunderstorm : MonoBehaviour
{
    private bool inAir = false;
    private float DURATION = 30.0f;
    private float DISTANCE_THRESHOLD = 5.0f;
    private float ZAP_COUNTDOWN = 5.0f;
    private Vector3 down = Vector3.down;
    private RaycastHit hit;    
    private Ray ray;

    public GameObject playerObject; //When spawning set this programatically as a part of the spawn function
    private PlayerMovement playerController;
    public ParticleSystem clouds;
    public ParticleSystem bolt;
    public ParticleSystem pow;
    public ParticleSystem sparks;


    private void Awake()
    {
        playerController = playerObject.GetComponent<PlayerMovement>();
        gameObject.transform.parent = playerObject.transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        ray = new Ray(gameObject.transform.position, down);
        Physics.Raycast(ray, out hit, DISTANCE_THRESHOLD);
        if (hit.collider == null)
        {
            inAir = true;
        }
        if(clouds != null) clouds.Play();
    }

    private void FixedUpdate()
    {
        //Last for a set duration
        DURATION -= Time.fixedDeltaTime;
        if (DURATION >= 0 && !inAir)
        {
            //Stop particle systems <-- is this needed?
            Destroy(gameObject);
        }

        //If player is grounded we will stop zap counter
        if(playerController.isGrounded)
        {
            inAir = false;
            bolt.Stop();
            sparks.Stop();
            pow.Stop();
            ZAP_COUNTDOWN = 5.0f;
        }
        else // Otherwise check if player is high enough to zap
        {
            ray = new Ray(gameObject.transform.position, down);
            Physics.Raycast(ray, out hit, DISTANCE_THRESHOLD);
            if (hit.collider == null)
            {
                inAir = true;
                if(bolt != null) bolt.Play();
                if(sparks != null) sparks.Play();
                if(pow != null) pow.Play();
            }
        }

        ThunderCoroutine();
    }

    private void ThunderCoroutine()
    {
        if (inAir)
        {
            ZAP_COUNTDOWN -= Time.fixedDeltaTime;
            if(ZAP_COUNTDOWN >= 0)
            {
                playerController.GetHit(down, 100); //Needs to be tested if force is appropriate
                ZAP_COUNTDOWN = 5.0f;
            }
        }
    }
}
