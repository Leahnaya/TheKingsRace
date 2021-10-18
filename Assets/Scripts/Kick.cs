using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kick : MonoBehaviour
{
    private float kickForce = 50f;
    // Start is called before the first frame update
    void Start(){
    }

    // Update is called once per frame
    void Update(){
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("kickable")){
            Vector3 direction = this.transform.forward;
            Debug.Log(direction);
            collision.rigidbody.AddForce(direction * kickForce, ForceMode.Impulse);
        }
    }
}
