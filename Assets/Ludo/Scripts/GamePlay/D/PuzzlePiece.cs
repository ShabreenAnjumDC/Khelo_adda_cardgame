using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    // Vector3 to store the correct position of the puzzle piece
    public Vector3 correctPosition;

    // Boolean to check if the puzzle piece is in the correct position
    public bool isCorrect = false;

    // Reference to the puzzle game script
    public PuzzleGame puzzleGame;

    void Start()
    {
        // Set the initial position of the puzzle piece
        transform.position = correctPosition;
    }

    void Update()
    {
        // Check if the puzzle piece is in the correct position
        if (transform.position == correctPosition)
        {
            isCorrect = true;
        }
        else
        {
            isCorrect = false;
        }
    }

    void OnMouseDown()
    {
        // Check if the puzzle piece is in the correct position
        if (!isCorrect)
        {
            // If not, move the puzzle piece to the correct position
            transform.position = correctPosition;
        }
    }
}
