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
        Debug.Log("Generating moves");
        boardController.GetComponent<main>().generateMoves();
        List<List<short>> movesInternal = boardController.GetComponent<main>().moves;

        //If there are no moves (stalemate) return 0 == draw
        if (boardController.GetComponent<main>().moves.Count == 0)
        {
            return 0;
        }
        bestMove = null;
        foreach(List<short> move in movesInternal.ToArray())
        {
            //int[,] previousGridIntPositions = new int[8,8];
            //previousGridIntPositions = boardController.GetComponent<main>().assignIntGridPositions(boardController.GetComponent<main>().gridPositions);
            //List<List<short>> tempMovesList = boardController.GetComponent<main>().moves;
            //Debug.Log("MAKING A MOVE");
            if(move[3] == 1){Debug.Log("THIS MOVE IS AN ATTACK: " + move[0] + move[1]);}
            boardController.GetComponent<main>().makeMove(move);
            boardController.GetComponent<main>().nextTurn();
            movesMade++;
            int evaluation = -Search(depth - 1, -beta, -alpha);
            boardController.GetComponent<main>().unMakeMove(boardController.GetComponent<main>().movesMadeList[^1]);
            //RestoreStateFromSnapshot(previousGridIntPositions);
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


    public void RestoreStateFromSnapshot(int[,] snapShot)
    {
        // Clear the current chessboard
        boardController.GetComponent<main>().gridPositions = new GameObject[8,8];
        GameObject[] objectsToBeCleaned = GameObject.FindGameObjectsWithTag("chessPiece");
        for(int i = 0; i < objectsToBeCleaned.Length; i++)
        {
           Destroy(objectsToBeCleaned[i]);
        }

        // Restore the positions and types of the chess pieces from the snapshot
        for (int i = 0; i < snapShot.Length; i++)
        {
            if(snapShot[i % 8, i / 8] != 0)
            {
                string tempName = "null";
                switch(snapShot[i % 8, i / 8])
                {
                    //pawns first as most common
                    case 5 : tempName = "whitePawn"; break;
                    case 6 : tempName = "blackPawn"; break;

                    case 1 : tempName = "whiteKing"; break;
                    case 2 : tempName = "blackKing"; break;
                    case 3 : tempName = "whiteQueen"; break;
                    case 4 : tempName = "blackQueen"; break;
                    case 7 : tempName = "whiteRook"; break;
                    case 8 : tempName = "blackRook"; break;
                    case 9 : tempName = "whiteBishop"; break;
                    case 10 : tempName = "blackBishop"; break;
                    case 11 : tempName = "whiteKnight"; break;
                    case 12 : tempName = "blackKnight"; break;
                }
                boardController.GetComponent<main>().gridPositions[i % 8, i / 8] = boardController.GetComponent<main>().spawnPiece(tempName, i % 8, i / 8);                
            }
        }
    }





    public void Initialize()
    {
        boardController = GameObject.FindGameObjectWithTag("GameController");
    } 


    public void initiateAIMove()
    {
        aiTurnActive = true;
        //int[,] intGridPositions = boardController.GetComponent<main>().assignIntGridPositions(boardController.GetComponent<main>().gridPositions);
        // for(int i = 0; i < intGridPositions.Length; i++)
        // {
        //     Debug.Log("peice number is: " + intGridPositions[i % 8, i / 8]);
        // }
        Debug.Log("Current white evaluation: " + Search(1,-999999999,999999999));
        Debug.Log("Finalizing move");
        boardController.GetComponent<main>().makeMove(bestMove);
        aiTurnActive = false;
        boardController.GetComponent<main>().nextTurn();
    }

}
