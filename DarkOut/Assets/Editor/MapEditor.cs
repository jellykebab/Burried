using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor (typeof (MapGenControls))]
public class MapEditor : Editor {

	public override void OnInspectorGUI ()
	{
		MapGenControls map = target as MapGenControls;

        if(DrawDefaultInspector () && map.realTimeEditor == true){
            map.GenerateMap ();
        }

        if(GUILayout.Button("Generate Map")){
            map.GenerateMap (); 
        }
		
	}
	
}