public static class Constants
{
    //Eaten Vlaues for edibles:
    public const int eatenValueLow= 1;
    public const int eatenValueMedium = 10;
    public const int eatenValueHigh = 20;
    public const int eatenValueMax = 50;

    //speed for player and ghosts
    public const float speedMin = 1.6f;
    public const float speedSlow = 2.0f;
    public const float speedFast = 4.0f;
    public const float speedMax = 6.0f;

    //lower bound of the maze complexity that still creats a maze
    public const int initialWallProbability = 40; //40
    //the likly hood that the ghost moves in the 'optimal' direction
    public const int ghostMoveProbability = 65;

    //postions on the screen
    public const int xLeft = -7;
    public const int xRight = 7;
    public const int yTop = 4;
    public const int yBottom = -3; //to accommodate the info bar at bottom
    public const int centre = 0;

    //shifting postion to become an index
    public const int xShift = 7;
    public const int yShift = 3;

    //indexing inside of the maze matricies
    public const int mazeWidth = 15;
    public const int mazeHeight = 8;

    //how Many Edblies Left Before can Move To the Next Level
    public const int howManyEdbliesLeftBeforeMovingToNextLevel = 0; //set back to 0 when finish testing (i.e. shortcut)

    //the path of where the high score table file is stored
    public const string highScoreFilePath = ".\\HighScoreTableFile2.txt"; // at the moment it's; C:\Users\Lauren\Documents\Unity\PACwoman 2
    //public const string highScoreFilePath = "C:\\Users\\Lauren\\Documents\\NEA2019\\HighScoreTableFile1.txt";
}