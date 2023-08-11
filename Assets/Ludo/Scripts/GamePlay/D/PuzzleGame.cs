using UnityEngine;
using System.Collections;

public class PuzzleGame : MonoBehaviour
{
    // Array to hold the puzzle pieces
    public GameObject[] puzzlePieces;

    // Index of the current puzzle piece
    private int currentPiece = 0;

    void Start()
    {
        // Shuffle the puzzle pieces
        ShufflePuzzlePieces();
    }

    void Update()
    {
        // Check if the player has placed the current puzzle piece in the correct position
        if (puzzlePieces[currentPiece].transform.position == puzzlePieces[currentPiece].GetComponent<PuzzlePiece>().correctPosition)
        {
            currentPiece++;
            // Check if the player has completed the puzzle
            if (currentPiece >= puzzlePieces.Length)
            {
                Debug.Log("Puzzle complete!");
            }
        }
    }

    void ShufflePuzzlePieces()
    {
        // Shuffle the puzzle pieces using the Fisher-Yates shuffle algorithm
        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            int randomIndex = Random.Range(i, puzzlePieces.Length);
            GameObject temp = puzzlePieces[i];
            puzzlePieces[i] = puzzlePieces[randomIndex];
            puzzlePieces[randomIndex] = temp;
        }
    }
}
