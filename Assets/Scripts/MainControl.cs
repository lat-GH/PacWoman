using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainControl : MonoBehaviour {

    //numberOfHighScores = how many values(groups of name, score and date) will be displayed in the high score table, numberOfEntries = how many items and or lines read in form the file
    // if there arent any values read in you want it to decide that it this high score is at the end of the empty list, 
    //want such a big number so that the lowest high score that the user eneters will BE the new lowest
    int numberOfHighScores, numberOfEntries, lowestHighScore = 999999999;

    public static string[] linesOfTable = new string[]{};
    //reqire eleven for the case where there is a full list and need to push off the last item and input the users score into the 10th position 
    public static string[,] highScoreTableArry = new string[11, 3]; // only top ten high scores in the table: 


    // Use this for initialization
    void Start ()
    {
        //unity opertation so that the objects is retained when a new scene is loaded
        DontDestroyOnLoad(gameObject);
        ReadTableFromFile();
    }
	
    //loads the file contenets at the start so that the lowestHighscore can be initialized when the game is first played
    void ReadTableFromFile()
    {

        try { linesOfTable = System.IO.File.ReadAllLines(@Constants.highScoreFilePath); } 
        catch
        {
            //if cant find the file then sets the file to empty
            print("EXCEPTION : No file found: Highscore Table");
            //begins as if the file wa empty`
            numberOfEntries = 0;
            numberOfHighScores = 0;
            lowestHighScore = 999999999;
        }

        if (Globals.DEBUG) { print("has read the file which reads:"); }

        //VALIDATION : if the files content does not match the expected file format then will ignore it, as if it was all corrupted
        numberOfEntries = linesOfTable.Length;
        if (numberOfEntries % 3 != 0) { print("EXCEPTION: numberOfEntries is not multiple of 3 "); numberOfEntries = 0; }//detecteing if not triplets
        if (numberOfEntries > 30) { print("EXCEPTION: incorrect number of lines"); numberOfEntries = 0; }//detecting if the file is too long 


        //populating the 2D highScoreTableArry from the contents of the file
        for (int i = 0; i < numberOfEntries; i++)//if numberOfEntries = 0 then doesnt go into this loop at all
        {
            highScoreTableArry[(i / 3), i % 3] = linesOfTable[i];
        }

        numberOfHighScores = numberOfEntries / 3;//each high score has 3 enteries

        //checking wether the score is interpratble as an intger AND saves the lowestHighScore
        for (int i = 0; i < numberOfHighScores; i++)
        {
            int INTtest;
            try { INTtest = int.Parse(highScoreTableArry[i, 1]); }
            catch { print("EXCEPTION : ReadTableFromFile : highscoretable score was NOT an int"); numberOfEntries = 0; INTtest = 999999999; } //handles tha case where the score string isnt a number and rejects the highscores file data
            if (INTtest <= lowestHighScore)
            {
                lowestHighScore = INTtest;
            }
            else //handles the case that the values of the score are not in descending order and rejects the highscores file data 
            {
                if (Globals.DEBUG) { print("                          OUT OF ORDER " + lowestHighScore + "  " + INTtest); } 
                numberOfEntries = 0;
                numberOfHighScores = 0;
                lowestHighScore = 999999999;
                break;
            } 
        }

        if (Globals.DEBUG)
        {
            print("                                                                       Highscore arrayus BEFORE:");
            for (int i = 0; i < 10; i++)
            {
                print("                                                                 " + highScoreTableArry[i, 0] + " " + highScoreTableArry[i, 1] + " " + highScoreTableArry[i, 2]);
            }
        }

        //initializing the empty cells in the list highScoreTableArry to be empty strings so still displays the empty strings instead of the string from the previous file that are no longer there
        for (int i = numberOfHighScores; i < 10; i++) { for (int j = 0; j < 3; j++) { highScoreTableArry[i, j] = ""; } } 

        //saving the result, so can be used in other scenes
        Globals.LowestHighScore = lowestHighScore;
        Globals.NumberOfHighScores = numberOfHighScores;
 
    }
}
