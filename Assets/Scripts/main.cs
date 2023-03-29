using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class main : MonoBehaviour
{
    public GameObject chessPiece;
    //Giving the board grid locations for each piece
    private GameObject[,] gridPositions = new GameObject[8,8];

    //setting up arrays to store the different teams pieces
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];


    //setting current player and gamestate values
    private string currentTeam = "white";
    private bool gameFinished = false;

    public GameObject spawnPiece(string name, int x, int y)
    {
        GameObject obj = Instantiate(chessPiece, new Vector3(0,0,-1), Quaternion.identity);
        chessPiece piece = obj.GetComponent<chessPiece>();
        piece.name = name;
        piece.setBoardCoordinates(x,y);
        piece.Initialize();
        return obj;
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

    //Returning the game finished state
    public bool isGameOver()
    {
        return gameFinished;
    }

    //Changing which team gets to move based on whomevers turn it is
    public void nextTurn()
    {
        if(currentTeam == "white")
        {
            currentTeam = "black";
        }
        else
        {
            currentTeam = "white";
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
        Debug.Log("hooray");
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
    }

}
