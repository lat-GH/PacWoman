using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnGUI()
    {
        //Getting the inputs from each text field and storing them as strings

        GUI.Label(new Rect(100, 350, 100, 20), "Player1 Score: ");
        //GUI.Label(new Rect(600, 350, 100, 20), "Player2 Score: ");
        //GUI.Label(new Rect(100, 370, 100, 20), player1String);
        //GUI.Label(new Rect(600, 370, 100, 20), player2String);
        GUI.Label(new Rect(200, 350, 100, 20), Convert.ToString(Globals.Score));
        //GUI.Label(new Rect(700, 350, 100, 20), m_Player2ScoreString);

    }
}
