using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    public GameObject Popup;
    private void OnTriggerEnter(Collider other)
    {
        if( other.tag == "Player")
        {
            if (Popup.activeInHierarchy)
            {
                Popup.SetActive(false);
            } else
            {
                Popup.SetActive(true);
            }
        }
    }
}
