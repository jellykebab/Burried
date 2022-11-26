using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class M02 : Monsters
{
    [Header("Siren Parameters")]
    [SerializeField] private float timerMin;
    [SerializeField] private float timerMax;
    [SerializeField] private float safeRange;
    [SerializeField] private float lightRadius;
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private SpriteRenderer teleGlow;
    [SerializeField] private SpriteRenderer defaultSprite;
    
 
    [Header("Debug")]
    [SerializeField] private bool noiseRangeToggle = true;
    [SerializeField] private bool chaseRangeToggle = true;

    public Transform player = null;
    private Transform tgtPos = null;

    private float timer = 0.0f;
    private float teleTimer;
    private float soundTimer = 0.0f;

    private bool teleporting = false;
    private SphereCollider alertNoise;
    private AudioSource monsterSound = null;

    private string monsterCode = "M00";
    private string[] idleSounds;
    private string[] crySounds;


    private void Awake(){
        //debugToggle();
        monsterSetup();
        player = setPlayer();

        alertNoise = gameObject.GetComponent<SphereCollider>();
        monsterSound = gameObject.GetComponent<AudioSource>();
        alertNoise.enabled = false;

        timer = 0f; 
        teleTimer = Random.Range(timerMin, timerMax);
        soundTimer = Random.Range(2.0f, 6.0f);

        monsterCode = getMonsterCode();
        string currentCode = monsterCode + "_Idle";
        idleSounds = AssetDatabase.FindAssets(currentCode, new[] {"Assets/Actors/Monsters/Audio"});
        
        currentCode = monsterCode + "_Run";
        crySounds = AssetDatabase.FindAssets(currentCode, new[] {"Assets/Actors/Monsters/Audio"});
    }

    private void Update(){
        //Listen();
        lookLight();
        setTimer();
    }

    public void lookLight(){

        if (teleporting == false){
             Vector3 center = gameObject.transform.position;

            Collider[] hitColliders = Physics.OverlapSphere(center, lightRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.GetComponent<Collider>().gameObject.tag == "Light" ){

                    Debug.Log("Lights...!");
                    teleporting = true;
                    //playScream();
                    StartCoroutine(alertMonsters());
                }
            }
        }
    }

    private void setTimer(){
        timer += Time.deltaTime;
        if (timer > teleTimer){
            Teleport();       
        }

        if (timer >= soundTimer){

            int soundNum = Random.Range(0, idleSounds.Length);
            AudioClip soundToPlay = (AudioClip)AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath(idleSounds[soundNum]), typeof(AudioClip));
            monsterSound.clip = soundToPlay;
            monsterSound.Play();
            soundTimer += Random.Range(2.0f, 6.0f);
        }
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

    /*private void debugToggle(){
        toggleChaseRange();
        toggleNoiseRange();
    }*/

    private void tileDistance(Transform p){

        if(p.position != null){

            distance = Vector3.Distance(p.position, tgtPos.position);
        }
        else{
            Debug.Log("Cant calculate distance");
        }
    }

    private void Teleport(){

        bool positionFound = false;

        
        
        while(positionFound == false){
            tgtPos = GameObject.Find("Controller_MapGenerator").GetComponent<MapGenControls>().GetRandOpenTile();
            tileDistance(player);
            GameObject.Find("Controller_MapGenerator").GetComponent<MapGenControls>().returnTile();
            if (distance > safeRange){

                Debug.Log("Position found");
                targetPosition = tgtPos.position;
                gameObject.transform.position = targetPosition;
                Debug.Log("Teleported to " + targetPosition);

                positionFound = true;

                timer = 0;
                teleTimer = Random.Range(timerMin, timerMax);
                teleporting = false;
            }
            else{
                Debug.Log("Relocating");
                Teleport();
            }
        }   
    }

    private void playScream(){
        int soundNum = Random.Range(0, crySounds.Length);
        AudioClip soundToPlay = (AudioClip)AssetDatabase.LoadAssetAtPath( AssetDatabase.GUIDToAssetPath(crySounds[soundNum]), typeof(AudioClip));
        monsterSound.clip = soundToPlay;
        monsterSound.Play();
    }

    private IEnumerator alertMonsters(){

        bool screamed = false;
        
        alertNoise.enabled = true;

        Color tmpColor = teleGlow.color;

        while (tmpColor.a < 1f){

            if (screamed == false){
                playScream();
                screamed = true;
            }

            tmpColor.a += Time.deltaTime / fadeTime;
            teleGlow.color = tmpColor;

            if(tmpColor.a >= 1f){
                tmpColor.a = 1.0f;
            }

            yield return null;

        }

        alertNoise.enabled = false;

        Teleport();

        tmpColor.a = 0.0f;

        teleGlow.color = tmpColor;

        yield return null;
    }
}
