using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The node class enables creating a graph out of mazes by representing intersections of the hallways as vertices which connect edges.
/// </summary>
public class Node : MonoBehaviour
{
    /// <summary>
    /// An array of the nodes in the graph which neighbour this node by directly connecting via an edge.
    /// </summary>
    /// <value>
    /// An array of this node's connected vertices.
    /// </value>
    public Node[] neighbours;

    /// <summary>
    /// An array of the possible valid directions along which characters can travel from this node. The directions point along an edge and thus prescribe how one moves through a hallway.
    /// </summary>
    /// <value>
    /// An array of normalized vectors which constrain character movement.
    /// </value>
    public Vector2[] validDirections;

    /// <summary>
    /// Indicates whether this node is a portal.
    /// </summary>
    /// <value>
    /// Destination node for a portal node.
    /// </value>
    public bool isPortal;

    /// <summary>
    /// If this node is a portal, this object holds the corresponding destination node.
    /// </summary>
    /// <value>
    /// Destination node for a portal node.
    /// </value>
    public Node portalReceiver;


    /// <summary>
    /// The Start() function is called before the first frame update.
    /// The function considers the neighbours Node array and populates the validDirections, so determines how the maze can be traversed.
    /// </summary>
    /// <pre>
    /// Node objects have been placed at each intersection of the maze and the neighbours array has been populated with nodes which are directly adjacent along a hallway.
    /// </pre>
    /// <post>
    /// The validDirections array has been populated with normalized vectors which correspond to the directions determined by the neighbours array.
    /// </post>
    void Start()
    {
        validDirections = new Vector2[neighbours.Length];

        for (int i = 0; i < neighbours.Length; i++) {

            Vector2 neighbourDirection = neighbours[i].transform.localPosition - transform.localPosition;
            validDirections[i] = neighbourDirection.normalized;
        }
    }

    // Make each node visible in the Unity editor.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.3f);

        /*
        Gizmos.color = Color.red;
        for (int i = 0; i < validDirections.Length; i++) {
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)validDirections[i]);
        }
        */
    }
}
