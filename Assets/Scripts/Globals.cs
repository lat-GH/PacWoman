public static class Globals //static meaning that all the varibles in this class are unique 
//written as a collection of functiosn for accessing each of the variables
{
    //allows me to only display certian things in the console whilst im doing testing
    private static bool dEBUG = false;
    public static bool DEBUG 
    {
        get
        {
            return dEBUG;
        }
        set
        {
            dEBUG = value;
        }
    }

    // ======================= MAIN CONTROL =============================
    //relevant over persisting instances of the game

    private static int lowestHighScore, numberOfHighScores;

    private static string teamName;

    public static string TeamName
    {
        get
        {
            return teamName;
        }
        set
        {
            teamName = value;
        }
    }

    public static int LowestHighScore
    {
        get
        {
            return lowestHighScore;
        }
        set
        {
            lowestHighScore = value;
        }
    }

    public static int NumberOfHighScores
    {
        get
        {
            return numberOfHighScores;
        }
        set
        {
            numberOfHighScores = value;
        }
    }


    // ======================= GAME FRAME =============================
    //relevant each time a new game is played

    private static int score = 0, levelNumber = 0, numberOfPlayerAlive;
    public static int[] NumberOfLivesLeft = new int[3]; //so could index the players as 1 and 2 (and dont use 0)

    public static int Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
        }
    }
   
    public static int LevelNumber
    {
        get
        {
            return levelNumber;
        }
        set
        {
            levelNumber = value;
        }
    }
    public static int NumberOfPlayerAlive
    {
        get
        {
            return numberOfPlayerAlive;
        }
        set
        {
            numberOfPlayerAlive = value;
        }
    }


    // ======================= LEVEL =============================
    //relevant only during each individual level
    
    // declared the varibale of types Modes, mode can be any one of the things from the enum declartion above, depending of the mode diffrent actions need to be carried out, alsomost a sort of finfit state machine
    public enum Modes {normalPlay, powerPellet, endLevel, gameOver };
    private static Modes mode; 
    private static int  numberOfEdiblesLeft;

  public static Modes Mode 
    {
        get
        {
            return mode;
        }
        set
        {
            mode = value;
        }
    }

    public static int NumberOfEdiblesLeft
    {
        get
        {
            return numberOfEdiblesLeft;
        }
        set
        {
            numberOfEdiblesLeft = value;
        }
    }

    

}
