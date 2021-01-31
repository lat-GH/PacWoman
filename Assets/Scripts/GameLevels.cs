using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.SceneManagement;

using System.IO;

        
public class GameLevels : MonoBehaviour {

    private int[,] wallsMatrixH = new int[Constants.mazeWidth, Constants.mazeHeight]; // 15 x 8 (x by y) matrix for the horizintal wall elemnts 0 no wall & 1 is there is a wall
    private int[,] wallsMatrixV = new int[Constants.mazeWidth, Constants.mazeHeight]; // ditto for vertical walls

    /*nodesMatrix: 
     * 16x9 because need an n+1 walls to bound n nodes (need an extra column on the right and an extra row at the bottom) 
     * n-n-n requires 3 nodes but 2 walls
     * the z dimension is for the Up = 0 Right = 1 Down = 2 left = 3 walls at each node  (i.e. increasing clocksiwse will work with modulo 3 to loop round Directions)
    */
    private int[,,] nodesMatrix = new int[Constants.mazeWidth+1, Constants.mazeHeight+1, 4];

    // variables to manage the corrections of closed loops in the maze                                                
    private int[,] nodeVisitCountMatrix = new int[Constants.mazeWidth + 1, Constants.mazeHeight + 1]; // like node mtarix but holding the number of times each node has been visited
    private int[,,] pathTraversalCountMatrix = new int[Constants.mazeWidth + 1, Constants.mazeHeight + 1, 4]; // like the node matrix but holding the count of the number of times that each path for a node has been traversed
   
    //need to be public so that the prefab can be attached to them in the unity window
    public GameObject edibleGameObjcet; //the object for the prefab instantiations
    public GameObject powerPelletGameObject; //public to allow connection to the prefab
    public GameObject wallElementGameObject;

    private GameObject[] Players, ActiveEdibles;
    private GameObject ghostGameObject, ghostPinkyGameObject, ghostClydeGameObject, ghostNicGameObject;
    private GameObject scoreDisplayerGameoject;


    private GameObject[] allEdibleGameObjcets, allWallsGameObjects; //lists of all the edible and wall gameobjects

    //used to positon the edibles and wall elements on the grid when instantiated
    private Vector2 whereX; 
    private Vector2 wallPositionH, wallPositionV;

    //declaring a random number
    System.Random randomNumber = new System.Random();
    private int wallProbability = 0;

    void Start() {

        Globals.Mode = Globals.Modes.normalPlay; //initializing the mode

        //just a 90 degree rotation, so can easily turn wall 90 degrees to go from vertical to horizontal walls
        Quaternion rotation90 = Quaternion.Euler(0, 0, 90);

        //creating the outside walls
        wallElementGameObject.tag = "wallOuter";
        wallElementGameObject.transform.localScale = new Vector2(105, 1.5f);
        Instantiate(wallElementGameObject, new Vector2(0, Constants.yTop+1), transform.rotation);
        Instantiate(wallElementGameObject, new Vector2(0, Constants.yBottom - 1), transform.rotation);
        wallElementGameObject.transform.localScale = new Vector2(60, 1.5f);
        Instantiate(wallElementGameObject, new Vector2(Constants.xRight + 1.5f, 0), rotation90);
        Instantiate(wallElementGameObject, new Vector2(Constants.xLeft - 1.5f, 0), rotation90);

        wallElementGameObject.tag = "wall";

        //now instatiates all the edibles
        CreateEdibles();

        //instatiating the walls of the maze for the first time with an initial wall probability
        try { FreshLevelWalls(Constants.initialWallProbability); }
        catch { print("EXCEPTION: failed to generate the maze, returned to MainControl Scene"); SceneManager.LoadScene(0); }

    }

    void CreateEdibles()
    {
        // intatiates all the edibles and powerpellets

        for (int j = Constants.yBottom-1; j <= Constants.yTop; j++)
        {
            for (int i = Constants.xLeft; i <= Constants.xRight; i++)
            {
                whereX = new Vector2(i, j + 0.5f);
                if (Math.Abs(j) == 3 & Math.Abs(i) == 6)
                {
                    //only instantiates the power pellet in the corners of the maze
                    Instantiate(powerPelletGameObject, whereX, transform.rotation);
                }
                else
                {
                    Instantiate(edibleGameObjcet, whereX, transform.rotation);
                }
            }
        }

        
    }

    void ReActivateAllEdibles()
    {

        for (int i=0; i < allEdibleGameObjcets.Length; i++) { allEdibleGameObjcets[i].SetActive(true); }

    }

    /* first populates the Wall matrices randomly
       calculates the node matrix from the wall matrices
       deloopinator: using the node matrix to identify ad remove any closed loops
       instatiates the wall elemenst based on the resulting loop free wall matrices
    */
    void FreshLevelWalls(int wallProbability)
    { 

        // creating the randomly generated maze and storing it in the coresponding wall matrices H and V
        for (int j = Constants.yBottom; j <= Constants.yTop; j++) //8 
        {
            for (int i = Constants.xLeft; i <= Constants.xRight; i++) //15 
            {
                wallsMatrixH[i + Constants.xShift, j + Constants.yShift] = 0; //shifting postioins into matrix index to name the nodes
                wallsMatrixV[i + Constants.xShift, j + Constants.yShift] = 0; 

                //populates the Wall matrices randomly
                if (randomNumber.Next(1, 100) < wallProbability)
                {
                    wallsMatrixH[i + Constants.xShift, j + Constants.yShift] = 1; 
                }

                if (randomNumber.Next(1, 100) < wallProbability && j < Constants.yTop) //&& so wont do vertivcal bars for the top row
                {
                    wallsMatrixV[i + Constants.xShift, j + Constants.yShift] = 1;
                }

            }
        }
        //calculates the node matrix from the wall matrices
        NodeMatrixCreator();
        
        //deloopinator: using the node matrix to identify ad remove any closed loops
        DeLoopInator();

        //determines the scale of the maze wall elements that are going to be instantiated
        wallElementGameObject.transform.localScale = new Vector2(7, 1.5f);
        //just a 90 degree rotation, so can easily turn wall 90 degrees to go from vertical to horizontal walls
        Quaternion rotation90 = Quaternion.Euler(0, 0, 90);

        for (int j = Constants.yBottom; j <= Constants.yTop; j++) //8 
        {
            for (int i = Constants.xLeft; i <= Constants.xRight; i++) //15 
            {
                wallPositionH = new Vector2(i, j);
                wallPositionV = new Vector2(i - 0.5f, j + 0.5f);

                //instatiates the wall elemenst based on the resulting loop free wall matrices
                if (wallsMatrixH[i + Constants.xShift, j + Constants.yShift] == 1) 
                {
                    Instantiate(wallElementGameObject, wallPositionH, transform.rotation); 
                }

                if (wallsMatrixV[i + Constants.xShift, j + Constants.yShift] == 1) //wont do vertivcal bars for the -3 coordinates i.e bottom row
                {
                    Instantiate(wallElementGameObject, wallPositionV, rotation90);
                }

            }
        }

        if (Globals.DEBUG)
        {
            //DEBUGWriteToFileNodesMatrix("NodeMatrixCreator     Nodes   AFTER DELOOPINATOR"); //DEBUG CALLS
            //DEBUGWriteToFileWallsMatrix("NodeMatrixCreator     Walls   AFTER DELOOPINATOR");
        }
    }

    //destroys all the wall elemenst making up the maze (but not the outter walls)
    void CleanWalls()
    {
        allWallsGameObjects = GameObject.FindGameObjectsWithTag("wall");
        for (int i = 0; i < allWallsGameObjects.Length; i++) { Destroy(allWallsGameObjects[i]); }

    }

    /* NodeMatrixCreator:
        uses the H and V wall Matrices to create a new Node matrix which stores the presnt wall elements at each node in the maze
        for each node there are four elements one for each of thw alls that meet at the node
        USING : Up=0 Right=1 Down=2 left=3 i.e. increasing clocksiwse (will work with modulo 3 to loop round the different Directionss)
    */
    void NodeMatrixCreator()
    {  
        for (int j = 0; j < Constants.mazeHeight + 1; j++) //9 
        {

            for (int i = 0; i < Constants.mazeWidth + 1; i++) //16 
            {
                // inititializing the Count matrcies to 0
                nodeVisitCountMatrix[i, j] = 0;
                for (int k = 0; k <= 3; k++) 
                {
                    pathTraversalCountMatrix[i, j, k] = 0;
                }
               
                // populating the nodesMtarix using the wallsMatrix H and V (corners and edges are special and are dealt with seperately)
                if (i != Constants.mazeWidth && j != Constants.mazeHeight && i != 0 && j != 0) 
                {
                    //BTW: e.g. the Up wall element for one node is the same as the Down wall elemnt for the node above
                    nodesMatrix[i, j, 0] = wallsMatrixV[i, j]; //assigning the up to the node
                    nodesMatrix[i, j, 1] = wallsMatrixH[i, j]; //assigning the right to the node
                    nodesMatrix[i, j, 2] = wallsMatrixV[i, j - 1]; ////assigning the down node 
                    nodesMatrix[i, j, 3] = wallsMatrixH[i - 1, j]; //assigning the left to the node
                }
                //-------------------------------EDGE CASES (where meet the outter walls)----------------------------------
                else if (i == 0 && (j != 0 && j != Constants.mazeHeight)) //left edge excluding corners
                {
                    nodesMatrix[i, j, 0] = wallsMatrixV[i, j]; //assigning the up to the node  
                    nodesMatrix[i, j, 1] = wallsMatrixH[i, j]; //assigning the right to the node
                    nodesMatrix[i, j, 2] = wallsMatrixV[i, j - 1]; ////assigning the down node 
                }
                else if (i == Constants.mazeWidth && (j != 0 && j != Constants.mazeHeight)) //right edge excluding corners
                {
                    nodesMatrix[i, j, 3] = wallsMatrixH[i - 1, j]; //assigning the left to the node
                }
                else if (j==0 && i!=0 && i!= Constants.mazeWidth) //bottom edge excluing corners
                {
                    nodesMatrix[i, j, 0] = wallsMatrixV[i, j]; //assigning the up to the node  
                    nodesMatrix[i, j, 1] = wallsMatrixH[i, j]; //assigning the right to the node
                    nodesMatrix[i, j, 3] = wallsMatrixH[i - 1, j]; //assigning the left to the node
                }
            }

        }
        //----------------------------CORNER CASES--------------------------------------
        //bottom left corner case
        nodesMatrix[0, 0, 0] = wallsMatrixV[0, 0]; //assigning the up to the node 
        nodesMatrix[0, 0, 1] = wallsMatrixH[0, 0]; //assigning the right to the node

        //top left corner case
        nodesMatrix[0, Constants.mazeHeight-1, 1] = wallsMatrixH[0, Constants.mazeHeight - 1]; //assigning the right to the node
        nodesMatrix[0, Constants.mazeHeight - 1, 2] = wallsMatrixV[0, Constants.mazeHeight - 2]; ////assigning the down node 

        //bottom right corner case
        nodesMatrix[Constants.mazeWidth, 0, 3] = wallsMatrixH[Constants.mazeWidth-1, 0]; //assigning the left to the node
        //there are no Vertical wall elements at j=15

        //top right corner case
        nodesMatrix[Constants.mazeWidth, Constants.mazeHeight - 1, 3] = wallsMatrixH[Constants.mazeWidth - 1, Constants.mazeHeight - 1]; //assigning the left to the node
        //there are no Vertical wall elements at j=15

        //DEBUGWriteToFileNodesMatrix("NodeMatrixCreator     Nodes  BEFORE DELOOPINATOR"); //DEBUG CALLS
        //DEBUGWriteToFileWallsMatrix("NodeMatrixCreator     Walls  BEFORE DELOOPINATOR");
    }

    // --------------------------DEBUG START---------------------------------------------------
    // DEBUGWriteToFileNodesMatrix: this is a useful pretty print function to show the NodeMatrix as a maze into a text file
    //like this:
    /*
     NodeMatrixCreator     Nodes
                                    top: 8 :                                                
                                    mid: 8 :                                                
                                    bot: 8 :                                                
                                    top: 7 :                                                
                                    mid: 7 :     -- -- --    -- --                --        
                                    bot: 7 : i        i           i     i  i        i       
                                    top: 6 : i        i           i     i  i        i       
                                    mid: 6 :                                -- --           
                                    bot: 6 :             i  i  i     i     i  i        i    
                                    top: 5 :             i  i  i     i     i  i        i    
                                    mid: 5 :     --    --          --    --       -- -- --  
                                    bot: 5 :                   i           i  i     i  i    
                                    top: 4 :                   i           i  i     i  i    
                                    mid: 4 :           -- --    -- --       -- --           
                                    bot: 4 :             i           i     i  i  i          
                                    top: 3 :             i           i     i  i  i          
                                    mid: 3 :  -- --          --    --          -- -- -- --  
                                    bot: 3 :    i                    i           i     i    
                                    top: 2 :    i                    i           i     i    
                                    mid: 2 :     --       --       --       --              
                                    bot: 2 :                i        i  i              i    
                                    top: 1 :                i        i  i              i    
                                    mid: 1 :        --          --             --       --  
                                    bot: 1 :       i     i     i  i        i        i       
                                    top: 0 :       i     i     i  i        i        i       
                                    mid: 0 :  --    --       --       --    --          --  
                                    bot: 0 :                                                
                                    bot:   :000111222333444555666777888999000111222333444555

     */
    void DEBUGWriteToFileNodesMatrix(string where)
    {
        
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Lauren\Documents\NEA2019\WriteLines4.txt", true))
        {
            file.WriteLine(" "); file.WriteLine(" "); file.WriteLine(" "); file.WriteLine(" "); file.WriteLine(" "); file.WriteLine(where);
        }

        for (int j = Constants.mazeHeight; j > -1; j--) //9 
        {
            string toprow = "                                                             top: " + j + " :";
            string midrow = "                                                             mid: " + j + " :";
            string botrow = "                                                             bot: " + j + " :";

            for (int i = 0; i < Constants.mazeWidth + 1; i++) //16 
            {
                if (nodesMatrix[i, j, 0] == 1) { toprow += " "; toprow += "i"; toprow += " "; } else { toprow += "   "; }
                if (nodesMatrix[i, j, 3] == 1) { midrow += "-"; midrow += " "; } else { midrow += "  "; }
                if (nodesMatrix[i, j, 1] == 1) { midrow += "-"; } else { midrow += " "; }
                if (nodesMatrix[i, j, 2] == 1) { botrow += " "; botrow += "i"; botrow += " "; } else { botrow += "   "; }

            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Lauren\Documents\NEA2019\WriteLines4.txt", true))
            {
                file.WriteLine(toprow); file.WriteLine(midrow); file.WriteLine(botrow);
            }

        }
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Lauren\Documents\NEA2019\WriteLines4.txt", true))
        {
            file.WriteLine("                                                             bot:   :000111222333444555666777888999000111222333444555");
        }
    }

    // DEBUGWriteToFileWallsMatrixthis :is a useful pretty print function to show the Walls Matrices as a maze into a text file
    /*
     * NodeMatrixCreator     Walls
                                        V: 7 :                                             
                                        H: 7 :    -- -- --    -- --                --      
                                        V: 6 :|        |           |     |  |        |     
                                        H: 6 :                               -- --         
                                        V: 5 :            |  |  |     |     |  |        |  
                                        H: 5 :    --    --          --    --       -- -- --
                                        V: 4 :                  |           |  |     |  |  
                                        H: 4 :          -- --    -- --       -- --         
                                        V: 3 :            |           |     |  |  |        
                                        H: 3 : -- --          --    --          -- -- -- --
                                        V: 2 :   |                    |           |     |  
                                        H: 2 :    --       --       --       --            
                                        V: 1 :               |        |  |              |  
                                        H: 1 :       --          --             --       --
                                        V: 0 :      |     |     |  |        |        |     
                                        H: 0 : --    --       --       --    --          --

     * 
     */
    void DEBUGWriteToFileWallsMatrix(string where)
    {
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Lauren\Documents\NEA2019\WriteLines4.txt", true))
        {
            file.WriteLine(" "); file.WriteLine(" "); file.WriteLine(" "); file.WriteLine(" "); file.WriteLine(" "); file.WriteLine(where);
        }

        for (int j = Constants.mazeHeight - 1; j > -1; j--) //9
        {
            string H = "                                                               H: " + j + " :";
            string V = "                                                               V: " + j + " :";

            for (int i = 0; i < Constants.mazeWidth; i++) //16
            {
                if (wallsMatrixV[i, j] == 1) { V += "|  "; } else { V += "   "; }
                if (wallsMatrixH[i, j] == 1) { H += " --"; } else { H += "   "; }

            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Lauren\Documents\NEA2019\WriteLines4.txt", true))
            {
                file.WriteLine(V); file.WriteLine(H);
            }

        }
    }
 // -------------------------------DEBUG END---------------------------------------------------


    //--------MAZE CORRECTION ALGORITHM: ----TO DE-LOOP-IFY-------------------DE-LOOP-INATOR------------------
    //startNode = StartNodeFinder( NodesMatrix)
    //DirectionOfTraversal = ChooseTraversal(startNode,left) // checks for: exsistsnace of wall elements and wether thier node has been visited, chouses the next one going clcokwise
    //loops until completeTraversal
    //  DirectionCameFrom = drictionConversion(DirectionOfTraversal)
    //  newNode = TraverseToNode(DirectionOfTraversal, NodeMatrix) // need to update the nodes mtraix, and which matrix it is trying to currently examine
    //  DirectionOfTraversal = ChooseTraversal(newNode, DirectionCameFrom)


    //ChooseTraversal(currentNode, DirectionCameFrom):
    //  whichWallDoesItHave = []
    //  tempNewNode = NextClockwise(whichWallDoesItHave)
    //  Visited? = CheckifVisted(tempNewNode)
    //  IF Visited? == Flase: //or maybe = 0 (see notes)
    //      return tempNewNode
    //  ELSE:
    //      IF checkPath(tempNewNode) == goingBack: // OR already has a 'forward' to that node
    //          return tempNewNode
    //      ELIF checkPath(tempNewNode) == closedLoop: // OR there is NO 'forward' to that node
    //          wantedWallElement = FindWhichWallElementToDelete(tempNewNode, currentNode )
    //          DeleteWallElement(tempNewNode, wantedWallElement) //deletes the connecting wall element that closes the loop. dont forget needs to be removed 
    //          return currentNode

    void DeLoopInator()
    {

        int iOfNode, jOfNode, iOfNewNode, jOfNewNode;
        int testingCounter1, testingCounter2;//used to make the loops finite, primarily for DEBUG
        int directionOfTraversal, directionCameFromNew;

        //will de-loop-ify for each tree in the maze, when cant find any new trees it will be finished

        //loop over the trees in a maze
        testingCounter2 = 0;
        while (StartNodeFinder(out iStartNode, out jStartNode) == true) //when true means it has found a new tree i.e. a node that has not been visited before
        {
            ++testingCounter2; //keeping track of the number of trees
            //to avoid infinite looping, if error
            if (testingCounter2 > 50) { Exception ex = new Exception("EXCEPTION: testingcounter2 is bigger than 50 i.e. tried to deloop more than 50 trees"); throw ex; } //created my own exception
            if (Globals.DEBUG) { print("                                                              start node: Counter2 " + testingCounter2 + "   " + iStartNode + ", " + jStartNode); }

            ++ nodeVisitCountMatrix[iStartNode, jStartNode]; // increments the visit count by 1 for the start node of the tree

            iOfNode = iStartNode; //initiliaizing the values for the tree traversal loop
            jOfNode = jStartNode;
            directionOfTraversal = 3;//using 3 because want it to start the traversal coming in from the left

            //loops over the nodes in a tree, until completeTraversal of the tree
            testingCounter1 = 0;//how many nodes in a tree
            //ChooseTraversal: picks a new direction to traverse, returns flase when the whole tree has been traversed
            //NOTE: as well as chousing the node to traverse to, ChooseTraversal also breaks any closed maze loops
            while (ChooseTraversal(iOfNode, jOfNode, directionOfTraversal, out iOfNewNode, out jOfNewNode, out directionCameFromNew) == true) 
            {
                ++testingCounter1;
                //to avoid infinite looping, if error
                if (testingCounter1 > 500) {Exception ex = new Exception("EXCEPTION: testingcounter1 is bigger than 500 i.e. tried to deloop a tree with morethan 500 nodes"); throw ex ; } //created my own exception
                if (Globals.DEBUG) { print("     DeLoopInator: new node :  " + iOfNewNode + ", " + jOfNewNode); }

                //once has chosen the new node to traverse to, it moves to it i.e. sets it as the current node
                iOfNode = iOfNewNode;
                jOfNode = jOfNewNode;
                directionOfTraversal = directionCameFromNew;
            }

        }

    }

    //IsNodeAlive: not all nodes have wall elements attached to them, this determines wether or not it has any wall elements
    bool IsNodeAlive(int i, int j)
    {
        int numberOfWallElements = 0;
        for (int k = 0; k <= 3; k++)
        {
            numberOfWallElements += nodesMatrix[i, j, k];
        }
        if (numberOfWallElements > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    int iStartNode, jStartNode; // used to hold the start position of the tree thats being worked on
    // StartNodeFinder: finding a node to start on i.e. for a tree that hasnt yet been de-loop-ified
    bool StartNodeFinder(out int iStartNode, out int jStartNode) 
    {
        // initialized to an invalid reffernce 
        iStartNode = -1;
        jStartNode = -1;

        for (int j = 0; j < Constants.mazeHeight + 1; j++) //9  // loops thourgh all the nodes
        {
            for (int i = 0; i < Constants.mazeWidth + 1; i++) //16
            {
                // checks that the node has walls connected to it AND that has never been visited i.e. start of a tree
                if (IsNodeAlive(i, j) == true && nodeVisitCountMatrix[i, j] == 0)  
                {
                    iStartNode = i;
                    jStartNode = j;
                    return true; // has found a start node
                }

            }


        }
        return false;// has not found a start node, the whole maze has been deloopified
    }

    /* ChooseTraversal: 
     * chooses a candidate direction
     * finds the corresponding node to the direction
     * checks if its has ever been visited before:
     *      never been visted before=okay to traverse to it
     * 
     * 
     */

    bool ChooseTraversal (int iOfNode, int jOfNode, int directionCameFrom, out int iOfNewNode, out int jOfNewNode, out int directionCameFromNew) 
    {
        int tempDirectionNewpath, numberOfWallElementsAtNode=0;

        if (Globals.DEBUG) { print("          NodeMtarix:  " + iOfNode + ", " + jOfNode + ", " + nodesMatrix[iOfNode, jOfNode, 0] + ", " + nodesMatrix[iOfNode, jOfNode, 1] + ", " + nodesMatrix[iOfNode, jOfNode, 2] + ", " + nodesMatrix[iOfNode, jOfNode, 3]); }

        tempDirectionNewpath = NextClockwise(iOfNode, jOfNode, directionCameFrom);//candidate direction
        directionCameFromNew = (tempDirectionNewpath + 2) % 4;//based on the candidate direction it works out what its direction came from would be if it it moved to this node

        if (Globals.DEBUG){ print("          tempDirection:  " + tempDirectionNewpath); }

        UsePathToFindNode(iOfNode, jOfNode, tempDirectionNewpath, out iOfNewNode, out jOfNewNode);//returns candidate node

        //the candidate node have never been visited
        if (CheckifVistedNode(iOfNewNode, jOfNewNode) == false)
        {
            if (Globals.DEBUG) { print("          ENTER succesful traversal to new node"); }

            ++nodeVisitCountMatrix[iOfNewNode, jOfNewNode];
            ++pathTraversalCountMatrix[iOfNode, jOfNode, tempDirectionNewpath];

            if (Globals.DEBUG){ print("          LEAVE succesful traversal to new node"); }

            return true;
        }

        else //candidate node HAS been visited before
        {
            //      check if tree traversal is complete
            bool alldone = true;
            for (int k = 0; k <= 3; k++)
            {
                if (pathTraversalCountMatrix[iOfNewNode, jOfNewNode, k] == 0 && nodesMatrix[iOfNewNode, jOfNewNode, k] > 0) { alldone = false; } 
            }
            if (alldone)
            {
                if (Globals.DEBUG){ print("                                  all done"); }
                return false;
            } 

            //calculates the number of wall elemenets at the node
            for (int k =0; k <= 3; k++) { numberOfWallElementsAtNode += nodesMatrix[iOfNewNode, jOfNewNode, k]; }
            //if back at strat node and allls its wall elements have been visited then
            if (iOfNewNode == iStartNode && jOfNewNode == jStartNode && nodeVisitCountMatrix[iOfNewNode, jOfNewNode] == numberOfWallElementsAtNode)
            {
                if (Globals.DEBUG){ print("                                             ERROR all done"); }
                // finished the tree
                return false;
            }


            //checks if the path its about to traverse has already been traversed, but in the opposite direction
            //this case is NOT A LOOP
            if (CheckifVistedNode(iOfNewNode, jOfNewNode) == true && pathTraversalCountMatrix[iOfNewNode, jOfNewNode, (tempDirectionNewpath + 2) % 4] == 1)
            {
                ++nodeVisitCountMatrix[iOfNewNode, jOfNewNode];
                ++pathTraversalCountMatrix[iOfNode, jOfNode, tempDirectionNewpath];
                return true;
            }

            // the node has been visited and the path has never been traversed
            //this case IS A LOOP, need deloopifying
            else if (CheckifVistedNode(iOfNewNode, jOfNewNode) == true && pathTraversalCountMatrix[iOfNewNode, jOfNewNode, (tempDirectionNewpath + 2) % 4] == 0) 
            {
                //deletes the connecting wall element that closes the loop
                DeleteWallElement(iOfNode, jOfNode, tempDirectionNewpath, iOfNewNode,jOfNewNode);

                if (Globals.DEBUG){ print("                                                                                                            Wall has been deleted " + iOfNode + " " + jOfNode + " " + tempDirectionNewpath); }
                iOfNewNode = iOfNode;
                jOfNewNode = jOfNode;
                directionCameFromNew = directionCameFrom;
                return true;
            }

        }

        return false;

    }

    //calculates the next clockwise direction from the one you came in on
    int NextClockwise(int iOfNode, int jOfNode, int directionCameFrom)
    {
        int directionGoingTo;
        for (int a = 1; a <= 4; a++)
        {

            directionGoingTo = (directionCameFrom + a) % 4; 
            if (nodesMatrix[iOfNode, jOfNode, directionGoingTo] == 1)
            {
                return directionGoingTo;
            }
        }

        //should never get here because would have checkecked that the node is alive first, will always have at least one path to go down even if its the one it just came from
        return directionCameFrom; 
    }

    void UsePathToFindNode(int iOfNode, int jOfNode, int direction, out int iOfNewNode, out int jOfNewNode)
    {
        int i = -1;
        int j = -1; 
        if(direction == 0) //up
        {
            i = iOfNode;
            j = jOfNode + 1;
        }

        else if (direction == 1)//right
        {
            i = iOfNode + 1;
            j = jOfNode;
        }

        else if (direction == 2)//down
        {
            i = iOfNode;
            j= jOfNode - 1;
        }

        else if (direction == 3)//left
        {
            i = iOfNode - 1;
            j = jOfNode;
        }
        iOfNewNode = i;
        jOfNewNode = j;
    }

    bool CheckifVistedNode(int iOfNode, int jOfNode) //returns false ONLY if has never been visited
    {
       if( nodeVisitCountMatrix[iOfNode, jOfNode] == 0)
        {
            return false;
        }
        return true;
    }


    // DeleteWallElement; removes the wall element from all the matrices
    void DeleteWallElement(int iOfNode, int jOfNode, int DirectionPath, int iOfNewNode, int jOfNewNode)
    {
        pathTraversalCountMatrix[iOfNode, jOfNode, DirectionPath] = 0; 
        pathTraversalCountMatrix[iOfNewNode, jOfNewNode, (DirectionPath + 2) % 4] = 0;

        nodesMatrix[iOfNode, jOfNode, DirectionPath] = 0;
        nodesMatrix[iOfNewNode, jOfNewNode, (DirectionPath + 2) % 4] = 0;

        if (DirectionPath == 0)
        {
            wallsMatrixV[iOfNode, jOfNode] = 0;//removing the Up wall element from the node
        }
        if (DirectionPath == 1)
        {
            wallsMatrixH[iOfNode, jOfNode] = 0;//removing the Right wall element from the node
        }
        if (DirectionPath == 2)
        {
            wallsMatrixV[iOfNode, jOfNode-1] = 0;//removing the Down wall element from the node
        }
        if (DirectionPath == 3)
        {
            wallsMatrixH[iOfNode-1, jOfNode] = 0;//removing the Left wall element from the node
        }
  
    }

    bool neverbefore = true;
    int wait = 0;
   
    void Update () {
        //first time through it attaches all the other gameobjects it needs (from different classes): edibles & ghosts
        ActiveEdibles = GameObject.FindGameObjectsWithTag("edible");
        if (neverbefore)
        {
            neverbefore = false;
            // asigning all the edibles to this list of gameobjects
            allEdibleGameObjcets = GameObject.FindGameObjectsWithTag("edible");

            //asigning all the ghosts to gameobjects
            ghostGameObject = GameObject.FindGameObjectWithTag("ghost"); 
            ghostPinkyGameObject= GameObject.FindGameObjectWithTag("ghostPinky");
            ghostClydeGameObject = GameObject.FindGameObjectWithTag("ghostClyde");
            ghostNicGameObject = GameObject.FindGameObjectWithTag("ghostNic");

            //at the start of the game, first level, only want one ghost active
            ghostPinkyGameObject.SetActive(false);
            ghostClydeGameObject.SetActive(false);
            ghostNicGameObject.SetActive(false);

        }

        //-----------------MANAGING LEVELS AND END OF GAME-------------------
        if (Globals.Mode == Globals.Modes.gameOver)
        {
            ++wait;
            //waits a couple of seconds to display end of game message before returing to the main control
            if (wait > 250)
            {
                //deactivating the GUI text
                scoreDisplayerGameoject = GameObject.FindGameObjectWithTag("scoreDisplayer");
                scoreDisplayerGameoject.SetActive(false);

                //checking it the Score qaulifies for the High SCore table:
                //if thers isnt 10 values in high score table yet, you qualify anway
                if (Globals.NumberOfHighScores < 10) { SceneManager.LoadScene(3); }
                // (there are 10 values already in the tabel) 
                //and if thier score is high enough i.e. eligible for a high score, then takes the user to the Input high score Scene
                else if (Globals.LowestHighScore < Globals.Score) { SceneManager.LoadScene(3); } 
                //if not eligible to get onto the High Score Table then returns the user to the Main control Scene
                else { SceneManager.LoadScene(0); }
            }
        }
        //only game over when both the players have lost all thier lives
        else if (Globals.NumberOfLivesLeft[1] == 0 && Globals.NumberOfLivesLeft[2] == 0 )
        {
            Globals.Mode = Globals.Modes.gameOver;

        }
    
        else if (Globals.Mode == Globals.Modes.endLevel)
        {
            ++wait;
            if (wait > 80)
            {
                wait = 0;
                try { NewLevel(); }
                catch { print("EXCEPTION: failed to generate the a new level, returned to MainControl Scene"); SceneManager.LoadScene(0); }
            }
        }
        //once eaten all the edibles (including powerpellets) can porgress to the next level
        else if (ActiveEdibles.Length <= Constants.howManyEdbliesLeftBeforeMovingToNextLevel) 
        {
            Globals.Mode = Globals.Modes.endLevel;

        }
    }

    /*NewLevel: 
     * moves the player back to home position
     * reactivates all the Edibles
     * regenerates the walls of the maze
     * adds new ghosts depending on the level
     */
    void NewLevel()
    {
        //moves the player back to home position
        Players = GameObject.FindGameObjectsWithTag("player");
        for (int i = 0; i < Players.Length; i++) { Players[i].GetComponent<Player>().GoHome(); }

        //reactivates all the Edibles
        
        ReActivateAllEdibles();
        ActiveEdibles = GameObject.FindGameObjectsWithTag("edible");
        Globals.NumberOfEdiblesLeft = ActiveEdibles.Length;

        //regenerates the walls of the maze
        CleanWalls();
        //as the levels gets higher,  increases the density of the wall elements in the maze i.e. making the maze harder
        wallProbability = 40 + (int)(Globals.LevelNumber * 0.8);
        //doesnt let the maze get more dense than 70 though because the maze is no longer fun
        if (wallProbability > 70) { wallProbability = 70; }
        FreshLevelWalls(wallProbability);

        ++Globals.LevelNumber;

        //as level progresses increase he number of ghosts
        ghostGameObject.GetComponent<Ghost>().RandomRespawn(); // at each new level the ghost randomly respawns to a postion
        if (Globals.LevelNumber>=0) { ghostPinkyGameObject.SetActive(true); ghostPinkyGameObject.GetComponent<Ghost>().RandomRespawn(); } 
        if (Globals.LevelNumber >=3) { ghostClydeGameObject.SetActive(true); ghostClydeGameObject.GetComponent<Ghost>().RandomRespawn(); }
        if (Globals.LevelNumber >=5) { ghostNicGameObject.SetActive(true); ghostNicGameObject.GetComponent<Ghost>().RandomRespawn(); }

        Globals.Mode = Globals.Modes.normalPlay; //sets it back to normal play once completed all the editing for the new level
        
    }

}
