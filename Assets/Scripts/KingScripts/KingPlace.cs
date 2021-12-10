using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingPlace : MonoBehaviour
{
    //Is called when the King clicks on the Block button
    public void OnBlockClicked() {
        PlaceObject(0);
    }
    
    //Is called when the King clicks on the Hail button
    public void OnHailClicked() {
        PlaceObject(1);
    }

    //Is called when the King clicks on the Slime button
    public void OnSlimeClicked() {
        PlaceObject(2);
    }

    public GameObject Thunderstorm;
    //Is called when the King clicks on the Thunderstorm button
    public void OnThundClicked() {

    }


    public GameObject Block;
    public GameObject Hail;
    public GameObject Slime;
    public GameObject Grid;
    private void PlaceObject(int ID) {
        Grid.GetComponent<GridReveal>().GridSwitch(true);
        //Switch statement, ID-0 = Block,ID-1 = Hail,ID-2 = Slime
        GameObject Place;
        switch (ID) {//Parses in the button clicked into the right object that the King is placing
            case 0:
                Debug.Log("B");
                Place = Block;
                break;
            case 1:
                Debug.Log("H");
                Place = Hail;
                break;
            case 2:
                Debug.Log("S");
                Place = Slime;
                break;
        }
        //Layout the grid
            /* Have grid always existing? and Enable it for the King to see?
             * Draw the grid dynamically based on the King's Camera?
             * Draw it/Have it show up around the mouse cursor?
             */

        //The player selects where on the grid they want to place the object
            /* Center anchored
             * Should be "simple" to snap the cursor poistion onto the grid.
             */

        //The object is placed there
        //Energy is spent
    }
}
