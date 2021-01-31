using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ghost : MonoBehaviour {
    
    private int eatenValue, counter = 0;

    public float ghostSpeed, step;
    private bool toggle = true;

    protected Vector2 originalPosition;
    
    //creating an enum used to define directiosn of movement
    private enum Directions {up, down, left, right};
  
    //allows for control over what the sprite looks like 
    protected SpriteRenderer spriteRenderer;
    protected Color originalColour;

    //variables to hold unity components
    private BoxCollider2D bc;
    private Rigidbody2D rb;

    //defining a random number
    System.Random randomNumber = new System.Random();

    private void Awake()
    {
        //attaching the components to gameobject
        bc = gameObject.AddComponent<BoxCollider2D>() as BoxCollider2D;
        bc.isTrigger = false;
        rb = gameObject.AddComponent<Rigidbody2D>() as Rigidbody2D;
        rb.bodyType = RigidbodyType2D.Dynamic;

        step = 1;
        //prevents the gameobject lousing the correct sense of direction, otherwise the gameobject will rotate when caught on corners 
        rb.freezeRotation = true;
        //attaching the renderre of the the sprite  to  'spriteRenderer'
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    protected virtual void Start () {

        gameObject.tag = "ghost";
        eatenValue = Constants.eatenValueMax;
        ghostSpeed = Constants.speedSlow;
        originalPosition = new Vector2(Constants.xRight, Constants.yTop);
        GoHome(); //at the start of the game the ghosts begin @ thier original position 
        originalColour = spriteRenderer.color;//initializing the colour of the sprite
        
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        
        MoveGhost();
        if (Globals.Mode == Globals.Modes.powerPellet)
        {
            //making the ghost 'flash' by toggeling the colour of the sprite
            counter++;
            if (counter == 10) { toggle = !toggle;  counter = 0; }
            if (toggle) { print("                             toogle true"); spriteRenderer.color = Color.white; }
            if (!toggle) { print("                             toogle false"); spriteRenderer.color = originalColour; }

        }
        else { spriteRenderer.color = originalColour; }


        //once at a high enough level the ghosts travel really fast
        if(Globals.LevelNumber > 6) { ghostSpeed = Constants.speedFast; } 

    }

    Directions bestDirection; // declared here so not reset everytime MoveGhost is called
    int count = 0;
    int randomWait = 15; 

    void MoveGhost()
    {

        ++count;
        if (count == randomWait)
        { 
            //only works out a new best direction every 10frames, gives it a chance get somewhere different before changing direction

            count = 0;
            randomWait = randomNumber.Next(10, 25);

            GameObject[] players; //list which holds the x2 player gameobjects
            players = GameObject.FindGameObjectsWithTag("player");
            //defining the positions locally becuase want it to be redeclared every frame
            Vector2 targetPosition, trialPosition;

            //stores the x2 diffrent distances of the players from the ghost
            float[,] distances = new float[Enum.GetValues(typeof(Directions)).Length, 2]; // 2D array with size of first index 4 (the size/length is 4: up down left right) second index size 2 for the number of players

            for (int i = 0; i <= Globals.NumberOfPlayerAlive; i++)
            {
                targetPosition = GetTargetPosition(players[i]);//calculates the tagret positon seperatley for each player

                foreach (Directions dir in Enum.GetValues(typeof(Directions)))
                {
                    //for each direction: up, down, left and right it sees wich of the 4 movements would bring the ghost closer to the target postion(positon of player)
                    trialPosition = GetDeltaPosition(dir, rb.position);
                    distances[(int)dir, i] = DistanceBetween(trialPosition, targetPosition);
                }
            }

            bestDirection = GetBestDirection(distances);
        }

        MoveInDirection(bestDirection);
    }

    Vector2 GetTargetPosition(GameObject target)
    {
        //returns the postion of the GameObject its called with 'target'
        return target.transform.position;
    }

    //works out the x4 new possible postions going; up, down, left, right based on its current position
    Vector2 GetDeltaPosition(Directions dir, Vector2 CurrentPosition)
    {
        switch (dir)
        {
            case Directions.up: 
                 CurrentPosition.y += step;
                break;
            case Directions.down:
                 CurrentPosition.y -= step;
                break;
            case Directions.left:
                 CurrentPosition.x -= step;
                break;
            case Directions.right:
                CurrentPosition.x += step;
                break;

        }

        return CurrentPosition;
    }

    //calculates the distance between x2 postions
    float DistanceBetween(Vector2 trialPosition, Vector2 targetPosition)
    {
        return Mathf.Sqrt(Mathf.Pow(trialPosition.x - targetPosition.x,2) + Mathf.Pow(trialPosition.y - targetPosition.y,2)); //the pow is sqaure btw
       
    }


    Directions GetBestDirection(float[,] distances)
    {
        //best diresction is initialized to up
        Directions best = Directions.up;//the enum (up, down, left or right) of the bext direction
        int besti = 0;//determines which of the players is the closest
     
        if (Globals.Mode == Globals.Modes.normalPlay)//in this mode want the ghost to chase
        {
            //looping through both players
            for (int i = 0; i <= Globals.NumberOfPlayerAlive; i++)
            {
                //looping through all the directions
                foreach (Directions dir in Enum.GetValues(typeof(Directions)))
                {
                    //
                    if (distances[(int)dir,i] < distances[(int)best,besti])       //if there are 2 distances that are the same it will pick the first
                    {
                        if (randomNumber.Next(1, 100) < Constants.ghostMoveProbability)
                        {
                            //only randomly updates the best direction i.e. dosent always move in the best direction
                            //giving it an oppertuniy to escape dead ends that its acught up in
                            best = dir;
                            besti = i;
                        }
                    }

                }
            }
                
                    
        }

        if (Globals.Mode == Globals.Modes.powerPellet) //in this mode want the ghost to run away, otherwise its the same as chase
        {
            for (int i = 0; i <= Globals.NumberOfPlayerAlive; i++)
            {
                foreach (Directions dir in Enum.GetValues(typeof(Directions)))
                {
                    if (distances[(int)dir,i] > distances[(int)best, besti])
                    {
                        if (randomNumber.Next(1, 100) < Constants.ghostMoveProbability)
                        {
                            best = dir;
                            besti = i;
                        }
                            
                    }

                }
            }
        }

        return best;
    }

    
    void MoveInDirection(Directions bestDirection)
    {
       //changes the velocity instead of the position because want to change the movement smoothly 
         switch (bestDirection)
        {
            case Directions.up:
                rb.velocity = new Vector2(0, ghostSpeed);
                break;
            case Directions.down:
                rb.velocity = new Vector2(0, -ghostSpeed);
                break;
            case Directions.left:
                rb.velocity = new Vector2(-ghostSpeed, 0);
                break;
            case Directions.right:
                rb.velocity = new Vector2(ghostSpeed, 0);
                break;

        }
    }



    // --------------------------------------------------------ORIGINAL PSEUDOCODE OF THE ALGORITHM----------------------------------------------------
    //  enum Directions [up, down, left, right]
    //  targetPosition = GetTragetposition(TragetObject)    e.g. player
    //  for dir in Directions
    //     trialPosition = GetDeltaPosition (dir, CurrentPosition)
    //     distance[dir] = DistanceBetween(trialPosition, targetPosition)
    //  end
    //  bestDirection = GetBestDirection(distance[])   dont forget the flag
    //  MoveInDirection(bestDirection)




    //
    //  GetTragetposition(gameobject target)   the simplest vesrion of the the function will just acalculate the current position of the payer. this could be advanced to predict maybe the next position of tghe palyer, or to guess the direction it may go and use that as the position that thegost is targeting. 
    //  return target.transform.postion     work out what type this is!
    //
    //  GetDeltaPosition(Directions dir, transform CurrentPosition)   will current position be passed by value or ny reffrence
    //  switch(dir)
    //      case up 
    //         CurrentPosition.y += step 
    //      case down 
    //         CurrentPosition.y -= step 
    //      case left 
    //         CurrentPosition.x -= step 
    //      case right 
    //         CurrentPosition.x += step 
    //      return CurrentPosition
    //
    //DistanceBetween(transfrom trialPosition, transfrom targetPosition)   the simplest method is to work out the shortest cartesian ditsnace between 2 points, like two coordinates, ignoreing the walls of the maze. another method could be to follow the path of the maze and work out the distance of how far the ghost would actually have to travel around
    //  squareroot((trialPosition.x - targetPosition.x)^2 + (trialPosition.y - targetPosition.y)^2)
    //
    //GetBestDirection(array distance)   depedning on wether the ghsost is running away or chasing 'best' direction may be diffrent (want to get closer or get further away fom the player) also based on the ghosts personalitites we might have them calculate a 'best' move diffrently e.g. using random 
    //using simplest: absolute distance
    //BUBBLE SORT-ISH
    //     if flag == chasing:
    //          best = up
    //          for dir in Directions:         
    //              if distance[dir]  < distance[best]:       if there are 2 distances that are the same it will pick the first--maybe change this
    //                  best = dir
    //          
    //     if flag == chased:
    //          best = up
    //          for dir in Directions:         
    //              if distance[dir]  > distance[best]:   
    //                  best = dir
    //  return best
    //
    //MoveInDirection(Directions bestDirection)    NOT changeing the posiiotn, instead changeing the diretion of the velocity (to make it more smooth). we can also make this more complicated by making it speed up or slow down as it gets closer to the player, or maybe chnage the animation mvement style of the ghost. lost of room for additional adjustments
    //using simplest:
    //  gameObject.Velocity = 0,0
    //  switch(bestDirection)
    //      case up 
    //         gameObject.Velocity.y = ghostSpeed
    //      case down 
    //         gameObject.Velocity.y = -ghostSpeed 
    //      case left 
    //         gameObject.Velocity.x = -ghostSpeed 
    //      case right 
    //         gameObject.Velocity.x = ghostSpeed 


    // --------------------------------------------------------ORIGINAL PSEUDOCODE OF THE ALGORITHM----------------------------------------------------


   //causes the ghost to return back to its 'home' position
    protected void GoHome() 
    {
        transform.position = originalPosition;
    }

    //causes the ghost to change its postion to a random place in the maze, but not too close to the players home
    public void RandomRespawn()
    {
        int positionX;
        int positionY;
        bool badPosition = true;
        while (badPosition)
        {
            //ranodmly generates the x and y value of the position based on the size of the maze
            positionX = randomNumber.Next(Constants.xLeft, Constants.xRight);
            positionY = randomNumber.Next(Constants.yBottom, Constants.yTop-1);
            //dont want the ghost to randomly respwan to close to the ghosts' home, gives the ghost too much of an advantage to kill them
            if ((positionX < -2 || positionX > 2) && (positionY < -1 || positionY > 1)) { badPosition = false;  transform.position = new Vector2(positionX, positionY); }
        }
 
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        
        switch (other.collider.tag) 
        {
            //if an object with the tag of player collides with this gameObject  it increase the score and randomly espawn the ghost to a new position
            case "player": 
                switch (Globals.Mode) //need to test which mode because the action will be differnt based on if its power pellet or not
                {
                        case Globals.Modes.powerPellet:
                        //the players gains points from eating the ghost
                        Globals.Score += eatenValue;
                        //after eaten does not respawn back to their home otherwise it can be taken advantage of
                        RandomRespawn(); 
                        break;
                }

                break;
        }

    }

}

