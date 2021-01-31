using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*Cherry:  This is a variant of Edible
  its activation is triggered by a Timer so that it appears at random positions for a while then disappears
  It also differs from a standard edible because it doesnt need to be eaten to end a level and it is worth more */
public class Cherry : Edible { //inherits edible class (instead of monobehaviour)

    //declaring a random number
    System.Random randomNumber = new System.Random();
    
    //the cherry is worth more points than a standard edible
    public override void SetScoreValue()
    {
        scoreValue = Constants.eatenValueHigh; 
    }

    //Start: dont use the base function as it has multiple subtle differences from standard Edible:
    /* dont want Cherry to contribute to the number of edibles
       becasuse game uses the number of edibles left to determine the end of a level
       (i.e. dont want the change in level to be dependent on how many cherries have been eaten) */
    protected override void Start()
    {
        
        gameObject.tag = "cherry";
        SetScoreValue();
        //not instatiated like the other edibles, so need to activate the gameObject here
        gameObject.SetActive(true);
       
    }

    //CherryPosition: the cherry's position is randomly generated
    //
    public void CherryPosition() 
    {
        
        transform.position = new Vector2((float)(int)randomNumber.Next(-7, 7), (float)(int)randomNumber.Next(-3, 3)+0.5f); 
        
    }

    //when hit by a player the object is deactivated and the score is increased 
    //but unlike base class Edible the NumberOfEdiblesLeft count is not decreased (hence need to override)
    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "player")
        {

            gameObject.SetActive(false);
            Globals.Score += scoreValue;
        }
    }
}
