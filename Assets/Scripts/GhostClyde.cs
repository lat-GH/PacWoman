using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//this class inherits from the Ghost class (which in turn inherits from the monobehaviour class)
public class GhostClyde : Ghost
{
    System.Random randomSpeed = new System.Random();
    int count;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        gameObject.tag = "ghostClyde"; //each ghost has individual tags to identify them
        originalPosition = new Vector2(Constants.xRight, Constants.yBottom); // clyde has his own home
        GoHome();
    }

    private void Update()
    {
        ++count;
        // changes the speed randomly every 100 frames; his 'personality' is unpredicatble speed
        if (count > 100) { ghostSpeed = randomSpeed.Next((int)Constants.speedMin, (int)Constants.speedFast); count = 0; } 
    }
    
}

