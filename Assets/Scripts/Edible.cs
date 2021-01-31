using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edible : MonoBehaviour {

    //the value that the score increases by when eaten
    protected int scoreValue;
    public bool eaten;

    //variables to hold unity components
    private BoxCollider2D bc; 
    private Rigidbody2D rb;

    private void Awake()
    {
        //attaching the componenets to the gameobjects
        bc = gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;
        bc.isTrigger = true; //when true means the objects collide
        rb = gameObject.AddComponent<Rigidbody2D>() as Rigidbody2D;
        //kinematic means that the rigid body is nolonger affected by forces  and effects others when they're collided with
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    protected virtual void Start ()
    {

        gameObject.tag = "edible"; //attaching a tag to this gameObject
        ++Globals.NumberOfEdiblesLeft;//every time an edible is instatiated the NumberOfEdiblesLeft is increemenetd by one
        SetScoreValue();
    }
	
    public virtual void SetScoreValue()
    {
        scoreValue = Constants.eatenValueLow; 
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "player")
        {
            // if the object that collides with the edbile has a tag of player then
            //deactivates it, deacrese the  NumberOfEdiblesLeft and increase the players score based on the value of the edible that it colldied with
            gameObject.SetActive(false);
            --Globals.NumberOfEdiblesLeft;
            Globals.Score += scoreValue;
        }
    }
 }

