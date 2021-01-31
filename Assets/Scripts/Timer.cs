using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer : MonoBehaviour {

    public float timerLength, timerLengthCherry;
    public bool coutnDown;

    public GameObject CherryGameobject;


    void Start ()
    {
        //initialises the timer
        timerLength = 0;
        coutnDown = false;
        //finds and assignings the cherry game objects
        CherryGameobject = GameObject.FindWithTag("cherry");

    }
	
	void Update ()
    {
        if (coutnDown == true)
        {
            //decreases the timer length by one eveery second
            timerLength -= Time.deltaTime;
            if (timerLength <= 0)
            {
                //once timer has run out; initialises the timer and mode back to normal
                timerLength = 0;
                Globals.Mode = Globals.Modes.normalPlay;
                coutnDown = false;
            }
        }
        
        CherryTimer();

    }

    public void SetTimer(float duration)
    {
        if (Globals.Mode == Globals.Modes.powerPellet) 
        {
            //adds to the count down so that it can begin
            timerLength += duration;
            coutnDown = true;
        }
    }

    void OnGUI()
    {
        //display the timer at the bottom of the game screen with scor and lives ect...
        GUI.Label(new Rect(20, 250+150, 100, 20), "Timer ");
        GUI.Label(new Rect(60, 250+150, 100, 20), Convert.ToString((int)timerLength));
    }

    
    void CherryTimer() //the random respawn of the cherry is NOT depenednet on whether or not it has been eaten by the player
    {
        //adds i to the timer very sceond
        timerLengthCherry += Time.deltaTime;
        if (timerLengthCherry > 10)
        {
            //this TOGGLES the cherry active/inactive
            CherryGameobject.SetActive(!CherryGameobject.activeSelf);//if alreday active then set inactive, but if not active then activatets it

            if (CherryGameobject.activeSelf == true) //will only call when cherry is active
            {
                //causes the cherry to randomly respawn into a new postion 
                //Generic signature for access a methods on another GameObjectc in unity: 
                //      myObject.GetComponent<MyScript>().MyFunction(); 
                //i.e. you get the script component (Cherry) from the GameObject (CherryGameobject) then call the function (CherryPosition) from the script
                CherryGameobject.GetComponent<Cherry>().CherryPosition();
      
            } 

            // initialises the timer
            timerLengthCherry = 0;    
            
        }

    }

    }

