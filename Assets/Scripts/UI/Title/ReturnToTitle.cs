using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToTitle : MonoBehaviour
{
    //reference to canvas  (used to clal rebindManager)
    public Canvas canvasRef;
    public GameObject warningBox;

    public void backToSettings()
    {
        //if there are no duplicates, let them go back
        if (canvasRef.GetComponent<RebindManager>().areThereDuplicates() == false){
            SceneManager.LoadScene("Options");
        }
        //bring 
        else
        {
            warningBox.SetActive(true);
        }
    }

        public void backToTitle() {
            SceneManager.LoadScene("TitleScene");
        }
}