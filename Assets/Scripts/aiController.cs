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

    static int countMaterial(string team)
    {
        int material = 0;
        //Multiple approaches can be used here - will start with finding all with specific tag however this is very inefficient and I will switch at a later date to something more appropriate 
        //RECOMMENDED OPTIMISATION = PERFORM THIS ON START AND CACHE IT, EVERY TIME A PIECE IS TAKEN MANIPULATE CACHE ACCORDINGLY
        if(team == "white")
        {
            GameObject[] pieces = GameObject.FindGameObjectsWithTag("whitePawn");
            material += pieces.Length * pawnValue;

            pieces = GameObject.FindGameObjectsWithTag("whiteRook");
            material += pieces.Length * rookValue;

            pieces = GameObject .FindGameObjectsWithTag("whiteKnight");
            material += pieces.Length * knightValue;

            pieces = GameObject.FindGameObjectsWithTag("whiteBishop");
            material += pieces.Length * bishopValue;

            pieces = GameObject.FindGameObjectsWithTag("whiteQueen");
            material += pieces.Length * queenValue;

            pieces = GameObject.FindGameObjectsWithTag("whiteKing");
            material += pieces.Length * kingValue;         
        }
        else
        {
            GameObject[] pieces = GameObject.FindGameObjectsWithTag("blackPawn");
            material += pieces.Length * pawnValue;

            pieces = GameObject.FindGameObjectsWithTag("blackRook");
            material += pieces.Length * rookValue;

            pieces = GameObject .FindGameObjectsWithTag("blackKnight");
            material += pieces.Length * knightValue;

            pieces = GameObject.FindGameObjectsWithTag("blackBishop");
            material += pieces.Length * bishopValue;

            pieces = GameObject.FindGameObjectsWithTag("blackQueen");
            material += pieces.Length * queenValue;

            pieces = GameObject.FindGameObjectsWithTag("blackKing");
            material += pieces.Length * kingValue;       
        }

        return material;
    }

    public void Initialize()
    {
        boardController = GameObject.FindGameObjectWithTag("GameController");
    }



}
