using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class inherits from the Ghost class (which in turn inherits from the monobehaviour class)
public class GhostNic : Ghost
{

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        gameObject.tag = "ghostNic"; //each ghost has individual tags to identify them
        ghostSpeed = Constants.speedFast;
        originalPosition = new Vector2(Constants.xLeft, Constants.yTop);//Nic has here own home
        GoHome();
    }


}
