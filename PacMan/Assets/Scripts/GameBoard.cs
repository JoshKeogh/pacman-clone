using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    private static int boardWidth = 28;
    private static int boardHeight = 36;

    public GameObject[,] board = new GameObject[boardWidth, boardHeight];

    // Start is called before the first frame update
    void Start()
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach (GameObject obj in objects)
        {
            Vector2 pos = obj.transform.position;

            if (obj.name != "PacMan") {
                board[(int)pos.x, (int)pos.y] = obj;
            } else {
                Debug.Log("Found PacMan at " + pos);
            }
        }
    }
}
