using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


public class HighScoreTable : MonoBehaviour
{
    //numberOfHighScores = how many values(groups of name, score and date) will be displayed in the high score table, numberOfEntries = how many items and or lines read in form the file
    // if there arent any values read in you want it to decide that it this high score is at the end of the empty list, 
    //want such a big number so that the lowest high score that the user eneters will BE the new lowest
    int numberOfHighScores, numberOfEntries, lowestHighScore = 999999999;

    public static string[] linesOfTable = new string[]{};
    //reqire eleven for the case where there is a full list and need to push off the last item and input the users score into the 10th position 
    public static string[,] highScoreTableArry = new string[11, 3]; // with the format: name, score, date 
    
    bool neverbefore = true;

    void Start()
    {
        //loads the file contenets at the start so that the lowestHighscore can be initialized 
        ReadTableFromFile();
    }


    void Update()
    {
        //only gets called once
        if (neverbefore)
        {
            neverbefore = false;
            SortTable();
            WriteTableToFile();

        }

    }

    
    void ReadTableFromFile()
    {
        //tries to read the file
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

        //VALIDATION : if the files content does not match the expected file format then will ignore, as if it was all corrupted
        numberOfEntries = linesOfTable.Length;
        if (numberOfEntries % 3 != 0) { print("EXCEPTION: numberOfEntries is not multiple of 3 "); numberOfEntries = 0; }//detecteing if not triplets
        if (numberOfEntries > 30) { print("EXCEPTION: incorrect number of lines"); numberOfEntries = 0; }//detecting if the file is too long  3*max instaed of 30


        //populating the 2D highScoreTableArry from the contents of the file
        for (int i = 0; i< numberOfEntries; i++) //if numberOfEntries = 0 then doesnt go into this loop at all
        {
            highScoreTableArry[(i / 3), i % 3] = linesOfTable[i]; //the second dimesion will only have three items in them
        }

        numberOfHighScores = numberOfEntries / 3; // each high score has 3 enteries

        
        //checking whether the score is interpratble as an intger AND saves the lowestHighScore
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

    void SortTable()
    {
        int intScore;
        bool hasItBeenAdded = false;
        if (Globals.DEBUG){ print("                               Began sort table"); }

        //inserting the users name,score,date into the list an shuffles down the scores below it
       for(int i = 0; i < numberOfHighScores; i++)
        {
            try { intScore = int.Parse(highScoreTableArry[i, 1]); } //this is double caustionary, should have already been dealt with
            catch { print("EXCEPTION: SortTable : highscoretable score was not an int"); throw; } //handles tha case where the score string isnt a number, throws, should never happen as have just cleaned it in readfile

            if (Globals.Score > intScore)
            {   
                //have an 11th element for when have a full table, but the number of High scores is kept to 10. this also handles the case when its not full and want to add a value onto the end              
                for (int j = (numberOfHighScores); j>i ; j--) 
                {
                    highScoreTableArry[j, 0] = highScoreTableArry[j-1, 0];//shift all the values down by 1
                    highScoreTableArry[j, 1] = highScoreTableArry[j-1, 1];
                    highScoreTableArry[j, 2] = highScoreTableArry[j-1, 2];

                }

                //assiging the users values into the correct position
                highScoreTableArry[i, 0] = Globals.TeamName;
                highScoreTableArry[i, 1] = (Globals.Score).ToString();
                highScoreTableArry[i, 2] = DateTime.Today.ToString("dd/MM/yy");

                // increasing the number of entries after adding the users values (unless alreday at max)
                if (numberOfHighScores < 10) { numberOfHighScores = numberOfHighScores + 1; numberOfEntries = numberOfEntries + 3; } 

                hasItBeenAdded = true;
                break;
            };
        }

        //for the case when the list/file is not full AND users score has not added to the list yet: it just populates the end of the list
        if (numberOfHighScores < 10 && hasItBeenAdded == false)
        {
            highScoreTableArry[numberOfHighScores, 0] = Globals.TeamName;
            highScoreTableArry[numberOfHighScores, 1] = (Globals.Score).ToString();
            highScoreTableArry[numberOfHighScores, 2] = DateTime.Today.ToString("dd/MM/yy");

            numberOfHighScores = numberOfHighScores + 1; numberOfEntries = numberOfEntries + 3;// increasing the number of entries once the values have been added to the list
        }

        if (Globals.DEBUG)
        {
            print("                                                                       Highscore arrayus AFTER:");
            for (int i = 0; i < numberOfHighScores; i++)
            {
                print("                                                                 " + highScoreTableArry[i, 0] + " " + highScoreTableArry[i, 1] + " " + highScoreTableArry[i, 2]);
            }
        }

    }

    void WriteTableToFile()
    {
        if (Globals.DEBUG) { print("                                                                should now write to file"); }

        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@Constants.highScoreFilePath, false)) 
        {
            //overwriting numberOfHighScores back into the original file
            for (int i = 0; i < numberOfHighScores; i++)
            {
                for (int j = 0; j <3; j++) // still want it to be doing it in 3s beacsue the data is grouped int name,score,date
                {
                    file.WriteLine(highScoreTableArry[i, j]); // writes each entery on a new line
                }
            }
        }

    }

    private GUIStyle guiStyleHeadings = new GUIStyle();
    private GUIStyle guiStyleScores = new GUIStyle();
    void OnGUI()
    {
        //displaying the headings
        guiStyleHeadings.fontSize = 20;
        GUI.Label(new Rect(70, 20, 100, 20), "Name: ", guiStyleHeadings);
        GUI.Label(new Rect(250, 20, 100, 20), "Score: ", guiStyleHeadings);
        GUI.Label(new Rect(350, 20, 100, 20), "Date: ", guiStyleHeadings);

        //displaying the results of the top ten highs score values
        for (int i = 0; i < 10; i++)
        {
            GUI.Label(new Rect(40, 45 + (i * 20), 100, 20), Convert.ToString(i +1) , guiStyleScores);
            GUI.Label(new Rect(70, 45 + (i*20), 100, 20), highScoreTableArry[i, 0], guiStyleScores);
            GUI.Label(new Rect(250, 45 + (i * 20), 100, 20), highScoreTableArry[i, 1], guiStyleScores);
            GUI.Label(new Rect(350, 45 + (i * 20), 100, 20), highScoreTableArry[i, 2], guiStyleScores);
        }

    }
}
