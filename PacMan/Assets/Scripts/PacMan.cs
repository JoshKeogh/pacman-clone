using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : MonoBehaviour
{
    public float speed = 4.0f;
    public Node currentNode;

    public Sprite idleSprite;

    private Vector2 nextDirection, direction = Vector2.zero;
    private Node previousNode, targetNode;


    // Start is called before the first frame update
    void Start()
    {
        Node node = GetNodeAtPosition(transform.localPosition);

        if (node != null) {
            currentNode = node;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        Move();
        UpdateOrientation();
        UpdateAnimationState();
    }

    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {

            ChangePosition(Vector2.left);

        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {

            ChangePosition(Vector2.right);

        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {

            ChangePosition(Vector2.up);

        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {

            ChangePosition(Vector2.down);
            
        }
    }

    void ChangePosition(Vector2 dir)
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

    void Move()
    {
        if (targetNode != currentNode && targetNode != null) {
            if (OverShotTaget()) {
                currentNode = targetNode;
                transform.localPosition = currentNode.transform.position;

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

    void MoveToNode(Vector2 dir)
    {
        Node moveToNode = CanMove(dir);

        if (moveToNode != null) {
            transform.localPosition = moveToNode.transform.position;
            currentNode = moveToNode;
        }
    }

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

    void UpdateAnimationState()
    {
        if (direction == Vector2.zero) {

            GetComponent<Animator> ().enabled = false;
            GetComponent<SpriteRenderer> ().sprite = idleSprite;

        } else {
            GetComponent<Animator> ().enabled = true;
        }
    }

    Node CanMove(Vector2 dir)
    {
        Node moveToNode = null;
        if (currentNode == null) {
            Debug.Log("null currentNode");
        }

        for (int i = 0; i < currentNode.neighbours.Length; i++) {
            if (currentNode.validDirections[i] == dir) {
                moveToNode = currentNode.neighbours[i];
                break;
            }
        }

        return moveToNode;
    }

    Node GetNodeAtPosition(Vector2 pos)
    {
        GameObject tile = GameObject.Find("Game").GetComponent<GameBoard> ().board[(int)pos.x, (int)pos.y];
        if (tile != null) {
            return tile.GetComponent<Node> ();
        }

        return null;
    }

    bool OverShotTaget()
    {
        float nodeToTarget = LengthFromNode(targetNode.transform.position);
        float nodeToSelf = LengthFromNode(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }

    float LengthFromNode(Vector2 targetPosition)
    {
        Vector2 diff = targetPosition - (Vector2)previousNode.transform.position;
        return diff.sqrMagnitude;
    }
}
