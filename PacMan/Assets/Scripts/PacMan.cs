using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The PacMan class is the character controller for PacMan; it handles player input which controls the character, and deals with related events such as eating consumables.
/// <summary>
public class PacMan : MonoBehaviour
{
    /// <summary>
    /// How quickly PacMan travels through the maze.
    /// </summary>
    /// <value>
    /// The speed of the player.
    /// </value>
    public float speed = 7.0f;

    public int lives = 3;

    /// <summary>
    /// The Node at which the player starts when the level begins, determining their position.
    /// </summary>
    /// <value>
    /// Possibly the default Node for the level, or a saved value if loading a previous game.
    /// </value>
    public Node currentNode;

    /// <summary>
    /// The sprite which plays when PacMan is idle and not moving.
    /// </summary>
    public Sprite idleSprite;

    // Objects for keeping track of how the player navigates the maze.
    private Vector2 nextDirection, direction = Vector2.zero;
    protected Node previousNode, targetNode;
    // A reference to the level
    private GameBoard board;


    /// <summary>
    /// The Start() function is called before the first frame update and is used as a constructor to set the currentNode.
    /// </summary>
    /// <pre>
    /// The system has loaded PacMan into a level which represents the maze as a node-based graph.
    /// </pre>
    void Start()
    {
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<GameBoard> ();

        if (currentNode == null) {
            Node node = board.GetNodeAtPosition((Vector2)transform.localPosition);

            if (node != null) {
                currentNode = node;
            }
        }
        previousNode = currentNode;
    }

    /// <summary>
    /// The Update() function is called once per frame. It will check whether the player has provided input, then move the character and update its graphics, then handle collisions with consumables.
    /// </summary>
    /// <pre>
    /// PacMan is loaded into a valid maze.
    /// </pre>
    /// <post>
    /// PacMan was not updated to an invalid state, for example is still confined to the maze with proper animation.
    /// </post>
    void Update()
    {
        CheckInput();
        Move();
        UpdateOrientation();
        UpdateAnimationState();
        ConsumePellet();
    }

    /// <summary>
    /// This function handle's input from the user by requesting the system performs the corresponding change.
    /// </summary>
    /// <pre>
    /// The user has a properly configured input device.
    /// </pre>
    /// <post>
    /// The direction object is left unchanged, nextDirection may have been updated.
    /// </post>
    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {

            ChangeDirection(Vector2.left);

        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {

            ChangeDirection(Vector2.right);

        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {

            ChangeDirection(Vector2.up);

        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {

            ChangeDirection(Vector2.down);
        }
    }

    /// <summary>
    /// Update the direction in which the player has indicated they would like to travel once reaching the next node.
    /// </summary>
    /// <param>
    /// The direction in which the user has indicated they would like to change to.
    /// </param>
    /// <pre>
    /// PacMan is in a maze which is represented as a graph with nodes.
    /// </pre>
    /// <post>
    /// The direction or nextDirection objects may have been updated, only to a valid direction.
    /// </post>
    void ChangeDirection(Vector2 dir)
    {
        if (dir != direction) {
            nextDirection = dir;
        }

        if (currentNode != null)
        {
            Node moveToNode = CanMove(dir);

            if (moveToNode != null) {

                direction = dir;
                targetNode = moveToNode;
                previousNode = currentNode;
                currentNode = null;
            }
        }
    }

    /// <summary>
    /// PacMan's location within the maze is updated based on his direction. The direction of travel may be updated to nextDirection, or PacMan may be teleported by entering a portal.
    /// </summary>
    /// <pre>
    /// PacMan is in a maze which is represented as a graph with nodes.
    /// </pre>
    /// <post>
    /// PacMan was not updated to an invalid location, and is still confined to the maze.
    /// </post>
    /// <post>
    /// PacMan's direction may have been updated, but only to a valid direction.
    /// </post>
    void Move()
    {
        if (targetNode != currentNode && targetNode != null) {

            if (nextDirection == -direction) {
                direction *= -1;

                Node tempNode = targetNode;
                targetNode = previousNode;
                previousNode = tempNode;
            }

            if (board.OverShotTarget(transform.localPosition, targetNode, previousNode)) {
                currentNode = targetNode;
                transform.localPosition = currentNode.transform.position;

                Node otherPortal = board.GetPortal(currentNode.transform.position);
                if (otherPortal != null) {
                    transform.localPosition = otherPortal.transform.position;
                    currentNode = otherPortal;
                }

                Node moveToNode = CanMove(nextDirection);

                if (moveToNode == null) {
                    moveToNode = CanMove (direction);
                } else {
                    direction = nextDirection;
                }

                if (moveToNode != null) {

                    targetNode = moveToNode;
                    previousNode = currentNode;
                    currentNode = null;

                } else {
                    direction = Vector2.zero;
                }
            } else {
                transform.localPosition += (Vector3)(direction * speed * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Checks whether the currentNode has a neighbour which allows PacMan's direction to be updated to head towards that neighbouring node.
    /// </summary>
    /// <param>
    /// The direction of travel along which to check the currentNode has a neighbour in that direction.
    /// </param>
    /// <return>
    /// The neighbouring node which the character will head towards if they want to travel in the selected direction, or null if none was found.
    /// </return>
    /// <pre>
    /// The maze has been represented as a graph and the neighbouring nodes properly connected.
    /// </pre>
    /// <pre>
    /// The current node should not be null.
    /// </pre>
    Node CanMove(Vector2 dir)
    {
        Node moveToNode = null;

        for (int i = 0; i < currentNode.neighbours.Length; i++) {
            if (currentNode.validDirections[i] == dir) {
                moveToNode = currentNode.neighbours[i];
                break;
            }
        }
        
        return moveToNode;
    }

    /// <summary>
    /// Updates the rotation of PacMan so that the sprite to correspond to the direction of travel.
    /// </summary>
    /// <pre>
    /// The direction object points in a cardinal direction.
    /// </pre>
    /// <post>
    /// PacMan's sprite has been oriented to a valid direction without skewing it.
    /// </post>
    void UpdateOrientation()
    {
        if (direction == Vector2.left) {

            transform.localScale = new Vector3(-1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 0);

        } else if (direction == Vector2.right) {

            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 0);

        } else if (direction == Vector2.up) {

            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 90);

        } else if (direction == Vector2.down) {

            transform.localScale = new Vector3(1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 270);
        }
    }

    /// <summary>
    /// When PacMan stops moving this function changes his sprite to its idle animation, and changes it back again once he starts moving.
    /// </summary>
    /// <pre>
    /// PacMan has an idle sprite in addition to his default animator.
    /// </pre>
    /// <post>
    /// PacMan has had a valid animator enabled.
    /// </post>
    void UpdateAnimationState()
    {
        if (direction == Vector2.zero) {

            GetComponent<Animator> ().enabled = false;
            GetComponent<SpriteRenderer> ().sprite = idleSprite;

        } else {
            GetComponent<Animator> ().enabled = true;
        }
    }

    /// <summary>
    /// Determine whether a consumable is located at PacMan's current position, and consume it.
    /// </summary>
    /// <pre>
    /// The GameBoard has successfully initialized.
    /// </pre>
    /// <post>
    /// A consumed pellet is removed from the game board so it is not able to be eaten again.
    /// </post>
    void ConsumePellet()
    {
        board.ConsumeAtPosition(transform.position);

        if (board.dotsRemaining <= 0) {
            Debug.Log("Game Won!");
        }
    }
}
