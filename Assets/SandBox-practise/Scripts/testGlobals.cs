using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testGlobals : MonoBehaviour {

	// Use this for initialization
	void Start () {
        print(Globals.Score);
        	
	}
	
	// Update is called once per frame
	void Update () {

        Globals.Score = Globals.Score + 1;
        print(Globals.Score);
    }
}
