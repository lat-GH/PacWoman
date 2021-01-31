using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class inherits from the Ghost class (which in turn inherits from the monobehaviour class)
public class GhostPinky : Ghost {

	// Use this for initialization
	protected override void Start () {
        base.Start();
        gameObject.tag = "ghostPinky"; //each ghost has individual tags to identify them
        ghostSpeed = Constants.speedMin;
        originalPosition = new Vector2(Constants.xLeft, Constants.yBottom);//Pinky has his own home
        GoHome();
    }
	
	
}
