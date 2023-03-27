using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Script defining individual pieces
public class chessPiece : MonoBehaviour
{
    //Public object references
    public GameObject boardController;
    public GameObject movePlate;


    //Private vars for each piece
    private int boardX = -1;
    private int boardY = -1;
    private string team;

    //Board offset variables
    private const float boardOffsetA = 0.66f;
    private const float boardOffsetB = -2.3f;


    public Sprite whiteKing, whiteQueen, whiteBishop, whiteKnight, whiteRook, whitePawn;
    public Sprite blackKing, blackQueen, blackBishop, blackKnight, blackRook, blackPawn;

    public void setCoordinates() 
    {
        float x = boardX;
        float y = boardY;

        x *= boardOffsetA;
        y *= boardOffsetA;

        x += boardOffsetB;
        y += boardOffsetB;

        this.transform.position = new Vector3(x,y, -1);
    }


    //FNC for getting board positions

    public int getBoardX()
    {
        return boardX;
    }
    public int getBoardY()
    {
        return boardY;
    }
    public Vector3 getPieceCoordinates()
    {
        return new Vector3(boardX, boardY, -1);
    }


    //FNC for setting board positions 

    public void setBoardX(int x)
    {
        boardX = x;
    }
    public void setBoardY(int y)
    {
        boardY = y;
    }
    public void setBoardCoordinates(int x, int y)
    {
        boardX = x;
        boardY = y;
    }


    public void Initialize()
    {
        boardController = GameObject.FindGameObjectWithTag("GameController");
        
        //Switch statement to set the sprite of the piece  NEED TO FIND WAY TO CLEAN THIS CODE AND MAKE FUNC WITH PARAM ("PIECENAME") or something similar to optimise
        switch(this.name)
        {
            case "blackKing" : this.GetComponent<SpriteRenderer>().sprite = blackKing; break;
            case "blackQueen" : this.GetComponent<SpriteRenderer>().sprite = blackQueen; break;
            case "blackBishop" : this.GetComponent<SpriteRenderer>().sprite = blackBishop; break;
            case "blackKnight" : this.GetComponent<SpriteRenderer>().sprite = blackKnight; break;
            case "blackRook" : this.GetComponent<SpriteRenderer>().sprite = blackRook; break;
            case "blackPawn" : this.GetComponent<SpriteRenderer>().sprite = blackPawn; break;

            case "whiteKing" : this.GetComponent<SpriteRenderer>().sprite = whiteKing; break;
            case "whiteQueen" : this.GetComponent<SpriteRenderer>().sprite = whiteQueen; break;
            case "whiteBishop" : this.GetComponent<SpriteRenderer>().sprite = whiteBishop; break;
            case "whiteKnight" : this.GetComponent<SpriteRenderer>().sprite = whiteKnight; break;
            case "whiteRook" : this.GetComponent<SpriteRenderer>().sprite = whiteRook; break;
            case "whitePawn" : this.GetComponent<SpriteRenderer>().sprite = whitePawn; break;
        }

        //using the coordinate offset function
        setCoordinates();


    }


}
