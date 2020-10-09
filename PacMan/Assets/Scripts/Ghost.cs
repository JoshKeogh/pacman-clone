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
        targetNode = ChooseNextNode();
        direction = (targetNode.transform.position - transform.localPosition).normalized;
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
            if (targetNode != null) {
                direction = (targetNode.transform.position - transform.localPosition).normalized;
            }

        } else if (targetNode != currentNode) {

            if (OverShotTarget(transform.localPosition, targetNode, previousNode)) {
                if (targetNode.isPortal) {
                    currentNode = targetNode.portalReceiver;
                } else {
                    currentNode = targetNode;
                }
                
                transform.localPosition = currentNode.transform.position;
                
                previousNode = currentNode;
                targetNode = ChooseNextNode();
                
                if (targetNode != null) {
                    direction = (targetNode.transform.position - transform.localPosition).normalized;
                    currentNode = null;
                }
            } else {

                transform.localPosition += (Vector3)(direction.normalized * speed * Time.deltaTime);
            }
        }
    }

    protected Node ChooseNextNode()
    {
        Node target = null;

        PacMan pm = pacMan.GetComponent<PacMan> ();
        if (pm.targetNode != null && pm.targetNode != currentNode) {
            target = pm.targetNode;
        } else if (pm.currentNode != null) {
            target = pm.currentNode;
        } else if (currentNode != pm.previousNode) {
            target = pm.previousNode;
        }

        path = navigation.GetShortestPath(previousNode, target);

        target = null;
        if (path != null && path.Count > 1) {
            target = path[1];
        }

        return target;
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

}
