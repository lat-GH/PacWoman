using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDisplayer : MonoBehaviour {

    private int count;

    //creating an object of type GUIStyle used to keep record of the porperties/characteristic of the text that is displayed
    private GUIStyle guiStyle = new GUIStyle(); 


    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);  // So that ScoreDisplay object is retained across Scenes
    }
	
	

    void OnGUI() //OnGUI is called every frame, so kept up to date dynamically
    {
        int d = 150;
        
        //displays at the bottom the game screen whilst playing
        GUI.Label(new Rect(100, 250+d, 100, 20), "Team Score: ");
        GUI.Label(new Rect(200, 250+d, 100, 20), Convert.ToString(Globals.Score));

        GUI.Label(new Rect(250, 250+d, 100, 20), "Lives P1:");
        GUI.Label(new Rect(315, 250+d, 100, 20), Convert.ToString(Globals.NumberOfLivesLeft[1]));

        GUI.Label(new Rect(250, 235+d, 100, 20), "Lives P2:");
        GUI.Label(new Rect(315, 235+d, 100, 20), Convert.ToString(Globals.NumberOfLivesLeft[2]));



        GUI.Label(new Rect(350, 250+d, 100, 20), "Edibles: ");
        GUI.Label(new Rect(400, 250+d, 100, 20), Convert.ToString(Globals.NumberOfEdiblesLeft));


        GUI.Label(new Rect(350, 235+d, 100, 20), "Level:  ");
        GUI.Label(new Rect(400, 235+d, 100, 20), Convert.ToString(Globals.LevelNumber)); 


        //only displays this message when its the end of the game
        if (Globals.Mode == Globals.Modes.gameOver)
        {
            guiStyle.fontSize = 40;
            GUI.Label(new Rect(100, 100, 200, 200), "GAME OVER!!!", guiStyle);
        }
        //displays this message at the end of every level
        if(Globals.Mode == Globals.Modes.endLevel) {GUI.Label(new Rect(100, 100, 300, 100), "Well done you have completed Level:  "); GUI.Label(new Rect(320, 100, 200, 100), Convert.ToString(Globals.LevelNumber));  }
       
    }
}
