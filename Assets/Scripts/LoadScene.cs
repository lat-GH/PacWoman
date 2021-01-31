using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {
    

    public void LoadSceneByIndex(int sceneIndex)
    {
        //the chosen scene's index is assigned in the unity editor window
        /*Scene Index values are:
             0 Main Control 
             1 Game_frame_withLevels
             2 HighScore Display
             3 HighScore Input   
         */
        SceneManager.LoadScene(sceneIndex);
    }
}
