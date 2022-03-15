using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindDirection : MonoBehaviour
{
    public Vector3 windDireciton;

    private void Start() {
        windDireciton = new Vector3(0, 0, 0);
    }
}
