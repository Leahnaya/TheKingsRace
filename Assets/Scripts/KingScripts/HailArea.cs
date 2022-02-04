using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HailArea : MonoBehaviour
{
    // Update is called once per frame
    private float Xmax = 15f;
    private float Xmin = -15f;

    private float Zmax = -85f;
    private float Zmin = -115f;

    public GameObject Hail;

    int timer = 0;
    // FixedUpdate is called once per .02 seconds (or 50 times a second)
    void FixedUpdate()
    {
        if (timer == 50) {
            timer = 0;
            //Random diameter 4->8
            float diameter = Random.Range(4, 8);
            float radius = diameter / 2.0f;
            //Random pos X Xmax-radius -> Xmin+radius
            //Random pos Z Zmax-radius -> Zmin+radius
            Vector3 position = new Vector3(Random.Range(Xmin + radius, Xmax - radius), 100, Random.Range(Zmin + radius, Zmax - radius)); //TODO find ground and set y occordingly

            SpawnHail(diameter, position);
        }
        timer++;
    }

    // Spawns a singular Hail piece
    void SpawnHail(float size, Vector3 pos)
    {
        //Spawn HailBall and its shadow
        GameObject HailTemp = null;
        HailTemp = Instantiate(Hail);
        HailTemp.transform.position = pos;
        HailTemp.transform.localScale = new Vector3(size, size, size);
    }

    public void SetArea(float LeftBound, float RightBound, float TopBound, float BottomBound) {
        Xmin = LeftBound;
        Xmax = RightBound;
        Zmax = TopBound;
        Zmin = BottomBound;
    }
}
