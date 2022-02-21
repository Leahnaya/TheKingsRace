using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractInteractable : MonoBehaviour
{
    //image used for interactable (change this to specific image type when needed)
    [SerializeField] protected GameObject interactableImage;
    //List is fine, but when destroying objects list size will change if you want to delete the element from array
    //(works fien without removing it, but may want to use dynamically sized array later)
    [SerializeField] protected List<GameObject> interactableObjects = new List<GameObject>();

    //stubbed method that will preform interact based on 
    public abstract void OnInteract();

}
