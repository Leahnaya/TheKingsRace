using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour {

    public TMP_Text countdown_text;

    void Start() {
        countdown_text.text = "Ready?";
        StartCoroutine(BeginCoundown());
    }


    IEnumerator BeginCoundown() {
        yield return new WaitForSecondsRealtime(1f);
        countdown_text.text = "3";
        yield return new WaitForSecondsRealtime(1f);
        countdown_text.text = "2";
        yield return new WaitForSecondsRealtime(1f);
        countdown_text.text = "1";
        yield return new WaitForSecondsRealtime(1f);
        countdown_text.text = "Go!";
    }
}
