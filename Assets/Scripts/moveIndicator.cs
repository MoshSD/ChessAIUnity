using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveIndicator : MonoBehaviour
{
    //Reference to the boardController
    public GameObject controller;



    //Initializing the reference to the currently selected piece
    GameObject pieceReference = null;
    //Whether or not the current move indicator denotes a double pawn move 
    private bool pawnDoubleMove = false;    
    //Whether the move square represents a move that is an en-passant
    public bool enPassant = false;

    //board position
    int boardMatrixX;
    int boardMatrixY;
    
    //true indicates the piece is taking another
    public bool attacking = false;


    //Initialization func
    public void Initialize()
    {
        //If the players move is classified as an attack, highlight the move plate with red instead of the default
        if (attacking)
        {
            Debug.Log("win");
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    //Externally setting the double move var
    public void setDoubleMove(bool move)
    {
        pawnDoubleMove = move;
    }


    //When the player clicks, check if the players current move is and attack - if it is, destroy the piece it is attacking.  Then move the piece that the player has selected
    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        //if the player is attacking, destroy the target piece
        if(attacking)
        {
            if(enPassant == false)
            {
                Debug.Log("not enpassant");
                GameObject piece = controller.GetComponent<main>().GetPosition(boardMatrixX, boardMatrixY);

                //If the kings pieces are taken - End the game
                if(piece.name == "whiteKing") controller.GetComponent<main>().winner("black");
                if(piece.name == "blackKing") controller.GetComponent<main>().winner("white");
                Destroy(piece);
            }

            //En-passant logic for both teams 
            else if (enPassant == true && pieceReference.name == "blackPawn")
            {
                GameObject piece = controller.GetComponent<main>().GetPosition(boardMatrixX, boardMatrixY + 1);
                Destroy(piece);

            }
            else if (enPassant == true && pieceReference.name == "whitePawn")
            {
                GameObject piece = controller.GetComponent<main>().GetPosition(boardMatrixX, boardMatrixY - 1);
                Destroy(piece);
            }
            

        }

        //setting the position where the piece used to be to null 
        controller.GetComponent<main>().setPositionEmpty(pieceReference.GetComponent<chessPiece>().getBoardX(),
            pieceReference.GetComponent<chessPiece>().getBoardY());

        //Setting the moved pieces new location after the move has occurred
        pieceReference.GetComponent<chessPiece>().setBoardX(boardMatrixX);
        pieceReference.GetComponent<chessPiece>().setBoardY(boardMatrixY);
        pieceReference.GetComponent<chessPiece>().setCoordinates();
        //Confirming that the player made a double move (first pawn move is double)
        pieceReference.GetComponent<chessPiece>().setHasMovedDouble(pawnDoubleMove);


        
        controller.GetComponent<main>().setPosition(pieceReference);
        //Setting the player to the one that was not playing previously
        controller.GetComponent<main>().nextTurn();

        //Removing the movement indicators 
        pieceReference.GetComponent<chessPiece>().destroyMoveIndicators();

        //Setting has moved to true
        pieceReference.GetComponent<chessPiece>().setHasMoved(true);
        
    }


    //Setting the boardMatrix variables
    public void setCoordinates(int x, int y)
    {
        boardMatrixX = x;
        boardMatrixY = y;
    }

    //Setting the reference to the piece that has been selected
    public void setPieceReference(GameObject obj)
    {
        pieceReference = obj;
    }

    //Returning a reference to the currently selected piece
    public GameObject GetReference()
    {
        return pieceReference;
    }

}
