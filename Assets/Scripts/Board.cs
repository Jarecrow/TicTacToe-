using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{  
    // Start is called before the first frame update

    public GameObject mCellPrefab; //prefab for the button cells
    public Cell[] mCells = new Cell[9]; //array for the 3x3 board
    private Cell[] mCellsTwo = new Cell[16]; //array for the 4x4 board
    public static bool nineCellGrid = false; //array for the board choice
    public static bool aiMode = false; //turns ai mode on or off
    public static bool aiMoving = false;
    [SerializeField]
    private bool aiButton; //prevents multiple clicks
    public Main bMain; //reference to Main class
    public Button aiMoveButton;
    [SerializeField]
    private GameObject aiButtonObject; //the AI button prefab
    [SerializeField]
    private Button undoButton; //Undo Button prefab

    public void Build(Main main) //Instantiate 9 or 19 cells based on niceCellGrid bool
    {
        if (aiMode)
        {
            aiButtonObject.SetActive(true); //AI button only active in AI mode
            aiMoveButton.interactable = false; //Can't interact until after player moves
        }
        if (nineCellGrid) //If true, instantiate 9 cells
        {
            for (int i = 0; i <= 8; i++)
            {
                GameObject newCell = Instantiate(mCellPrefab, transform);
                mCells[i] = newCell.GetComponent<Cell>();
                mCells[i].mMain = main;
            }
        }
        else if (!nineCellGrid) //if false, instantiate 16 cells
        {
            for (int i = 0; i <= 15; i++)
            {
                GameObject newCell = Instantiate(mCellPrefab, transform);
                mCellsTwo[i] = newCell.GetComponent<Cell>();
                mCellsTwo[i].mMain = main;
            }
        }
        Reset(); //Set/Reset method to run at the beginning of every round
    }

    public void Reset() //reset the board
    {
        if (nineCellGrid == true) //9 cell game
        {
            foreach (Cell cell in mCells)
            {
                cell.mLabel.text = ""; //blank label

                cell.mButton.interactable = true; //button is set to playable
            }
        }

        if (nineCellGrid == false) //16 cell game
        {
            foreach (Cell cell in mCellsTwo)
            {
                cell.mLabel.text = ""; //blank label

                cell.mButton.interactable = true; //button is set to playable
            }
        }

        Main.mXTurn = true; //Always start with player X
    }

    public void RunAI() //call method for ai move via AI Move Button
    {
        if (!aiButton) //check that the ai player isn't already making a move
        {
            aiMoving = true; //will clear on next player move
            aiButton = true; //to prevent multiple button clicks
            aiMoveButton.interactable = false; //disable ai button
            undoButton.interactable = false; //disable undo button
            Main.undoCount = 0;

            if (nineCellGrid) //if true, get random cell from 9 cell grid
            {
                bool validCell = false;

                while (!validCell) //keeps looping while found cells are already played
                {
                    int i = (Random.Range(0, mCells.Length)); //Get random cell index
                    mCells[i].GetComponent<Cell>(); //Get the cell instance associated with the indexed button
                    if (mCells[i].mButton.interactable) //If the button is interactable/unplayed
                    {
                        mCells[i].mLabel.text = Main.GetTurnCharacter(); //Get the appropriate player icon
                        mCells[i].mButton.interactable = false; //Set the button as played
                        bMain.Switch(); //Switch player
                        aiButton = false; //Set ai click bool to false
                        validCell = true; //Set validCell to true and end while loop
                    }
                }
            }

            if (!nineCellGrid) //if false, get random cell from 16 cell grid
            {
                bool validCell = false; //Set bool as false to indicate no playable button found

                while (!validCell) //continues looping while found sells are already played
                {
                    int i = (Random.Range(0, mCellsTwo.Length)); //Ge random cell index
                    mCellsTwo[i].GetComponent<Cell>(); //Get the cell instance associated with the indexed button
                    if (mCellsTwo[i].mButton.interactable) //If the button is interactable/unplayed 
                    {
                        mCellsTwo[i].mLabel.text = Main.GetTurnCharacter(); //Get random cell index
                        mCellsTwo[i].mButton.interactable = false; //Set the button as played
                        bMain.Switch(); //Switch player
                        aiButton = false; //Set ai click bool to false
                        validCell = true; //Set valid cell to true and end while loop
                    }
                }
            }
        }
    }

    //public void RunEvilAI() - Add more challening AI if desirable

    public bool CheckForWinner() //check for horizontal, vertical, and diagonal winners
    {
        int i = 0;

        if (nineCellGrid == true) //check for the nine cell game
        {

            //Horizontal
            for (i = 0; i <= 6; i += 3)
            {
                if (!CheckValues(i, i + 1))
                    continue;

                if (!CheckValues(i, i + 2))
                    continue;

                return true;
            }

            //Vertical
            for (i = 0; i <= 2; i++)
            {
                if (!CheckValues(i, i + 3))
                    continue;

                if (!CheckValues(i, i + 6))
                    continue;

                return true;
            }

            //LeftDiagonal

            if (CheckValues(0, 4) && CheckValues(0, 8))
                return true;

            //RightDiagonal

            if (CheckValues(2, 4) && CheckValues(2, 6))
                return true;
        }
        else if (nineCellGrid == false) //check for 16 cell game
        {

            //Horizontal
            for (i = 0; i <= 12; i += 4)
            {
                if (!CheckValuesTwo(i, i + 1))
                    continue;

                if (!CheckValuesTwo(i, i + 2))
                    continue;

                if (!CheckValuesTwo(i, i + 3))
                    continue;

                return true;
            }

            //Vertical
            for (i = 0; i <= 3; i++)
            {
                if (!CheckValuesTwo(i, i + 4))
                    continue;

                if (!CheckValuesTwo(i, i + 8))
                    continue;

                if (!CheckValuesTwo(i, i + 12))
                    continue;

                return true;
            }

            //LeftDiagonal

            if (CheckValuesTwo(0, 5) && CheckValuesTwo(0, 10) && CheckValuesTwo(0, 15))
                return true;

            //RightDiagonal

            if (CheckValuesTwo(3, 6) && CheckValuesTwo(3, 9) && CheckValuesTwo(3, 12))
                return true;
        }

        return false;
    }

    private bool CheckValues(int firstIndex, int secondIndex) //check to see if values match for 9 cell game
    {
        string firstValue = mCells[firstIndex].mLabel.text;
        string secondValue = mCells[secondIndex].mLabel.text;

        if (firstValue == "" || secondValue == "")
            return false;
        if (firstValue == secondValue)
            return true;
        else
            return false;
    }
    private bool CheckValuesTwo(int firstIndex, int secondIndex) //check to see if values match for 16 cell game
    {
        string firstValue = mCellsTwo[firstIndex].mLabel.text;
        string secondValue = mCellsTwo[secondIndex].mLabel.text;

        if (firstValue == "" || secondValue == "")
            return false;
        if (firstValue == secondValue)
            return true;
        else
            return false;
    }
}
