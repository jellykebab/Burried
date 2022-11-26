    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class M01_Chase : StateMachineBehaviour
{

    private float timer = 0.0f;
    private float soundTimer = 0.0f;

    private NavMeshAgent posAgent = null;
    private Transform player = null;
    private AudioSource monsterSound = null;
    private M01 monsterControl = null;
    private string monsterCode = "M00";
    private string[] chaseSounds;

    [SerializeField]private float chaseSpeed = 3.5f;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0.0f;
        soundTimer = Random.Range(2.0f, 6.0f);

        animator.transform.LookAt(player);

        Debug.Log("On chase");

        posAgent = animator.GetComponent<NavMeshAgent>();
        posAgent.speed  = chaseSpeed;
        player = GameObject.FindWithTag("Player").transform;

        monsterSound = animator.gameObject.GetComponent<AudioSource>();
        monsterControl = animator.gameObject.GetComponent<M01>();

        monsterCode = monsterControl.getMonsterCode();
        string currentCode = monsterCode + "_Chase";
        chaseSounds = AssetDatabase.FindAssets(currentCode, new[] {"Assets/Actors/Monsters/Audio"});

        playSound(); 

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;

        if (monsterControl.inRange == true){
            posAgent.SetDestination(player.position);
        }
        else if(monsterControl.monsterHeard == true){
            posAgent.SetDestination(monsterControl.targetPosition);
        }
        else{
            if (posAgent.remainingDistance <= posAgent.stoppingDistance){
                animator.SetBool("onChase", false);
            }
            else{
                Debug.Log("Finishing chase");
            }
        }

        float distance = Vector3.Distance(player.position, animator.transform.position);

        if (timer >= soundTimer){

            playSound();
            soundTimer += Random.Range(2.0f, 6.0f);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        monsterSound.Stop();
        posAgent.SetDestination(animator.transform.position);
        Debug.Log("Chase end");
    }


    private void playSound(){

        int soundNum = Random.Range(0, chaseSounds.Length);
        AudioClip soundToPlay = (AudioClip)AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath(chaseSounds[soundNum]), typeof(AudioClip));
        monsterSound.clip = soundToPlay;
        monsterSound.Play();
    }

}
