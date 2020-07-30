using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node[] neighbours;
    public Vector2[] validDirections;

    // Start is called before the first frame update
    void Start()
    {
        validDirections = new Vector2[neighbours.Length];

        for (int i = 0; i < neighbours.Length; i++) {

            Vector2 neighbourDirection = neighbours[i].transform.localPosition - transform.localPosition;
            validDirections[i] = neighbourDirection.normalized;
        }
    }
}
