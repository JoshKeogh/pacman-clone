using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    protected static List<Node> graph = new List<Node> ();

    // Start is called before the first frame update
    void Start()
    {
        Object[] objects = GameObject.FindGameObjectsWithTag("Node");
        Node node = null;
        foreach (GameObject obj in objects) {
            node = obj.GetComponent<Node> ();
            if (node != null) {
                graph.Add(node);
            }
        }
    }

    public List<Node> GetShortestPath(Node source, Node target)
    {
        if (source == null || target == null) {
            return null;
        }

        List<Node> unvisited = new List<Node> ();
        Dictionary<Node, Node> previous = new Dictionary<Node, Node> ();
        Dictionary<Node, float> distances = new Dictionary<Node, float> ();
        
        Node current;
        for (int i = 0; i < graph.Count; i++) {
            current = graph[i];
            unvisited.Add(current);
            distances.Add(current, float.MaxValue);
        }
        distances[source] = 0;
        
        List<Node> shortestPath = new List<Node> ();
        while (unvisited.Count > 0) {
            unvisited = unvisited.OrderBy(node => distances[node]).ToList();

            current = unvisited[0];
            unvisited.Remove(current);

            if (current == target) {
                while (previous.ContainsKey(current)) {
                    shortestPath.Insert(0, current);
                    current = previous[current];
                }
                shortestPath.Insert(0, current);
                break;
            }

            foreach (Node neighbour in current.neighbours) {
                float distanceToNeighbour = Vector2.Distance(current.transform.position, neighbour.transform.position);

                float pathLengthToNeighbour = distances[current] + distanceToNeighbour;

                if (pathLengthToNeighbour < distances[neighbour]) {
                    distances[neighbour] = pathLengthToNeighbour;
                    previous[neighbour] = current;
                }
            }
        }

        return shortestPath;
    }
}
