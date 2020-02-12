using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public Text mLabel; //button text label
    public Button mButton; //the current button
    public Main mMain; //reference to Main class
    public bool AImode = false; //AI mode bool 
    public static List<string> moveHistory = new List<string>(); //history of player moves
    public static List<Button> buttonHistory = new List<Button>();

    //fills in the current button with the current player's icon and calls to Switch players
    public void Fill() //fills in the cell when the player clicks the button
    {
        if (!Main.undoing)//if not undoing a move, then proceed to fill cell
        {
            mButton.interactable = false; //so player can't click the same cell twice
            buttonHistory.Add(mButton); //record the button clicked in button history
            mLabel.text = Main.GetTurnCharacter(); //apply the appropriate player label as button text
            mMain.Switch();//switch player after move is complete
        }
    }
}

