using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

   

    protected int playerID; //required to destinguish between player 1 and player 2

    public float playerSpeed;
    private Vector2 originalPosition;

    //variables to hold unity components
    private BoxCollider2D bc;
    private Rigidbody2D rb;

    private void Awake() 
    {
        //attaching the componenets to the gameobjects
        bc = gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;
        bc.isTrigger = false; //when true means the objects collide
        rb = gameObject.AddComponent<Rigidbody2D>() as Rigidbody2D;
        // Dynamic behaviour causes the Rigidbody2D to react to gravity and applied forces including contacts with other dynamic or Kinematic Rigidbody2D
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.simulated = true;
        rb.freezeRotation = true; //otherwise the gameobject will rotate when caught on corners so the keys you press no longer correspond to the Directions you would expect
  
    }


   
    //protected allows it to be overwritten but still kept private 
    protected virtual void Start ()
    {
        //want the properties of a dynamic game object but dont want it to react to gravity, so set gravity to 0
        Physics2D.gravity = new Vector2(0, 0);

        playerID = 1;
        gameObject.tag = "player";
        playerSpeed = Constants.speedFast;
        Globals.NumberOfLivesLeft[playerID] = 3;
        Globals.NumberOfPlayerAlive = 1; // index0 is player 1 and index1 is player 2 
        Globals.Score = 0; //initializing the score of the team
        originalPosition = new Vector2(Constants.centre, Constants.centre); //the players' 'home' 
        GoHome();//players start at home
    }

    protected virtual void FixedUpdate ()
    {
        //controls the players' movement based on the arrow keys
        transform.Translate(Input.GetAxis("Horizontal-1") * Time.deltaTime * playerSpeed, Input.GetAxis("Vertical-1") * Time.deltaTime * playerSpeed, 0f);
    }

    public void GoHome()
    {
        // moves the player object to originalPosition
        transform.position = originalPosition;
      
    }

    private void DeadCheck()
    {
        //once all lives have been lost, deactivates the player gameObjects
        if (Globals.NumberOfLivesLeft[playerID] <= 0)
        {
            gameObject.SetActive(false);
            -- Globals.NumberOfPlayerAlive;
        }
    }

    //OnCollisionEnter2D :  
    //Sent when an incoming collider makes contact with this object's collider.
    //Further information about the collision is reported in the Collision2D parameter passed during the call

    private void OnCollisionEnter2D(Collision2D other)//other reffres to whatever object has collided with it
    {
        //if the tag of the 'other' is one of the ghosts then it take away o alife of the player and returns them home
        switch (other.collider.tag) 
        {
            case "ghost": 
            case "ghostPinky":
            case "ghostClyde":
            case "ghostNic":
                switch (Globals.Mode)
                {
                    case Globals.Modes.normalPlay://if the mode was powerpellet then the player would eat the ghost not louse a life
                        --Globals.NumberOfLivesLeft[playerID];
                        GoHome();
                        DeadCheck();
                        break;
                }

            break;//the required syntax for a case statement in c#

        }

    }
}
