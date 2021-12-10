using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private UnityEngine.Vector3 target;
    private bool isLive=false;

    public float speed = 90f;

    //finds targed
    public void Seek(UnityEngine.Vector3 _target)
    {
        //can also do effects, speed on the bullet, damage amount, etc.
        target = _target;
        isLive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            //Destroy(gameObject);
            return;
        }
        if (isLive)
        {
            UnityEngine.Vector3 dir = target - transform.position;
            
            //distance 
            float distanceThisFrame = speed * Time.deltaTime;


            //checking from current distance to current target
            if (dir.magnitude <= distanceThisFrame)
            {
                HitTarget();
                return;
            }

            //move in the worldspace
            transform.Translate(dir.normalized * distanceThisFrame, Space.World);

        }
    }

    //logic for hitting something
    void HitTarget()
    {
        isLive=false;
        Destroy(gameObject);
    }




}
