using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class WarningPop : MonoBehaviour
{
    public GameObject warningBox;
    // Start is called before the first frame update

    //they no want defautls
    public void cancel()
    {
        warningBox.SetActive(false);
    }
    public void setDefaultAndContinue()
    {
        //set default
        this.GetComponent<RebindManager>().setAllControlsToDefaults();
        warningBox.SetActive(false);
        SceneManager.LoadScene("Options");
    }
}
