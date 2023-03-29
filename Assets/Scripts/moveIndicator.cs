using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveIndicator : MonoBehaviour
{
    //Reference to the boardController
    public GameObject controller;



    //Initializing the reference to the currently selected piece
    GameObject pieceReference = null;


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


    //When the player clicks, check if the players current move is and attack - if it is, destroy the piece it is attacking.  Then move the piece that the player has selected
    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        //if the player is attacking, destroy the target piece
        if(attacking)
        {
            GameObject piece = controller.GetComponent<main>().GetPosition(boardMatrixX, boardMatrixY);
            Destroy(piece);
        }

        //setting the position where the attacked piece used to be to empty space
        controller.GetComponent<main>().setPositionEmpty(pieceReference.GetComponent<chessPiece>().getBoardX(),
            pieceReference.GetComponent<chessPiece>().getBoardY());

        //Setting the moved pieces new location after the move has occurred
        pieceReference.GetComponent<chessPiece>().setBoardX(boardMatrixX);
        pieceReference.GetComponent<chessPiece>().setBoardY(boardMatrixY);
        pieceReference.GetComponent<chessPiece>().setCoordinates();

        controller.GetComponent<main>().setPosition(pieceReference);

        //Removing the movement indicators 
        pieceReference.GetComponent<chessPiece>().destroyMoveIndicators();
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
