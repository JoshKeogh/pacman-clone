using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The Tile class provides some extra information about the game objects in the maze.
/// <summary>
public class Tile : MonoBehaviour
{
    public bool isPortal, isPellet, isSuperPellet, didConsume;

    /// <summary>
    /// If this Tile class is attached to a node which is a portal, this object holds the corresponding destination node.
    /// </summary>
    /// <value>
    /// Destination node for a portal node.
    /// </value>
    public GameObject portalReceiver;
}
