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

    private int Ytop = 600;
    int Lifetime = 0;

    public GameObject Hail;

    int timer = 0;

    RaycastHit hit;
    [SerializeField] private LayerMask LayerMask;
    float height = 0f;
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

            Vector3 position = new Vector3(Random.Range(Xmin + radius, Xmax - radius), Ytop, Random.Range(Zmin + radius, Zmax - radius));//Finds where the hail will spawn in the air
            if (Physics.Raycast(position, transform.TransformDirection(Vector3.down), out hit, float.MaxValue, LayerMask)) {//Raycasts to find where the ground is
                height = Ytop - hit.distance;
            }
            position = new Vector3(position.x, 100+height, position.z); //find ground and set y occordingly

            SpawnHail(diameter, position);
        }
        timer++;

        Lifetime++;
        if (Lifetime == 3000) {
            Destruction();
        }
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

    void Destruction() {
        Destroy(gameObject);
    }
}
