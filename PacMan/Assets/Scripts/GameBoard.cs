using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;



/// <summary>
/// The game board class stores data relating to a level, such as the dimensions, total number of pellets, and game objects in the space.
/// </summary>
public class GameBoard : MonoBehaviour
{
    private static int boardWidth = 28;
    private static int boardHeight = 36;

    /// <summary>
    /// The total number of dots or pellets in the maze, which PacMan will need to consume to beat the level.
    /// </summary>
    /// <value>
    /// Count of pellets in the maze.
    /// </value>
    public int dotsRemaining
    {
        get { return _dotsRemaining; }
    }
    private int _dotsRemaining;
    public int totalDots;

    /// <summary>
    /// The player's score for the board, determined by the amount of consumables eaten.
    /// </summary>
    /// <value>
    /// The player's score.
    /// </value>
    public int score = 0;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI livesText;

    public float elapsedTime = 0f;

    public int lives = 3;

    /// An array referencing nodes within the game board.
    private static GameObject[,] nodes = new GameObject[boardWidth, boardHeight];

    // An array referencing the pellets within the maze.
    private static GameObject[,] dots = new GameObject[boardWidth, boardHeight];


    /// <summary>
    /// The Start() function is called before the first frame update and is useful for initialization.
    /// The function sets up the GameBoard class by finding objects like pellets and nodes in the maze and places the objects in an array.
    /// The total number of dots found in the maze is calculated and stored.
    /// </summary>
    /// <pre>
    /// The player has started a level and the game has initialized properly. The maze has been populated with its game objects and the dimensions of the board have been indicated.
    /// </pre>
    /// <post>
    /// The board array has been populated with pertinent game objects from the maze.
    /// </post>
    /// <post>
    /// The total number of pellets in the level has been set.
    /// </post>
    void Start()
    {
        Object[] objects = GameObject.FindGameObjectsWithTag("Dot");
        _dotsRemaining = 0;
        foreach (GameObject obj in objects) {
            dots[Mathf.RoundToInt(obj.transform.position.x), Mathf.RoundToInt(obj.transform.position.y)] = obj;
            _dotsRemaining++;
        }
        totalDots = _dotsRemaining;
        score = 0;

        objects = GameObject.FindGameObjectsWithTag("Node");
        foreach (GameObject obj in objects) {
            nodes[(int)obj.transform.position.x, (int)obj.transform.position.y] = obj;
        }
    }

    /// <summary>
    /// Retrieves the pellet object at a particular position on the game board.
    /// </summary>
    /// <param>
    /// The position within the maze to search for a pellet.
    /// </param>
    /// <return>
    /// The relevant object if a pellet was found, otherwise null.
    /// </return>
    /// <pre>
    /// The GameBoard has initialized and the board was populated with the maze objects.
    /// </pre>
    /// <pre>
    /// The parameter is a valid position on the board.
    /// </pre>
    public GameObject ConsumeAtPosition(Vector2 pos)
    {
        GameObject obj = dots[Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)];

        if (obj != null) {
            Consumable pellet = obj.GetComponent<Consumable> ();

            if (pellet != null && !pellet.didConsume) {
                obj.GetComponent<SpriteRenderer> ().enabled = false;
                pellet.didConsume = true;
                
                score += pellet.pointsValue;
                scoreText.text = score.ToString();

                _dotsRemaining--;
            }
        }

        return obj;
    }

    /// <summary>
    /// Find the node which is at a position on the game board.
    /// </summary>
    /// <param>
    /// The position within the game board to be checked.
    /// </param>
    /// <return>
    /// The node which was found at the specified position within the maze, or null if none was found.
    /// </return>
    /// <pre>
    /// GameBoard.board array has been populated with the maze's nodes.
    /// </pre>
    public Node GetNodeAtPosition(Vector2 pos)
    {
        GameObject obj = nodes[Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)];
        return obj.GetComponent<Node> ();
    }
    
    /// <summary>
    /// Mazes may have nodes which teleport characters to the other side of a maze. This checks whether a position corresponds to a teleporting node and retrieves the destination portal node.
    /// </summary>
    /// <param>
    /// The relevant position on the game board.
    /// </param>
    /// <return>
    /// If the position corresponds to a portal node, returns the corresponding destination portal node, otherwise null.
    /// </return>
    /// <pre>
    /// GameBoard.board array has been populated with the maze's nodes.
    /// </pre>
    /// <pre>
    /// Portal nodes have been connected to their corresponding destination node.
    /// </pre>
    public Node GetPortal(Vector2 pos)
    {
        Node node = nodes[(int)pos.x, (int)pos.y].GetComponent<Node> ();

        if (node != null && node.isPortal) {
            return node.portalReceiver;
        }
        
        return null;
    }
    
    /// <summary>
    /// Determines whether the character moved too far and overshot their target node.
    /// </summary>
    /// <return>
    /// Whether or not the character has travelled past their destination.
    /// </return>
    /// <pre>
    /// The previousNode exists.
    /// </pre>
    /// <pre>
    /// A target node has been set.
    /// </pre>
    public bool OverShotTarget(Vector2 position, Node target, Node previous)
    {
        float nodeToTarget = LengthFromNode(target.transform.position, previous);
        float nodeToSelf = LengthFromNode(position, previous);

        return nodeToSelf > nodeToTarget;
    }

    /// <summary>
    /// Calculate the distance between the previous node and a position.
    /// </summary>
    /// <param>
    /// Position on the game board to be checked.
    /// </param>
    /// <return>
    /// The distance between the target position and previous node.
    /// </return>
    /// <pre>
    /// The node exists.
    /// </pre>
    public float LengthFromNode(Vector2 pos, Node node)
    {
        Vector2 diff = pos - (Vector2)node.transform.position;
        return diff.sqrMagnitude;
    }

    /// <summary>
    /// Find the distance between two points.
    /// </summary>
    /// <param>
    /// The two positions to check. Order does not matter.
    /// </param>
    /// <return>
    /// The distance between the two points.
    /// </return>
    public float GetDistance(Vector2 posA, Vector2 posB)
    {
        return (posB - posA).magnitude;
    }
}
