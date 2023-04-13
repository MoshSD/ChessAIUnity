    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiController : MonoBehaviour
{
    public GameObject boardController = GameObject.FindGameObjectWithTag("GameController");
    const int pawnValue = 1;
    const int rookValue = 5;
    const int bishopValue = 3;
    const int knightValue = 3;
    const int queenValue = 9;
    const int kingValue = 39;

    public bool controllingBlack = true;

    void Start()
    {
        Initialize();
    }

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
        //Multiple approaches can be used here - will start with finding all with specific tag however this is very inefficient and I will switch at a later date to something more appropriate 
        //RECOMMENDED OPTIMISATION = PERFORM THIS ON START AND CACHE IT, EVERY TIME A PIECE IS TAKEN MANIPULATE CACHE ACCORDINGLY
        var clonedGridPositions = (GameObject[])boardController.GetComponent<main>().returnGridPositions().Clone();
        if(team == "white")
        {
            foreach(var element in clonedGridPositions)
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
        else
        {
            foreach(var element in clonedGridPositions)
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

        return material;
    }

    public void Initialize()
    {
        boardController = GameObject.FindGameObjectWithTag("GameController");
    }



}
