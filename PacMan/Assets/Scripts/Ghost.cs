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
    public float speed = 6.6f;

    /// <summary>
    /// The Node at which the ghost starts when the level begins, determining their position.
    /// </summary>
    /// <value>
    /// Possibly the default Node for the level, or a saved value if loading a previous game.
    /// </value>
    public Node startingNode;

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

    private List<Node> path;
    // Objects for keeping track of how the ghost navigates the maze.
    private Node currentNode, targetNode, previousNode;
    private Vector2 direction, nextDirection;
    // A reference to the player
    private GameObject pacMan;
    // A reference to the level
    private GameBoard board;


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
        
        if (startingNode == null) {
            Node node = board.GetNodeAtPosition((Vector2)transform.localPosition);
            if (node != null) {
                currentNode = node;
            }
        }
        previousNode = currentNode;
        
        direction = Vector2.right;
        targetNode = ChooseNextNode();
        /*targetNode = board.GetNodeAtPosition((Vector2)pacMan.transform.position);
        if (targetNode == null && pacMan.GetComponent<PacMan> ().currentNode != null) {
            targetNode = pacMan.GetComponent<PacMan> ().currentNode;
        }*/
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
        if ((pacMan.transform.localPosition - transform.position).sqrMagnitude < 0.1f) {
            PacMan pman = pacMan.GetComponent<PacMan> ();
            pman.lives--;
            
            Debug.Log("Lives: " + pman.lives);

            if (pman.lives <= 0) {
                Debug.Log("Lost Game!");
            }
        }
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
                currentNode = targetNode;

                transform.localPosition = currentNode.transform.position;

                Node otherPortal = board.GetPortal((Vector2)currentNode.transform.position);
                if (otherPortal != null) {
                    transform.localPosition = otherPortal.transform.position;
                    currentNode = otherPortal;
                }

                targetNode = ChooseNextNode();
                previousNode = currentNode;
                currentNode = null;
            } else {
                transform.localPosition += (Vector3)(direction * speed * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Determine which node should next be targeted as the ghost's destination.
    /// </summary>
    /// <return>
    /// Either a target node which the ghost can travel towards, or null.
    /// </return>
    /// <pre>
    /// A reference to PacMan has been found in the board space.
    /// </pre>
    /// <pre>
    /// The current node is connected to neighbouring nodes, as to allow directions for the ghost to travel.
    /// </pre>
    protected Node ChooseNextNode()
    {
        Vector2 targetTile = Vector2.zero;

        Vector2 targetPosition = new Vector2(Mathf.RoundToInt(pacMan.transform.position.x), Mathf.RoundToInt(pacMan.transform.position.y));
        Vector2 targetDirection = (targetPosition - (Vector2)transform.position).normalized;

        Node moveToNode = null;
        
        if (currentNode != null && currentNode.neighbours != null) {
            for (int i = 0; i < currentNode.validDirections.Length; i++) {
                        Debug.Log(currentNode.validDirections[i]);
                if ((currentNode.validDirections[i].x > 0 && targetDirection.x > 0) ||
                    (currentNode.validDirections[i].x < 0 && targetDirection.x < 0) ||
                    (currentNode.validDirections[i].y > 0 && targetDirection.y > 0) ||
                    (currentNode.validDirections[i].y < 0 && targetDirection.y < 0)) {

                        moveToNode = currentNode.neighbours[i];
                        direction = currentNode.validDirections[i];
                        break;
                }
            }
        }

        return moveToNode;
    }
}
