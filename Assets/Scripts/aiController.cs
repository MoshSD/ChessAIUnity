    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiController : MonoBehaviour
{
    public GameObject[,] gridPositionsCloned = new GameObject[8,8];
    public GameObject boardController;
    const int pawnValue = 1;
    const int rookValue = 5;
    const int bishopValue = 3;
    const int knightValue = 3;
    const int queenValue = 9;
    const int kingValue = 39;

    public bool controllingBlack = true;

    public int evaluate()
    {
        int whiteEval = countMaterial("white");
        int blackEval = countMaterial("Black");
        int evaluation = whiteEval - blackEval;
        int perspective = (boardController.GetComponent<main>().getWhiteToMove()) ? 1 : -1;
        return evaluation * perspective;
    }

    public int countMaterial(string team)
    {
        int material = 0;

        //boardController.GetComponent<main>().gridPositions.CopyTo(gridPositionsCopied, 0);
        if(team == "white")
        {
            foreach(var element in gridPositionsCloned)
            {
                if(element != null)
                {
                    string tempName = element.name;
                    switch(tempName)
                    {
                        case "whiteKing" : material += kingValue; break;
                        case "whiteQueen" : material += queenValue; break;
                        case "whiteBishop" : material += bishopValue; break;
                        case "whiteKnight" : material += knightValue; break;
                        case "whiteRook" : material += rookValue; break;
                        case "whitePawn" : material += pawnValue; break;
                    }
                }
            }
        }
        else if(team == "black")
        {
            foreach(var element in gridPositionsCloned)
            {
                if(element != null)
                {
                    string tempName = element.name;
                            switch(tempName)
                    {
                        case "blackKing" : material += kingValue; break;
                        case "blackQueen" : material += queenValue; break;
                        case "blackBishop" : material += bishopValue; break;
                        case "blackKnight" : material += knightValue; break;
                        case "blackRook" : material += rookValue; break;
                        case "blackPawn" : material += pawnValue; break;
                    }
                }
            }
        }
        return material;
    }

    public void Initialize()
    {
        boardController = GameObject.FindGameObjectWithTag("GameController");

        // Initialize the copied array as C# has no support for copying 2dimensional arrays like this
        for (int i = 0; i < gridPositionsCloned.GetLength(0); i++)
        {
            for (int j = 0; j < gridPositionsCloned.GetLength(1); j++) {
                gridPositionsCloned[i,j] = this.GetComponent<main>().gridPositions[i,j];
                //Debug.Log(gridPositionsCloned[i, j]);
            }
        }
    } 

    // void Update()
    // {
    //     for (int i = 0; i < boardController.GetComponent<main>().gridPositions.GetLength(0); i++)
    //     {
    //         for (int j = 0; j < boardController.GetComponent<main>().gridPositions.GetLength(1); j++) {
    //             Debug.Log(boardController.GetComponent<main>().gridPositions[i, j]);
    //         }
    //     }
    // }

    //Giving ai ability to make and unmake moves
    void makeMove(List<short> moveId)
    {
        short fromSquare = moveId[0];
        short toSquare = moveId[1];
        short promotion = moveId[2];
        short capture = moveId[3];
        short special1 = moveId[4];
        short special2 = moveId[5];

    }

    void unMakeMove(int moveId)
    {

    }

}
