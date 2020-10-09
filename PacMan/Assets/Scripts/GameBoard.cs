using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;



/// <summary>
/// The game board class stores data relating to a level, such as the dimensions, total number of pellets, and game objects in the space.
/// </summary>
public class GameBoard : MonoBehaviour
{
    [SerializeField]
    private static int boardWidth = 28;
    [SerializeField]
    private static int boardHeight = 36;

    /// <summary>
    /// The total number of dots or pellets in the maze, which PacMan will need to consume to beat the level.
    /// </summary>
    /// <value>
    /// Count of pellets in the maze.
    /// </value>
    public int totalDots;

    /// <summary>
    /// The player's score for the board, determined by the amount of consumables eaten.
    /// </summary>
    /// <value>
    /// The player's score.
    /// </value>
    public int score = 0;
    public TextMeshProUGUI scoreText;

    public double elapsedTime = 0f;
    public TextMeshProUGUI timerText;

    public int lives = 3;
    public TextMeshProUGUI livesText;

    /// An array referencing nodes within the game board.
    private static List<GameObject> nodes = new List<GameObject> ();

    // An array referencing the pellets within the maze.
    private static List<GameObject> dots = new List<GameObject> ();
    
    // A reference to the player
    private GameObject pacMan;

    // A reference to the ghost(s)
    private List<GameObject> ghosts = new List<GameObject> ();


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
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Dot");
        foreach (GameObject obj in objects) {
            if (obj != null) {
                dots.Add(obj);
            }
        }
        dots = dots.OrderBy(m => m.transform.position.x).ThenBy(m => m.transform.position.y).ToList();
        totalDots = dots.Count;
        score = 0;
        
        objects = GameObject.FindGameObjectsWithTag("Node");
        foreach (GameObject obj in objects) {
            if (obj != null) {
                nodes.Add(obj);
            }
        }
        nodes = nodes.OrderBy(m => m.transform.position.x).ThenBy(m => m.transform.position.y).ToList();

        pacMan = GameObject.FindGameObjectWithTag("PacMan");

        objects = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject obj in objects) {
            if (obj != null) {
                ghosts.Add(obj);
            }
        }
    }

    void Update()
    {
        UpdateTimer();
        if (dots.Count > 0) {
            ConsumeAtPosition((Vector2)pacMan.transform.position);
        }
    }

    void UpdateTimer()
    {
        elapsedTime += Time.deltaTime;
        TimeSpan t = TimeSpan.FromSeconds(elapsedTime);
        timerText.text = string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
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
    public void ConsumeAtPosition(Vector2 pos)
    {
        GameObject obj = dots.FirstOrDefault(m => (pos - (Vector2)m.transform.position).sqrMagnitude < 1f); // = dot[Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)];

        if (obj != null) {
            Consumable pellet = obj.GetComponent<Consumable> ();

            if (pellet != null && !pellet.didConsume) {
                pellet.didConsume = true;
                obj.GetComponent<SpriteRenderer> ().enabled = false;
                
                score += pellet.pointsValue;
                scoreText.text = score.ToString();

                dots.Remove(obj);
            }
        }
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
