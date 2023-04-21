using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class main : MonoBehaviour
{
    public GameObject chessPiece;
    //Giving the board grid locations for each piece
    public GameObject[,] gridPositions = new GameObject[8,8];

    //setting up arrays to store the different teams pieces
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];

    //FILO stack type datastructure for storing pieces
    private List<GameObject> takenStack = new List<GameObject>();
    private List<GameObject> promotionStack = new List<GameObject>();

    public List<List<short>> moves = new List<List<short>>();
    public List<List<short>> movesMadeList = new List<List<short>>();

    //setting current player and gamestate values
    private string currentTeam = "black";
    private bool whiteToMove = true;    
    private bool gameFinished = false;
    private bool isWhiteAI = true;
    private bool isBlackAI = true;

    public GameObject spawnPiece(string name, int x, int y)
    {
        GameObject obj = Instantiate(chessPiece, new Vector3(0,0,-1), Quaternion.identity);
        chessPiece piece = obj.GetComponent<chessPiece>();
        piece.name = name;
        piece.tag = name;

        //If the piece is controlled by ai, make sure the class is aware
        if (piece.name.Contains("white") && isWhiteAI)
        {
            piece.isAiControlled = true;
        }
        else if(piece.name.Contains("black") && isBlackAI)
        {
            piece.isAiControlled = true;
        }
        if(piece.name.Contains("white") && this.GetComponent<aiController>().controllingBlack == true)
        {
            piece.isPlayerControlled = true;
        }
        else if(piece.name.Contains("black") && this.GetComponent<aiController>().controllingBlack == false)
        {
            piece.isPlayerControlled = true;
        }

        piece.setBoardCoordinates(x,y);
        piece.Initialize();
        return obj;
    }

    //Code for making and unmaking moves - using this encoding table https://www.chessprogramming.org/Encoding_Moves
    public void makeMove(List<short> moveId)
    {
        short fromSquare = moveId[0];
        short toSquare = moveId[1];
        short promotion = moveId[2];
        short capture = moveId[3];
        short special1 = moveId[4];
        short special2 = moveId[5];
        //Debug.Log("MAKING MOVE FROM: " + fromSquare + " TO " + toSquare);
        
        //converting the short number into grid positions
        GameObject fromObj = gridPositions[fromSquare % 8, fromSquare / 8];
        //Can be null - be careful
        GameObject toObj = gridPositions[toSquare % 8, toSquare / 8];

        //Quiet move
        if(promotion + capture + special1 + special2 == 0)
        {
            gridPositions[toSquare % 8, toSquare / 8] = fromObj;
            setPositionEmpty(fromSquare % 8, fromSquare / 8);
            //Debug.Log(gridPositions[toSquare % 8, toSquare / 8].name);
            //if(gridPositions[fromSquare % 8, fromSquare / 8] == null){Debug.Log("MOVE MADE CORRECTLY");}
        }

        //Double pawn push
        if(promotion + capture + special1 == 0 && special2 == 1)
        {
            Debug.Log("Double pawn push TELEMETRY: ");
            if(gridPositions[toSquare % 8, toSquare / 8] == null)
            {
                Debug.Log("TOSQUARE IS GOOD TO RECIEVE MOVE");
            }
            else
            {
                Debug.Log("ERROR, PAWN IS MOVING TO OCCUPIED SPACE");
            }
            gridPositions[toSquare % 8, toSquare / 8] = fromObj;
            setPositionEmpty(fromSquare % 8, fromSquare / 8);
            fromObj.GetComponent<chessPiece>().hasMovedDouble = true;
        }

        //capture
        if(promotion == 0 && capture == 1 && special1 + special2 == 0)
        {
            //Adding the taken piece to the buffer so it can be reversed later
            takenStack.Add(gridPositions[toSquare % 8, toSquare / 8]);
            setPositionEmpty(toSquare % 8, toSquare / 8);

            gridPositions[toSquare % 8, toSquare / 8] = fromObj;
            setPositionEmpty(fromSquare % 8, fromSquare / 8);
        }
        //castling
        //Kingside
        if(promotion + capture == 0 && special1 == 1 && special2 == 0)
        { 
            gridPositions[fromSquare % 8, fromSquare / 8] = toObj;
            setPositionEmpty(toSquare % 8, toSquare / 8);
            toSquare -= 1;
            fromSquare += 1;
            gridPositions[fromSquare % 8, fromSquare / 8] = toObj;
            gridPositions[toSquare % 8, toSquare / 8] = fromObj;
        }
        //Queenside
        else if(promotion + capture == 0 && special1 + special2 == 2)
        {
            setPositionEmpty(toSquare % 8, toSquare / 8);
            setPositionEmpty(fromSquare % 8, fromSquare / 8);

            toSquare += 2;
            fromSquare -= 1;
            gridPositions[fromSquare % 8, fromSquare / 8] = toObj;
            gridPositions[toSquare % 8, toSquare / 8] = fromObj;
        }
        //EP capture
        else if(promotion == 0 && capture == 1 && special1 == 0 && special2 == 1)
        {
            if(toSquare - fromSquare < 0)
            {
                //black EP
                if(toSquare == fromSquare - 7)
                {
                    //EP in the square one positive to the black pawn
                    takenStack.Add(gridPositions[(fromSquare + 1) % 8, (fromSquare + 1) / 8]);
                    setPositionEmpty((fromSquare + 1) % 8, (fromSquare + 1) / 8);

                    gridPositions[toSquare % 8, toSquare / 8] = fromObj;
                }
                else
                {
                    //EP in the square one negative to the black pawn
                    takenStack.Add(gridPositions[(fromSquare - 1) % 8, (fromSquare - 1) / 8]);
                    setPositionEmpty((fromSquare - 1) % 8, (fromSquare - 1) / 8);
                    setPositionEmpty(fromSquare % 8, fromSquare / 8);

                    gridPositions[toSquare % 8, toSquare / 8] = fromObj;
                }
            }
            else
            {
                //White EP
                if(toSquare == fromSquare + 7)
                {
                    takenStack.Add(gridPositions[(fromSquare - 1) % 8, (fromSquare - 1) / 8]);
                    setPositionEmpty((fromSquare - 1) % 8, (fromSquare - 1) / 8);
                    setPositionEmpty(fromSquare % 8, fromSquare / 8);

                    gridPositions[toSquare % 8, toSquare / 8] = fromObj;
                }
                else
                {
                    takenStack.Add(gridPositions[(fromSquare + 1) % 8, (fromSquare + 1) / 8]);
                    setPositionEmpty((fromSquare + 1) % 8, (fromSquare + 1) / 8);
                    setPositionEmpty(fromSquare % 8, fromSquare / 8);

                    gridPositions[toSquare % 8, toSquare / 8] = fromObj;
                }
            }
        }
        //PROMOTIONS    
        if(promotion == 1 && capture == 0)
        {
            string teamName = "null";
            //White piece promotion
            if(toSquare > 55)
            {
                teamName = "white";
            }
            //Black piece promotion
            else
            {
                teamName = "black";
            }
            promotionStack.Add(gridPositions[fromSquare % 8, fromSquare / 8]);
            setPositionEmpty(fromSquare % 8, fromSquare / 8);            
            //Knight promotion
            if(special1 + special2 == 0)
            {
                spawnPiece((teamName + "Knight"), toSquare % 8, toSquare / 8);
            }
            //Bishop promotion
            else if(special1 == 0 && special2 == 1)
            {
                spawnPiece((teamName + "Bishop"), toSquare % 8, toSquare / 8);                
            }
            //Rook promotion
            else if(special1 == 1 && special2 == 0)
            {
                spawnPiece((teamName + "Rook"), toSquare % 8, toSquare / 8);
            }
            //Queen promotion
            else if(special1 + special2 == 2)
            {
                spawnPiece((teamName + "Queen"), toSquare % 8, toSquare / 8);
            }
        }
        //CAPTURE PROMOTIONS
        else if(promotion == 1 && capture == 1)
        {
            string teamName = "null";
            //White piece promotion
            if(toSquare > 55)
            {
                teamName = "white";
            }
            //Black piece promotion
            else
            {
                teamName = "black";
            }

            //Adding pieces to relevant storage stacks
            takenStack.Add(toObj);
            setPositionEmpty(toSquare % 8, toSquare / 8);
            promotionStack.Add(gridPositions[fromSquare % 8, fromSquare / 8]);
            setPositionEmpty(fromSquare % 8, fromSquare / 8);   

            //Knight promotion
            if(special1 + special2 == 0)
            {
                spawnPiece((teamName + "Knight"), toSquare % 8, toSquare / 8);
            }
            //Bishop promotion
            else if(special1 == 0 && special2 == 1)
            {
                spawnPiece((teamName + "Bishop"), toSquare % 8, toSquare / 8);                 
            }
            //Rook promotion
            else if(special1 == 1 && special2 == 0)
            {
                spawnPiece((teamName + "Rook"), toSquare % 8, toSquare / 8);
            }
            //Queen promotion
            else if(special1 + special2 == 2)
            {
                spawnPiece((teamName + "Queen"), toSquare % 8, toSquare / 8);
            }
        }

        //ACTUALLY UPDATE PIECE POSITION ON BOARD

        fromObj.GetComponent<chessPiece>().setBoardX(toSquare % 8);
        fromObj.GetComponent<chessPiece>().setBoardY(toSquare / 8);

        gridPositions[toSquare % 8, toSquare / 8].GetComponent<chessPiece>().setCoordinates();
        setPosition(fromObj);




    }    

    public void unMakeMove(List<short> moveId)
    {
        short fromSquare = moveId[0];
        short toSquare = moveId[1];
        short promotion = moveId[2];
        short capture = moveId[3];
        short special1 = moveId[4];
        short special2 = moveId[5];

        //Debug.Log("UNMAKING MOVE FROM: " + toSquare + " TO " + fromSquare);

        //fuck my life i dont wanna make this fucking method not one bit
        //PLEASE REMEMBER IN THE UNMAKE METHOD - TOSQUARE IS THE SQUARE THAT WAS MOVED TO BEFORE, NOW BECOMING THE FROMSQUARE

        //Writing this method is gonna make me go insane (yes == no rn)

        //fromObj is basically now the toObj
        //converting the short number into grid positions
        GameObject fromObj = gridPositions[fromSquare % 8, fromSquare / 8];
        //Can be null - be careful
        GameObject toObj = gridPositions[toSquare % 8, toSquare / 8];

        //Quiet move
        if(promotion + capture + special1 + special2 == 0)
        {
            gridPositions[fromSquare % 8, fromSquare / 8] = toObj;
            setPositionEmpty(toSquare % 8, toSquare / 8);
        }

        //Double pawn push
        if(promotion + capture + special1 == 0 && special2 == 1)
        {
            toObj.GetComponent<chessPiece>().hasMovedDouble = false;
            gridPositions[fromSquare % 8, fromSquare / 8] = toObj;
            setPositionEmpty(toSquare % 8, toSquare / 8);
        }

        //capture
        if(promotion == 0 && capture == 1 && special1 + special2 == 0)
        {
            //Adding the taken piece to the buffer so it can be reversed later
            //Get most recently added item to the taken stack and set it to the toSquare 
            gridPositions[toSquare % 8, toSquare / 8] = takenStack[^1];
            takenStack.Remove(takenStack[^1]);      
            gridPositions[fromSquare % 8, fromSquare / 8] = toObj;
        }
        //castling
        //Kingside
        if(promotion + capture == 0 && special1 == 1 && special2 == 0)
        {
            //Setting king back to original location
            gridPositions[fromSquare % 8, fromSquare / 8] = gridPositions[(toSquare - 1) % 8, (toSquare - 1) / 8]; 
            setPositionEmpty((toSquare - 1) % 8, (toSquare - 1) / 8);
            
            //Setting rook back to original location
            gridPositions[toSquare % 8, toSquare / 8] = gridPositions[(toSquare - 2) % 8, (toSquare - 2) / 8];
            setPositionEmpty((toSquare - 2) % 8, (toSquare - 2) / 8);
        }
        //Queenside
        else if(promotion + capture == 0 && special1 + special2 == 2)
        {
            //Setting king back to original location
            gridPositions[fromSquare % 8, fromSquare / 8] = gridPositions[(toSquare - 2) % 8, (toSquare - 2) / 8];
            setPositionEmpty((toSquare - 2) / 8, (toSquare - 2) % 8);
            
            //Setting rook back to original location
            gridPositions[toSquare % 8, toSquare / 8] = gridPositions[(fromSquare - 1) % 8, (fromSquare - 1) / 8];
            setPositionEmpty((fromSquare - 1) % 8, (fromSquare - 1) / 8);
        }

        //ENPASSANT REVERSAL - NO IDEA HOW IM GONNA DO THIS
        else if(promotion == 0 && capture == 1 && special1 == 0 && special2 == 1)
        {
            if(toSquare - fromSquare < 0)
            {
                //black EP
                if(toSquare == fromSquare - 7)
                {
                    //Taking the pawn off of the stack and replacing it on the board - 
                    gridPositions[(fromSquare + 1) % 8, (fromSquare + 1) / 8] = takenStack[^1];
                    takenStack.Remove(takenStack[^1]); 

                    //Moving the attacking pawn back to where it was previously
                    gridPositions[fromSquare % 8, fromSquare / 8] = toObj;
                    setPositionEmpty(toSquare % 8, toSquare / 8);
                }
                else
                {
                    //EP in the square one negative to the black pawn
                    gridPositions[(fromSquare - 1) / 8, (fromSquare - 1)] = takenStack[^1];
                    takenStack.Remove(takenStack[^1]);
                    
                    gridPositions[fromSquare % 8, fromSquare / 8] = toObj;
                    setPositionEmpty(toSquare % 8, toSquare / 8);
                }
            }
            else
            {
                //White EP
                if(toSquare == fromSquare + 7)
                {
                    gridPositions[(fromSquare - 1) % 8, (fromSquare - 1) / 8] = takenStack[^1];
                    takenStack.Remove(takenStack[^1]);

                    gridPositions[fromSquare % 8, fromSquare / 8] = toObj;
                    setPositionEmpty(toSquare % 8, toSquare / 8);
                }
                else
                {
                    gridPositions[(fromSquare + 1) % 8, (fromSquare + 1) / 8] = takenStack[^1];
                    takenStack.Remove(takenStack[^1]); 

                    //Moving the attacking pawn back to where it was previously
                    gridPositions[fromSquare % 8, fromSquare / 8] = toObj;
                    setPositionEmpty(toSquare % 8, toSquare / 8);
                }
            }
        }
        //PROMOTIONS    
        if(promotion == 1 && capture == 0)
        {
            setPositionEmpty(toSquare % 8, toSquare / 8);
            gridPositions[fromSquare % 8, fromSquare / 8] = promotionStack[^1];
            promotionStack.Remove(promotionStack[^1]);
        }
        //CAPTURE PROMOTIONS
        else if(promotion == 1 && capture == 1)
        {
            setPositionEmpty(toSquare % 8, toSquare / 8);
            gridPositions[fromSquare % 8, fromSquare / 8] = promotionStack[^1];
            promotionStack.Remove(promotionStack[^1]);

            gridPositions[toSquare % 8, toSquare / 8] = takenStack[^1];
            takenStack.Remove(takenStack[^1]);
        }

        //ACTUALLY UPDATE PIECE POSITION ON BOARD
        toObj.GetComponent<chessPiece>().setBoardX(fromSquare % 8);
        toObj.GetComponent<chessPiece>().setBoardY(fromSquare / 8);

        gridPositions[fromSquare % 8, fromSquare / 8].GetComponent<chessPiece>().setCoordinates();
        setPosition(toObj);


    }

    //set the positions of the pieces
    public void setPosition(GameObject obj)
    {
        chessPiece piece = obj.GetComponent<chessPiece>();
        gridPositions[piece.getBoardX(), piece.getBoardY()] = obj;
    }

    public void setPositionEmpty(int x, int y)
    {
        gridPositions[x,y] = null;
    }

    public GameObject GetPosition(int x, int y)
    {
        return gridPositions[x,y];
    }


    //Returning the team that is currently playing
    public string getCurrentTeam()
    {
        return currentTeam;
    }

    public bool getWhiteToMove()
    {
        return whiteToMove; 
    }

    //Returning the game finished state
    public bool isGameOver()
    {
        return gameFinished;
    }

    //Changing which team gets to move based on whomevers turn it is
    public void nextTurn(bool gamestateChange = false)
    {

        if(currentTeam == "white")
        {
            currentTeam = "black";
            whiteToMove = false;
        }
        else
        {
            currentTeam = "white";
            whiteToMove = true; 
        }

        if(gamestateChange)
        {
            Debug.Log("NEXT TURN");
            this.GetComponent<aiController>().initiateAIMove();
        }
    }

    //Check for whether the game has finished or not
    public void Update()
    {
        if(gameFinished == true && Input.GetMouseButtonDown(0))
        {
            gameFinished = false;

            SceneManager.LoadScene("main");
        }
    }

    //Setting the text renderers to correctly display gamestate
    public void winner(string playerWinner)
    {
        gameFinished = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<TMPro.TextMeshProUGUI>().enabled = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<TMPro.TextMeshProUGUI>().text = playerWinner + " Wins!";
        GameObject.FindGameObjectWithTag("RestartText").GetComponent<TMPro.TextMeshProUGUI>().enabled = true;
    }


    public bool positionOnBoard(int x, int y)
    {
        if(x < 0 || y < 0 || x >= gridPositions.GetLength(0) || y >= gridPositions.GetLength(1)) return false;
            return true;
    }

    //Trying to write functions to make the instantiating section easier to look at, no luck so far
    void spawnWhitePawns()
    {
        //spawn pawns here 
        for(int i = 0; i < 7; i++)
        {
            spawnPiece("whitePawn", i, 1);
        }
    }
    void spawnBlackPawns()
    {
        for(int i = 0; i < 7; i++)
        {
            spawnPiece("blackPawn", i,6);
        }
    }


    public void generateMoves()
    {
        Debug.Log("moves list is about to be cleared completely");
        moves.Clear();
        for(int i = 0; i < gridPositions.Length; i++)
        {
            //Generate moves for given piece
            if(gridPositions[i % 8, i / 8] != null)
            {
                if(gridPositions[i % 8, i / 8].GetComponent<chessPiece>().getTeam() == currentTeam){gridPositions[i % 8, i / 8].GetComponent<chessPiece>().onMouseUpScript(true);}
            }
        }
    }



    void Start()
    {
        // instantiating the pieces
        playerWhite = new GameObject[]
        {
            spawnPiece("whiteKing", 4, 0), spawnPiece("whiteQueen", 3 , 0), spawnPiece("whiteBishop", 2, 0), spawnPiece("whiteBishop", 5, 0),
            spawnPiece("whiteKnight", 1, 0), spawnPiece("whiteKnight", 6, 0), spawnPiece("whiteRook", 0, 0), spawnPiece("whiteRook", 7, 0),
            spawnPiece("whitePawn", 0, 1), spawnPiece("whitePawn", 1 , 1), spawnPiece("whitePawn", 2, 1), spawnPiece("whitePawn", 3, 1),
            spawnPiece("whitePawn", 4, 1), spawnPiece("whitePawn", 5, 1), spawnPiece("whitePawn", 6, 1), spawnPiece("whitePawn", 7, 1),
        };

        playerBlack = new GameObject[]
        {
            spawnPiece("blackKing", 4, 7), spawnPiece("blackQueen", 3 , 7), spawnPiece("blackBishop", 2, 7), spawnPiece("blackBishop", 5, 7),
            spawnPiece("blackKnight", 1, 7), spawnPiece("blackKnight", 6, 7), spawnPiece("blackRook", 0, 7), spawnPiece("blackRook", 7, 7),
            spawnPiece("blackPawn", 0, 6), spawnPiece("blackPawn", 1 , 6), spawnPiece("blackPawn", 2, 6), spawnPiece("blackPawn", 3, 6),
            spawnPiece("blackPawn", 4, 6), spawnPiece("blackPawn", 5, 6), spawnPiece("blackPawn", 6, 6), spawnPiece("blackPawn", 7, 6),
        };

        //setting piece positions
        for (int i = 0; i < playerWhite.Length; i++)
        {
            setPosition(playerWhite[i]);
            setPosition(playerBlack[i]);
        }

        //Initialize a.i controller
        this.GetComponent<aiController>().Initialize();

        
    }

}
