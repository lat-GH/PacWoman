using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.

public class InputHighScore : MonoBehaviour {

    // textbox unity object
    public InputField TheInputField; 
    public string userInput;

    public void Start()
    {
        TheInputField.text = "Enter Team Name..."; // determines what is displayed in the input field before the user inputs anything
        TheInputField.characterLimit = 20; //restricts how many characters that the user can enter
        //listener that waits the 'on end edit'event (when return is hit OR they leave the context of the InputField) then is calls the function AsigningUserInput
        TheInputField.onEndEdit.AddListener(delegate { AsigningUserInput(); }); 

    }

    void AsigningUserInput()
    {
        Globals.TeamName = TheInputField.text;//storing the reslut that the usre enters into the globalS
        if (Globals.DEBUG) { print(Globals.TeamName); }
    }

    

}
