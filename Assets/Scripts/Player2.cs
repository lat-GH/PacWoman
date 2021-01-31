using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class inherits from the Player class (which in turn inherits from the monobehaviour class)
public class Player2 : Player {

	protected override void Start () {
        base.Start();//runs the original start on the Player object/script
        playerID = 2;
        Globals.NumberOfLivesLeft[playerID] = 3;
    }
	
	
    protected override void FixedUpdate()
    {
        //player 2 uses the keys awsd to control the movement of the player
        transform.Translate(Input.GetAxis("Horizontal-2") * Time.deltaTime * playerSpeed, Input.GetAxis("Vertical-2") * Time.deltaTime * playerSpeed, 0f);
    }
}
