using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M01 : Monsters
{

    [Header("Debug")]
    [SerializeField] private bool noiseRangeToggle = true;
    [SerializeField] private bool chaseRangeToggle = true;

    public Transform player = null;

    private void Awake(){
        debugToggle();
        monsterSetup();
        player = setPlayer();
    }

    private void Update() {
        playerDistance(player);
        Listen();
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.GetComponent<Collider>().gameObject.tag == "Player" ){

                if (inRange)
                    gameOver();

            }
    }

    public void gameOver(){
        playerCaught = true;
        Utility.instance.GameOver();
    }

    private void debugToggle(){
        toggleChaseRange();
        toggleNoiseRange();
    }

    private void toggleNoiseRange(){
        GameObject.Find("Noise_Range").SetActive(noiseRangeToggle);
    }

    private void toggleChaseRange(){
        GameObject.Find("Chase_Range").SetActive(chaseRangeToggle);
    }
}
