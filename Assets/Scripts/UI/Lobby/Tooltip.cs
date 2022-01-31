using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{

    private Text tooltipText;

    private void Start(){
        tooltipText = transform.Find("Text").GetComponent<Text>();
        HideTooltip();
    }

    public void ShowTooltip(string curTooltip){
        gameObject.SetActive(true);
        tooltipText.text = curTooltip;
    }

    public void HideTooltip(){
        gameObject.SetActive(false);
    }
}
