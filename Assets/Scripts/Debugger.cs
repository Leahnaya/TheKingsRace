using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    float y = 0;

    private void FixedUpdate()
    {
        if (gameObject.transform.position.y > y)
        {
            y = gameObject.transform.position.y;
            Debug.Log(y.ToString());
        }
    }
}
