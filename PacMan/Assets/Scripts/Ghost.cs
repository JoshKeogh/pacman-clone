using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Ghost class constitutes the base AI and has functionalities common to all of the ghosts.
/// </summary>
public class Ghost : MonoBehaviour
{
    /// <summary>
    /// How quickly the ghost travels through the maze.
    /// </summary>
    /// <value>
    /// The speed of the ghost.
    /// </value>
    public float speed = 3f;

/*
    /// <summary>
    /// The possible states which the ghosts can be in; Chase, Scatter, or Frightened.
    /// </summary>
    public enum Mode {
        Chase,
        Scatter,
        Frightened
    }
    Mode previousMode, currentMode = Mode.Scatter;
*/

    // Objects for keeping track of how the ghost navigates the maze.
    private List<Node> path;
    private Vector2 direction;
    [SerializeField]
    private Node currentNode, targetNode, previousNode;
    // A reference to the player
    private GameObject pacMan;
    // A reference to the level
    private GameBoard board;
    // A reference to the pathfinding
    private PathFinding navigation;


    /// <summary>
    /// The Start() function is called before the first frame update.
    /// The function initializes the ghost by finding the player character in the space and setting a targetNode.
    /// </summary>
    /// <pre>
    /// The script has been attached to a ghost which is in maze, where the maze is connected as a graph via the Node class.
    /// </pre>
    /// </pre>
    /// PacMan is on the board.
    /// </pre>
    /// <post>
    /// The ghost stores a reference to the PacMan object.
    /// </post>
    /// <post>
    /// A target node for the ghost to head towards has been set.
    /// </post>
    void Start()
    {
        pacMan = GameObject.FindGameObjectWithTag("PacMan");
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<GameBoard> ();
        navigation = GameObject.FindGameObjectWithTag("Board").GetComponent<PathFinding> ();
        direction = (targetNode.transform.position- transform.localPosition).normalized;
    }

    /// <summary>
    /// Update() is called once per frame. It will update the logic, then location of the ghosts.
    /// </summary>
    /// <post>
    /// The ghost has not been updated to an invalid state.
    /// </post>
    void Update()
    {
        Move();
        
    }

    /// <summary>
    /// Update the location of the ghost on the board. Continually head in the direction of a target node if one is selected.
    /// </summary>
    /// <post>
    /// The ghost has not over shot its target destination node.
    /// </post>
    /// <post>
    /// The ghost remains within the bounds of the maze.
    /// </post>
    protected void Move()
    {
        if (targetNode == null) {
            targetNode = ChooseNextNode();

        } else if (targetNode != currentNode) {

            if (board.OverShotTarget(transform.localPosition, targetNode, previousNode)) {
                Node otherPortal = board.GetPortal((Vector2)targetNode.transform.position);
                if (otherPortal != null) {
                    currentNode = otherPortal;
                } else {
                    currentNode = targetNode;
                }
                
                transform.localPosition = currentNode.transform.position;
                
                previousNode = currentNode;
                targetNode = ChooseNextNode();
                currentNode = null;
                
                if (targetNode != null) {
                    direction = (targetNode.transform.position - transform.localPosition).normalized;
                }
            } else {

                transform.localPosition += (Vector3)(direction * speed * Time.deltaTime);
            }
        }
    }

    protected Node ChooseNextNode()
    {
        Node target;

        PacMan pm = pacMan.GetComponent<PacMan> ();
        if (pm.targetNode != null) {
            target = pm.targetNode;
        } else if (pm.currentNode != null) {
            target = pm.currentNode;
        } else {
            target = pm.previousNode;
        }

        path = navigation.GetShortestPath(previousNode, target);

        target = null;
        if (path != null && path.Count > 1) {
            target = path[1];
        }

        return target;
    }
}
