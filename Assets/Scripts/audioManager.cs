using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioManager : MonoBehaviour
{
    [SerializeField] private AudioSource attackSound; //Chesspiece attack sound
    [SerializeField] private AudioSource moveSound;   //Chesspiece move sound

    public AudioSource returnAudio(string requestedSound)
    {
        if(requestedSound == "moveSound")
        {
            return(moveSound);
        }
        else if(requestedSound  == "attackSound")
        {
            return(attackSound);
        }
        return null;
    }
}
