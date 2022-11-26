using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class M01_Idle : StateMachineBehaviour
{

    private float timer = 0.0f;
    private float idleTime = 0.0f;
    private float soundTimer = 0.0f;

    private Transform player = null;
    private AudioSource monsterSound = null;
    private M01 monsterControl = null;
    private string monsterCode = "M00";
    private string[] idleSounds;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0.0f;
        idleTime = Random.Range(5.0f, 15.0f);
        soundTimer = Random.Range(2.0f, 6.0f);

        player = GameObject.FindWithTag("Player").transform;
        monsterSound = animator.gameObject.GetComponent<AudioSource>();
        monsterControl = animator.gameObject.GetComponent<M01>();

        monsterCode = monsterControl.getMonsterCode();
        string currentCode = monsterCode + "_Idle";

        idleSounds = AssetDatabase.FindAssets(currentCode, new[] {"Assets/Actors/Monsters/Audio"});
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    { 
        animator.transform.LookAt(player);
     
        timer += Time.deltaTime;

        float distance = Vector3.Distance(player.position, animator.transform.position);

        if (timer >= soundTimer){

            int soundNum = Random.Range(0, idleSounds.Length);
            AudioClip soundToPlay = (AudioClip)AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath(idleSounds[soundNum]), typeof(AudioClip));
            monsterSound.clip = soundToPlay;
            monsterSound.Play();
            soundTimer += Random.Range(2.0f, 6.0f);
        }

        if (monsterControl.noiseHeard == true || timer > idleTime){
            Debug.Log("Idle > Patrol");
            animator.SetBool("onPatrol", true);
            
        }

        if (monsterControl.monsterHeard == true){
            Debug.Log("Idle > Chase");
            animator.SetBool("onChase", true);
        }     
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0.0f;
        monsterSound.Stop();
        Debug.Log("Idle end");    
    }

}
