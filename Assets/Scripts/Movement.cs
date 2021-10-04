using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private CharacterController cc;
    public float speed = 5;
    public float gravity = -5;

    float velocityY = 0;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        velocityY += gravity * Time.deltaTime;

        Vector3 temp = Vector3.zero;
        if (Input.GetKey(KeyCode.A))
        {
            temp += transform.right * -1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            temp += transform.forward * -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            temp += transform.right;
        }
        if (Input.GetKey(KeyCode.W))
        {
            temp += transform.forward;
        }

        Vector3 vel = temp * speed;
        vel.y = velocityY;

        cc.Move(vel * Time.deltaTime);

        if (cc.isGrounded)
        {
            velocityY = 0;
            if (Input.GetKey(KeyCode.Space))
            {
                velocityY = 3;
            }
        }

        if (cc.transform.position.y < -2)
        {
            cc.transform.position = new Vector3(0, (float)1.25, 0);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("WallRun"))
        {
            velocityY = 0;
        }
    }
}
