using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Dash : MonoBehaviour{
    public float dashTime = 0.5f;
    private float _currentDashTime = 0f;
    private bool _isDashing = false;
    private Vector3 _dashStart, _dashEnd;
    private float m_DashDist = 1.0f;
    private CharacterController characterController;
    void Start(){
        characterController = this.gameObject.GetComponent<CharacterController>();
    }


    void FixedUpdate(){
        Vector3 tdashStart = this.gameObject.transform.position;
        Vector3 tdashEnd = tdashStart + (this.gameObject.transform.forward * m_DashDist);
        Debug.DrawLine(tdashStart, tdashEnd);
        if (Input.GetKeyDown(KeyCode.E)){
            if (_isDashing == false) {
                // dash start
                _isDashing = true;
                _currentDashTime = 0;
                _dashStart = this.gameObject.transform.position;
                _dashEnd = _dashStart + (this.gameObject.transform.forward * m_DashDist);
            }
        }
        if (_isDashing){
            // incrementing time
            _currentDashTime += Time.deltaTime;
            // a value between 0 and 1
            float perc = Mathf.Clamp01(_currentDashTime / dashTime);
            // updating position
            characterController.Move(Vector3.Lerp(_dashStart, _dashEnd, perc));
            if (_currentDashTime >= dashTime){
                // dash finished
                _isDashing = false;
                this.gameObject.transform.position = _dashEnd;
            }
        }
    }



}