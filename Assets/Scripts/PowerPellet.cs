using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class inherits from the Edibles class (which in turn inherits from the monobehaviour class)
public class PowerPellet : Edible
{
    public GameObject Timer;

    public override void SetScoreValue()
    {
        scoreValue = Constants.eatenValueMedium; 
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        //completets the original OnTriggerEnter2D function inherited from edibles before adding in the extra functionality
        base.OnTriggerEnter2D(other);
        
        if (other.tag == "player")
        {
            Globals.Mode = Globals.Modes.powerPellet;//inistiates the power pellet mode
            Timer = GameObject.FindWithTag("timer");//begins the call for the 10 second count down
             //Generic signature for access a methods on another GameObjectc in unity: 
                //      myObject.GetComponent<MyScript>().MyFunction(); 
                //i.e. you get the script component (Timer) from the GameObject (Timer) then call the function (SetTimer) from the script
            Timer.GetComponent<Timer>().SetTimer(10f);
        }
    }
}
