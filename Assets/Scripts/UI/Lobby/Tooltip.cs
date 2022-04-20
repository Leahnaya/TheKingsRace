using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour
{

    private TextMeshProUGUI tooltipText;

    private void Start(){
        tooltipText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        HideTooltip();
    }

    public void ShowTooltip(string curTooltip){
        this.gameObject.SetActive(true);
        tooltipText.text = curTooltip;
    }

    public void HideTooltip(){
        this.gameObject.SetActive(false);
    }
}
