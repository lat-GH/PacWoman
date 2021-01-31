using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallElement : MonoBehaviour {

    //variables to hold unity components
    private BoxCollider2D bc;
    private Rigidbody2D rb;

    private Vector2 whereX;

    private void Awake() 
    {
        //assigning compenents to the wall elements
        bc = gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;
        bc.isTrigger = false; //when something hits no events should be triggered
        rb = gameObject.AddComponent<Rigidbody2D>() as Rigidbody2D;
        rb.bodyType = RigidbodyType2D.Static; //tops the Rigidbody2D from reacting to gravity or applied forces including contacts with any other Rigidbody2D.
        rb.simulated = true; //does simulate unity's physics


    }

}
