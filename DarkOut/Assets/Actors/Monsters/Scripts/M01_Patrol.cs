using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class M01_Patrol : StateMachineBehaviour
{
    
    private float timer = 0.0f;
    private float patrolTime = 10.0f;
    private float soundTimer = 0.0f;
    
    private MapGenControls mapControl = null;
    private GameObject map = null;

    private Transform tgtLocation = null;
    private NavMeshAgent posAgent = null;
    private Transform player = null;
    private AudioSource monsterSound = null;
    private M01 monsterControl = null;
    private string monsterCode = "M00";
    private string[] idleSounds;
    [SerializeField] float patrolSpeed = 1f;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0.0f;
        patrolTime = Random.Range(10.0f, 30.0f);
        soundTimer = Random.Range(2.0f, 6.0f);

        posAgent = animator.GetComponent<NavMeshAgent>();
        posAgent.speed = patrolSpeed;

        map = GameObject.Find("Controller_MapGenerator");
        mapControl = map.GetComponent<MapGenControls>(); 

        player = GameObject.FindWithTag("Player").transform;
        monsterSound = animator.gameObject.GetComponent<AudioSource>();
        monsterControl = animator.gameObject.GetComponent<M01>();

        monsterCode = monsterControl.getMonsterCode();

        string currentCode = monsterCode + "_Idle";

        idleSounds = AssetDatabase.FindAssets(currentCode, new[] {"Assets/Actors/Monsters/Audio"});

        GameObject tgtLocation = new GameObject();
        setLocation();
        
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (posAgent.remainingDistance <= posAgent.stoppingDistance || monsterControl.noiseHeard == true){

            setLocation();
        }
        
        timer += Time.deltaTime;

        float distance = Vector3.Distance(player.position, animator.transform.position);

        if (timer >= soundTimer){

            int soundNum = Random.Range(0, idleSounds.Length);
            AudioClip soundToPlay = (AudioClip)AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath(idleSounds[soundNum]), typeof(AudioClip));
            monsterSound.clip = soundToPlay;
            monsterSound.Play();
            soundTimer += Random.Range(2.0f, 6.0f);            
        }

        if (monsterControl.inRange == true){
            Debug.Log("Chasing player");
            Debug.Log("Patrol > Chase");
            animator.SetBool("onChase", true);
        }

        if (timer > patrolTime){
            animator.SetBool("onPatrol", false);
        }  
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        monsterSound.Stop();
        posAgent.SetDestination(animator.transform.position);
        Debug.Log("Patrol end");
    }

    private void setLocation(){

        tgtLocation = new GameObject().transform;

        if (monsterControl.noiseHeard == true){

            Debug.Log("Following noise");
            tgtLocation.transform.position = monsterControl.targetPosition;
        }
        else{
            
            Debug.Log("Random location");
            Transform tgtTile = mapControl.GetRandOpenTile();
            mapControl.returnTile();
            
            tgtLocation.position = tgtTile.position;
        }
        

        posAgent.SetDestination(tgtLocation.position);

        Destroy(tgtLocation.gameObject);
    }


}
