using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiController : MonoBehaviour
{
    //public GameObject[,] gridPositionsCloned = new GameObject[8,8];
    public GameObject boardController;
    const int pawnValue = 1;
    const int rookValue = 5;
    const int bishopValue = 3;
    const int knightValue = 3;
    const int queenValue = 9;
    const int kingValue = 39;
    public bool controllingBlack = true;
    public bool aiTurnActive = false;

    public List<short> bestMove = new List<short>();

    

    int movesMade = 0;
    int movesUnmade = 0;

    public int evaluate()
    {
        int whiteEval = countMaterial("white");
        int blackEval = countMaterial("black");
        //Debug.Log("white evaluation: " + whiteEval + " black evaluation: " + blackEval);
        int evaluation = whiteEval - blackEval;
        int perspective = (boardController.GetComponent<main>().getWhiteToMove()) ? 1 : -1;
        return evaluation * perspective;
    }
    public int Search (int depth, int alpha, int beta)
    {
        if(depth == 0)
        {
            Debug.Log("depth is now 0, RETURNING FROM SEARCH FUNC");
            //Make move here 
            // Debug.Log("Moves made: " + movesMade);
            // Debug.Log("Moves unmade: " + movesUnmade);
            //Debug.Log("Evaluation of best position = " + evaluate());
            return evaluate();
        }
        
        //Generate moves for CURRENT team
        boardController.GetComponent<main>().generateMoves();
        List<List<short>> movesInternal = boardController.GetComponent<main>().moves;

        //If there are no moves (stalemate) return 0 == draw
        if (boardController.GetComponent<main>().moves.Count == 0)
        {
            return 0;
        }
        bestMove = null;
        foreach(List<short> move in movesInternal)
        {
            //List<List<short>> tempMovesList = boardController.GetComponent<main>().moves;
            //Debug.Log("MAKING A MOVE");
            if(move[3] == 1){Debug.Log("THIS MOVE IS AN ATTACK: " + move[0] + move[1]);}
            boardController.GetComponent<main>().makeMove(move);
            boardController.GetComponent<main>().nextTurn();
            movesMade++;
            int evaluation = -Search(depth - 1, -beta, -alpha);
            boardController.GetComponent<main>().unMakeMove(move);
            //boardController.GetComponent<main>().generateMoves();
            //boardController.GetComponent<main>().nextTurn();
            movesUnmade++;
            if(evaluation >= beta)
            {
                Debug.Log("pruning");
                //Prune good move
                return beta;
            }
            if(evaluation > alpha)
            {
                Debug.Log("new move is better than previous");
                alpha = evaluation;
                bestMove = move;
            }
        }
        return alpha;
    }

    public int countMaterial(string team)
    {
        int material = 0;

        //boardController.GetComponent<main>().gridPositions.CopyTo(gridPositionsCopied, 0);
        if(team == "white")
        {
            foreach(var element in this.GetComponent<main>().gridPositions)
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
            foreach(var element in this.GetComponent<main>().gridPositions)
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
    } 


    public void initiateAIMove()
    {
        aiTurnActive = true;
        Debug.Log("Current white evaluation: " + Search(1,-999999999,999999999));
        Debug.Log("Finalizing move");
        boardController.GetComponent<main>().makeMove(bestMove);
        aiTurnActive = false;
        boardController.GetComponent<main>().nextTurn();
    }

}
