using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Script defining individual pieces
public class chessPiece : MonoBehaviour
{
    //Public object references
    public GameObject boardController;
    public GameObject moveIndicator;


    //Private vars for each piece
    private int boardX = -1;
    private int boardY = -1;
    private string team;

    //Board offset variables
    private const float boardOffsetA = 0.66f;
    private const float boardOffsetB = -2.3f;

    //Var that will be used for initial pawn movement as well as castling
    private bool hasMoved = false;

    //Used for implementing en-passant - check for whether the pawn last moved two spaces
    private bool lastMoveWasDouble = false;

    //Remove indicators from the A.I controlled team
    public bool isAiControlled = false;
    public bool isPlayerControlled = false;
    public bool hasMovedDouble = false;

    List<short> currentMove = new List<short>();

    public int movesGeneratedForPiece = 0;

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

    //Function for external scripts to change whether the piece has moved or not
    public void setHasMoved(bool moved)
    {
        hasMoved = moved;
    }


    //Determining whether the last move was double -- accessible publicly
    public void setHasMovedDouble(bool move)
    {
        lastMoveWasDouble = move;
    }

    public string getTeam()
    {
        return(team);
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


    //FNC for setting board positions============================
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
    //===========================================================

    //Func for spawning the movement indicators for each piece                      +++++++++++++ CAN BE OPTIMISED TO INCLUDE ATTACKING AS WELL RATHER THAN USING SEPERATE FUNC +++++++++++++  
    public void moveIndicatorSpawn(int boardMatrixX, int boardMatrixY, bool isMoveDouble = false, string castleSide = "null", bool promotionMove = false)
    {
        //Adding to moves array
        if(isAiControlled)
        {
            short toVal = (short)((boardMatrixY * 8) + boardMatrixX);
            short fromVal = (short)((boardY * 8) + boardX); 
            short promoVal = 0;
            short attackVal = 0;
            short special1 = 0;
            short special2 = 0;
            if(isMoveDouble){special2 = 1; Debug.Log("double move has been initialized for moves list");}
            if(castleSide == "kingSide"){special1 = 1;}
            if(castleSide == "queenSide"){special1 = 1; special2 = 1;}
            //Only does queen promotion as of now but will be improved to include all different types
            if(promotionMove){promoVal = 1; special1 = 1; special2 = 1;}
            
            //Debug.Log(this.name + "moving from: " + fromVal + " to: " + toVal);
            currentMove.Add(fromVal);
            currentMove.Add(toVal);
            currentMove.Add(promoVal);
            currentMove.Add(attackVal);
            currentMove.Add(special1);
            currentMove.Add(special2);

            boardController.GetComponent<main>().moves.Add(currentMove);
        }


        if(isPlayerControlled && boardController.GetComponent<aiController>().aiTurnActive == false)
        {
            //Setting internal vars to the board positions of the indicator
            float x = boardMatrixX;
            float y = boardMatrixY;



            //Offsetting from unity worldspace to actual boardspace
            x *= boardOffsetA;
            y *= boardOffsetA;
            x += boardOffsetB;
            y += boardOffsetB;

            //Instantiating the movement indicator object (spawning it)
            GameObject mi = Instantiate(moveIndicator, new Vector3(x, y, -3.0f), Quaternion.identity);
            moveIndicator miScript = mi.GetComponent<moveIndicator>();
            miScript.setPieceReference(gameObject);
            miScript.setCastleSide(castleSide);
            if(castleSide == "kingSide")
            {
                miScript.setCoordinates(boardMatrixX - 1, boardMatrixY);        
            }
            else if(castleSide == "queenSide")
            {
                miScript.setCoordinates(boardMatrixX + 2, boardMatrixY);
            }
            else
            {
                miScript.setCoordinates(boardMatrixX, boardMatrixY);
            }
            miScript.setDoubleMove(isMoveDouble);
            miScript.setPromotionMove(promotionMove);
            miScript.Initialize();
        }
    }

    //Func for spawning the attack movement indicators for each piece                 
    public void moveIndicatorAttackSpawn(int boardMatrixX, int boardMatrixY, bool enPassant = false, bool promotionMove = false)
    {
        //Adding to moves array
        if(isAiControlled)
        {
            short toVal = (short)((boardMatrixX * 8) + boardMatrixY);
            short fromVal = (short)((boardX * 8) + boardY); 
            short promoVal = 0;
            short attackVal = 1;
            short special1 = 0;
            short special2 = 0;
            //Only does queen promotion as of now but will be improved to include all different types
            if(promotionMove){promoVal = 1; special1 = 1; special2 = 1;}
            if(enPassant){special2 = 1;}

            //Debug.Log(this.name + "moving from: " + fromVal + " to: " + toVal);
            currentMove.Add(fromVal);
            currentMove.Add(toVal);
            currentMove.Add(promoVal);
            currentMove.Add(attackVal);
            currentMove.Add(special1);
            currentMove.Add(special2);

            boardController.GetComponent<main>().moves.Add(currentMove);
        }


        if(isPlayerControlled && boardController.GetComponent<aiController>().aiTurnActive == false)
        {
            //Setting internal vars to the board positions of the indicator
            float x = boardMatrixX;
            float y = boardMatrixY;
            //Offsetting from unity worldspace to actual boardspace
            x *= boardOffsetA;
            y *= boardOffsetA;
            x += boardOffsetB;
            y += boardOffsetB;
            //Instantiating the movement indicator object (spawning it)
            GameObject mi = Instantiate(moveIndicator, new Vector3(x, y, -3.0f), Quaternion.identity);
            moveIndicator miScript = mi.GetComponent<moveIndicator>();
            //Only differing line of code from the movement script - optimisation could include new parameter that specifies if attacking or not
            miScript.attacking = true;
            miScript.enPassant = enPassant;
            miScript.setPromotionMove(promotionMove);
            miScript.setPieceReference(gameObject);
            miScript.setCoordinates(boardMatrixX, boardMatrixY);
            miScript.Initialize();
        }
    }

    //Movement indicator spawn logic for individual or miscellanious movement such as "en-passante" or knight movement, these movement types follow no patten so it is easier to use this standardised method
    public void pointMoveIndicator(int x, int y, string castleSide = "null")
    {
        main sc = boardController.GetComponent<main>();
        if(castleSide != "null")
        {
            //Castle move indicator
            if(sc.positionOnBoard(x,y))
            {
                moveIndicatorSpawn(x, y, false, castleSide);
            }
        }
        else
        {
            if(sc.positionOnBoard(x, y))
            {
                GameObject cp = sc.GetPosition(x, y);
                if(cp == null)
                {
                    moveIndicatorSpawn(x, y);
                } else if (cp.GetComponent<chessPiece>().team != team)
                {
                    moveIndicatorAttackSpawn(x, y);
                }
            }
        }
    }

    //Movement indicator spawn logic for queens and rooks - Cardinal movement
    public void lineMoveIndicator(int incrementX, int incrementY)
    {
        //Adding the increment in the desired cardinal direction
        main sc = boardController.GetComponent<main>();
        int x = boardX + incrementX;
        int y = boardY + incrementY;

        //Looping through all of the spaces along the chosen direction, checking if they do not contain any pieces, if no pieces are detected, spawn an indicator and increment further
        while (sc.positionOnBoard(x,y) && sc.GetPosition(x,y) == null)
        {
            moveIndicatorSpawn(x, y);
            x += incrementX;
            y += incrementY;
        } 

        //If the team of the piece that has been encountered by the indicator spawner is of an opposing team, must show the attack indicator instead
        if (sc.positionOnBoard(x, y) && sc.GetPosition(x, y).GetComponent<chessPiece>().team != team)
        {
            moveIndicatorAttackSpawn(x, y);
        }
    }

    //Movement indicator spawn logic for the knights
    public void knightMoveIndicator()
    {
        // All of the possible movement positions for the knights
        pointMoveIndicator(boardX + 2, boardY + 1);
        pointMoveIndicator(boardX + 2, boardY - 1);
        pointMoveIndicator(boardX + 1, boardY - 2);
        pointMoveIndicator(boardX - 1, boardY - 2);
        pointMoveIndicator(boardX - 2, boardY - 1);
        pointMoveIndicator(boardX - 2, boardY + 1);
        pointMoveIndicator(boardX - 1, boardY + 2);
        pointMoveIndicator(boardX + 1, boardY + 2);
    }

    //Movement indicator spawn logic for the kings movement
    public void kingMoveIndicator()
    {
        pointMoveIndicator(boardX, boardY - 1);
        pointMoveIndicator(boardX, boardY + 1);
        pointMoveIndicator(boardX - 1, boardY - 1);
        pointMoveIndicator(boardX - 1, boardY);
        pointMoveIndicator(boardX - 1, boardY + 1);
        pointMoveIndicator(boardX + 1, boardY - 1);
        pointMoveIndicator(boardX + 1, boardY);
        pointMoveIndicator(boardX + 1, boardY + 1);

        //Add functionality for castling here
        main sc = boardController.GetComponent<main>();
        if(this.hasMoved == false && sc.GetPosition(boardX + 1, boardY) == null && sc.GetPosition(boardX + 2, boardY) == null && sc.GetPosition(boardX + 3, boardY).GetComponent<chessPiece>().hasMoved == false)
        {
            //Kingside castle
            pointMoveIndicator(boardX + 3, boardY, "kingSide");

        }
        if(this.hasMoved == false && sc.GetPosition(boardX - 1, boardY) == null && sc.GetPosition(boardX -2, boardY) == null && sc.GetPosition(boardX -3, boardY) == null && sc.GetPosition(boardX - 4, boardY).GetComponent<chessPiece>().hasMoved == false)
        {
            //Queenside castle
            pointMoveIndicator(boardX - 4, boardY, "queenSide");
        }
    }

    //Movement indicator spawn logic for the pawns 
    public void pawnMoveIndicator(int x, int y, int selfX, int selfY)
    {
        //Is position valid or not
        main sc = boardController.GetComponent<main>();
        if(sc.positionOnBoard(x, y))
        {
            if(sc.GetPosition(x, y) == null)
            {
                moveIndicatorSpawn(x, y);
            }

            //If the pawn has not moved, they are able to go two spaces forward instead of one
            if(this.name == "blackPawn" && hasMovedDouble == false  && boardController.GetComponent<main>().gridPositions[x, y - 1] == null)
            {
                moveIndicatorSpawn(x, y - 1, true);
            }
            else if(this.name == "whitePawn" && hasMovedDouble == false && boardController.GetComponent<main>().gridPositions[x, y + 1] == null)
            {
                moveIndicatorSpawn(x,y + 1, true);
            }

            //Checking whether there are pieces in range of the pawns attack, one forward + to the left, and one forward + to the right
            if(sc.positionOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) != null && sc.GetPosition(x + 1, y).GetComponent<chessPiece>().team != team)
            {
                if(y == 7 && this.team == "white")
                {
                    Debug.Log("white pawn may attack");
                    moveIndicatorAttackSpawn(x + 1, y, false, true);
                }
                else if(y == 0 && this.team == "black")
                {
                    Debug.Log("white pawn may attack");
                    moveIndicatorAttackSpawn(x + 1, y, false, true);
                }
                else 
                {
                    Debug.Log("white pawn may attack");
                    moveIndicatorAttackSpawn(x + 1, y);
                }
            }
            if(sc.positionOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null && sc.GetPosition(x - 1, y).GetComponent<chessPiece>().team != team)
            {
                if(y == 7 && this.team == "white")
                {
                    Debug.Log("pawn may attack");
                    moveIndicatorAttackSpawn(x - 1, y, false, true);
                }
                else if(y == 0 && this.team == "black")
                {
                    Debug.Log("pawn may attack");
                    moveIndicatorAttackSpawn(x - 1, y, false, true);
                }
                else 
                {
                    Debug.Log("pawn may attack");
                    moveIndicatorAttackSpawn(x - 1, y);
                }
            }

            //Checking whether the pawn can perform an en-passant in either attack directions
            if(sc.positionOnBoard(selfX - 1, selfY) && sc.GetPosition(selfX - 1, selfY) != null && sc.GetPosition(selfX - 1, selfY).GetComponent<chessPiece>().team != team && sc.GetPosition(selfX - 1, selfY).GetComponent<chessPiece>().lastMoveWasDouble == true)
            {
                moveIndicatorAttackSpawn(x - 1, y, true);
            }
            if(sc.positionOnBoard(selfX + 1, selfY) && sc.GetPosition(selfX + 1, selfY) != null && sc.GetPosition(selfX + 1, selfY).GetComponent<chessPiece>().team != team && sc.GetPosition(selfX + 1, selfY).GetComponent<chessPiece>().lastMoveWasDouble == true)
            {
                moveIndicatorAttackSpawn(x + 1, y, true);
            }

            //Piece promotion
            if(this.name == "blackPawn" && y == 0)
            {
                if(sc.GetPosition(x,y) == null)
                {
                    moveIndicatorSpawn(x, y, false, "null", true);
                }

            }
            else if (this.name == "whitePawn" && y == 7)
            {
                if(sc.GetPosition(x,y) == null)
                {
                    moveIndicatorSpawn(x, y, false, "null", true);
                }
            }
        }

    }


    //Func to indicate which spaces each piece can move to.
    public void initiateMoveIndicators()
    {
        switch(this.name)
        {
            //Black queen and white queen can move in all directions (8)
            case "blackQueen":
            case "whiteQueen":
                lineMoveIndicator(1,0);
                lineMoveIndicator(0,1);
                lineMoveIndicator(1,1);
                lineMoveIndicator(-1,0);
                lineMoveIndicator(-1,-1);
                lineMoveIndicator(-1,1);
                lineMoveIndicator(1,-1);
                lineMoveIndicator(0,-1);
                break;
            //Knights can only move with their specificly set move of 2 "forward" and then 1 after a right angle - or the inverse of this
            case "blackKnight":
            case "whiteKnight":
                knightMoveIndicator();
                break;
            //Bishops can move on diagonals, for a total of four different directions
            case "blackBishop":
            case "whiteBishop":
                lineMoveIndicator(1,1);
                lineMoveIndicator(1,-1);
                lineMoveIndicator(-1,1);
                lineMoveIndicator(-1,-1);
                break;
            //The kings have a specific type of move, one where they are only able to move by one square in each direction + potential for castling
            case "blackKing":
            case "whiteKing":
                kingMoveIndicator();
                break;
            //The rooks can move in the 4 cardinal directions only
            case "blackRook":
            case "whiteRook":
                lineMoveIndicator(1,0);
                lineMoveIndicator(0,1);
                lineMoveIndicator(-1,0);
                lineMoveIndicator(0,-1);
                break;
            //Pawns from different teams move in one direction only, the black pawn moving towards negative Y and the white pawns moving towards positive Y
            case "blackPawn":
                pawnMoveIndicator(boardX, boardY - 1, this.getBoardX(), this.getBoardY());
                break;
            case "whitePawn":
                pawnMoveIndicator(boardX, boardY + 1, this.getBoardX(), this.getBoardY());
                break;
                
        }
    }


    //Looping through all of the gameObjects in the scene with the tag "MoveIndicator" and destorying them from the scene
    public void destroyMoveIndicators()
    {
        GameObject[] moveIndicators = GameObject.FindGameObjectsWithTag("MoveIndicator");
        for (int i = 0; i < moveIndicators.Length; i++)
        {
            Destroy(moveIndicators[i]);
        }
    }

    //When a piece is clicked, display the movement indicators and destroy the previous ones
    private void OnMouseUp()
    {
        onMouseUpScript();  
    }

    public void onMouseUpScript(bool initiatedByAI = false)
    {
        if(initiatedByAI)
        {  
            currentMove.Clear();
            initiateMoveIndicators();
            return;
        }
        //Is the piece that has been clicked on a part of the team that is currently playing?
        if(!boardController.GetComponent<main>().isGameOver() && boardController.GetComponent<main>().getCurrentTeam() == team)
        {
            Debug.Log("FART FART FART FART FART ");
            destroyMoveIndicators();
            initiateMoveIndicators();
        }
    }


    //Init of the piece - What sprite it may have along with its name and other information
    public void Initialize()
    {
        boardController = GameObject.FindGameObjectWithTag("GameController");
        
        //Switch statement to set the sprite of the piece  NEED TO FIND WAY TO CLEAN THIS CODE AND MAKE FUNC WITH PARAM ("PIECENAME") or something similar to optimise
        switch(this.name)
        {
            case "blackKing" : this.GetComponent<SpriteRenderer>().sprite = blackKing; team = "black"; break;
            case "blackQueen" : this.GetComponent<SpriteRenderer>().sprite = blackQueen; team = "black"; break;
            case "blackBishop" : this.GetComponent<SpriteRenderer>().sprite = blackBishop; team = "black"; break;
            case "blackKnight" : this.GetComponent<SpriteRenderer>().sprite = blackKnight; team = "black"; break;
            case "blackRook" : this.GetComponent<SpriteRenderer>().sprite = blackRook; team = "black"; break;
            case "blackPawn" : this.GetComponent<SpriteRenderer>().sprite = blackPawn; team = "black"; break;

            case "whiteKing" : this.GetComponent<SpriteRenderer>().sprite = whiteKing; team = "white"; break;
            case "whiteQueen" : this.GetComponent<SpriteRenderer>().sprite = whiteQueen; team = "white"; break;
            case "whiteBishop" : this.GetComponent<SpriteRenderer>().sprite = whiteBishop; team = "white"; break;
            case "whiteKnight" : this.GetComponent<SpriteRenderer>().sprite = whiteKnight; team = "white"; break;
            case "whiteRook" : this.GetComponent<SpriteRenderer>().sprite = whiteRook; team = "white"; break;
            case "whitePawn" : this.GetComponent<SpriteRenderer>().sprite = whitePawn; team = "white"; break;
        }

        //using the coordinate offset function
        setCoordinates();


    }


}
