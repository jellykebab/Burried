using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M03_Smoke : MonoBehaviour
{

    private CoughMeter playerControl;

    // Start is called before the first frame update
    void Awake()
    {
        playerControl = GameObject.Find("Controller_Player").GetComponent<CoughMeter>();
    }
    
    private void OnTriggerEnter(Collider other) {

        Debug.Log($"OnTriggerEnter called. other's tag was {other.tag}.");

        if (other.CompareTag("Player")){
            Debug.Log("Player in smoke");
            playerControl.inSmoke = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")){
            Debug.Log("Player exited smoke");
            playerControl.inSmoke = false;
        }
    }
    
}
