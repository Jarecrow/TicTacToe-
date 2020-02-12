using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public Board mBoard; //the game board
    public static bool mXTurn = true; //bool to control which player is active
    private int mTurncount; //counts the number of turns 
    [SerializeField]
    public static int undoCount = 0; //to record the number of undos
    public GameObject mWinner; //the canvas to display when there is a winner
    public List<string> mainHistory = Cell.moveHistory; //reference to the move history list
    public List<Button> moveHistory = Cell.buttonHistory; //reference to the button history list
    [SerializeField]
    public static bool undoing = false; //only allows one undo button click at a time
    private bool redoing = false; //only allows one redo button click at a time
    [SerializeField]
    private string redoString; //stored string from undo that allows for cell redo
    private Button redoButton; //stored button to allow for cell redo
    [SerializeField]
    private Button RedoButton; //Redo Button prefab
    [SerializeField]
    private Button UndoButton; //Undo Button prefab
    [SerializeField]
    public Button aTwoPlayer; //Button A_2 Player (3 x 3 Grid)
    [SerializeField]
    public Button bTwoPlayer; //Button B_2 Player (4 x 4 Grid)
    [SerializeField]
    public Button cVsAI; //Button C_Player vs. AI (3 x 3)
    [SerializeField]
    public Button dVsAI; //Button D_Player vs. AI (4 x 4)
    [SerializeField]
    private Button RunAIButton; //The UI Button for AI moves
    [SerializeField]
    private Button MenuButton; //The return to Main Menu UI Button
    [SerializeField]
    private GameObject ShieldAI; //UI panel prevents player clicking a cell during AI move    
    [SerializeField]
    private GameObject xIndicator; //Indicator for player X's turn
    [SerializeField]
    private GameObject oIndicator; //Indicator for player O's turn
    public static bool indicator; //contorl the indicator icon
    [SerializeField]
    private Text xScore; //Player X's score text object
    [SerializeField]
    private Text oScore; //Player O's score text object
    private int xScoreInt; //Player X's score
    private int oScoreInt; //Player O's score 
    [SerializeField]
    private int redoCount = 0;

    public void Awake()
    {
        RedoButton.interactable = false; //Can't use Redo UI Button until player has used Undo UI Button
        mBoard.Build(this); //populate the board with interactive button cells
        UndoButton.interactable = false;
    }

    public void BuildGameA() //Build 2 player 3 x 3 game
    {
        Board.nineCellGrid = true; //Set 9 cell game
        Board.aiMode = false; //Disable AI player move button
        SceneManager.LoadScene("3x3"); //Load 3 x 3 Scene
        StartCoroutine(PopulateBoard()); //Populate grid with cells
    }

    public void BuildGameB() //Build 2 player 4 x 4 grid game
    {
        Board.nineCellGrid = false; //Set 16 cell game
        Board.aiMode = false; //Disable AI player move button
        SceneManager.LoadScene("4x4"); //Load 4 x 4 Scene
        StartCoroutine(PopulateBoard()); //Populate grid with cells
    }

    public void BuildGameC() //Build player vs. AI 3 x 3 game
    {
        Board.nineCellGrid = true; //Set 9 cell game
        Board.aiMode = true; //Enable AI player move button
        SceneManager.LoadScene("3x3"); //Load 3 x 3 Scene
        StartCoroutine(PopulateBoard()); //Populate grid with cells
    }

    public void BuildGameD() //Build player vs. AI 4 x 4 game
    {
        Board.nineCellGrid = false;//Build 16 cell board
        Board.aiMode = true; //Enable AI player move button
        SceneManager.LoadScene("4x4"); //Load 4 x 4 Scene
        StartCoroutine(PopulateBoard()); //Populate grid with cells
    }

    public void MainMenu() //Return to Main Menu
    {
      
        SceneManager.LoadScene("0_Main"); //Load menu
    }

    public IEnumerator PopulateBoard()
    {
        yield return new WaitForSeconds(1f * Time.deltaTime);
        mBoard.Build(this); //populate the board with interactive button cells
    }

    public void Switch() //count turns, change player, check for winner
    {

        mTurncount++; //add one to turn count 

        if (!Board.aiMoving) //AI is too cool to use Undo, so don't bother activating the button
        {
            UndoButton.interactable = true;
        }

        mainHistory.Add(mTurncount + " Player " + GetTurnCharacter()); //label the string list history

        bool hasWinner = mBoard.CheckForWinner(); //if true, check for winner

        if (Board.nineCellGrid == true) //check for 3 x 3 board with 9 cells
        {
            if (hasWinner || mTurncount == 9)
            {
                //End Game
                StartCoroutine(EndGame(hasWinner));
                return;
            }
        }
        else if (Board.nineCellGrid == false) //check for 4 x 4 board with 16 cells
        {
            if (hasWinner || mTurncount == 16)
            {
                //End Game
                StartCoroutine(EndGame(hasWinner));
                return;
            }
        }
        undoCount = 0; //int holds # of player undos
        undoing = false; //reset undoing bool
        RedoButton.interactable = false; //deactivate until next player undo
        if (!Board.aiMoving)
        {
            RunAIButton.interactable = true;
        }

        mXTurn = !mXTurn; //switch player

        if (mXTurn)
        {
            xIndicator.SetActive(true);
            oIndicator.SetActive(false);
        }
        else if (!mXTurn)
        {
            oIndicator.SetActive(true);
            xIndicator.SetActive(false);
        }

        if (Board.aiMode && !mXTurn) //shield player from clicking cells during ai play
        {
            ShieldAI.SetActive(true);
        }
        else if (mXTurn)
        {
            ShieldAI.SetActive(false);
        }
        Board.aiMoving = false; //End AI turn      
    }

    public static string GetTurnCharacter() //Get the text "icon" for the player
    {
        if (mXTurn)
        {
            return "X"; //Player X
        }
        else
        {
            return "O"; //Player Y
        }
    }

    private IEnumerator EndGame(bool hasWinner) //to display the correct winner and call to reset board
    {
        UndoButton.interactable = false; //no undos stored for first play
        RedoButton.interactable = false; //no redos stored at first play
        xIndicator.SetActive(false); //clear indicator
        oIndicator.SetActive(false); //clear indicator       
        Text winnerLabel = mWinner.GetComponentInChildren<Text>();
        if (hasWinner)
        {
            winnerLabel.text = GetTurnCharacter() + " " + "won!"; //the winning players turn icon plus won text
        }
        else
        {
            winnerLabel.text = "Draw!"; //maximum turn count reached with no winner
        }

        ShieldAI.SetActive(false); //remove shield (still active if AI won)

        mWinner.SetActive(true); //set winner message canvas to active

        if (mXTurn && hasWinner)
        {
            xScoreInt++;
            xScore.text = ("SCORE: " + xScoreInt);
        }
        else if(!mXTurn && hasWinner)
        {
            oScoreInt++;
            oScore.text = ("SCORE: " + oScoreInt);
        }

        WaitForSeconds wait = new WaitForSeconds(5.0f); //length of winning display message before reset

        yield return wait;

        mBoard.Reset(); //reset the board 
        mTurncount = 0; //reset the turn counter
        undoCount = 0; //reset the Undo counter
        UndoButton.interactable = false; //inactive until undo is stored
        RedoButton.interactable = false; //inactive until redo is stored
        Board.aiMoving = false; //End the AI turn 
        mWinner.SetActive(false); //set the winner message display canvas to inactive
        mainHistory.Clear(); //remove history string/playermove items from list
        moveHistory.Clear(); //remove history of buttons played from list  
        xIndicator.SetActive(true); //Player X begins
        oIndicator.SetActive(false); //Player O is second player
    }

    public void Undo() //tied to undo button on click () event
    {
        if (!undoing && mainHistory.Count > 0 && moveHistory.Count > 0 && undoCount < 1) //check for undo in progress and existing moves
        {
            undoing = true; //set to true to prevent multiple undo clicks

            ShieldAI.SetActive(false); //drop ai shield panel

            if (!mXTurn) //switch player turn indicator
            {
                xIndicator.SetActive(true);
                oIndicator.SetActive(false);
            }
            else if (mXTurn)
            {
                oIndicator.SetActive(true);
                xIndicator.SetActive(false);
            }
            undoCount++; //add one to the undo queue (currently limited to one per player)
            RunAIButton.interactable = false; //Deactivate button   
            if (mTurncount > 0) //if there's a move
            {
                mTurncount--; //reduce the move count by one
            }
            var item = mainHistory[mainHistory.Count - 1]; //go back one item in the string list
            mainHistory.Remove(item); //remove the move from the string list
            var buttonItem = moveHistory[moveHistory.Count - 1]; //go back one in the button list
            redoButton = buttonItem; //store the button for possible redo
            if (redoButton != null)
            {
                redoString = buttonItem.GetComponentInChildren<Text>().text;
            }
            buttonItem.GetComponentInChildren<Text>().text = ""; //set the cell text to blank
            buttonItem.interactable = true; //make the button interactable/put cell back into play
            moveHistory.Remove(buttonItem); //remove the played/clicked button from the list
            UndoButton.interactable = false; //only allow each player one undo per turn
            undoing = false; //reset undoing bool
            redoing = false; //reset redoing bool
            redoCount++;
            RedoButton.interactable = true; //activate the UI Redo Button now that it holds and undone cell play
            mXTurn = !mXTurn; //Switch player
        }
    }

    public void Redo() //tied to the redo button on click () event
    {
        if (!redoing) //bool to check that redo hasn't already been clicked
        {
            if (!mXTurn) //switch player turn indicator
            {
                xIndicator.SetActive(true);
                oIndicator.SetActive(false);
            }
            else if (mXTurn)
            {
                oIndicator.SetActive(true);
                xIndicator.SetActive(false);
            }
            redoing = true; //bool to allow redo click
            redoButton.interactable = false; //marked the button as played/clicked
            redoButton.GetComponentInChildren<Text>().text = redoString; //add the player icon to the button cell
            mainHistory.Add((mTurncount + 1) + " Player " + redoString + " Redo"); //add player text icon to the player history list
            moveHistory.Add(redoButton); //add the played button to the move history list
            mTurncount++; //add one move to the turn count
            undoCount = 0; //subtract one from the undo count
            UndoButton.interactable = true;
            undoing = false; //make the undo button clickable again
            RedoButton.interactable = false; //Inactive until next Undo UI Button click
            RunAIButton.interactable = true; //Reactivate button
            if (Board.aiMode)
            {
                ShieldAI.SetActive(true);
            }
            mXTurn = !mXTurn; //switch player
        }
    }    
}




