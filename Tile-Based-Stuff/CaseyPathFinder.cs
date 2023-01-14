using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    // this list of gameobjects contains all of the tiles: see Start() for initialization
    private GameObject[] allWalkableTiles;
    
    
    // This is a map of ordered pairs and their node counterparts that contains info such as distance from source
    private List<Node> nodeData = new();

    // list of visited nodes
    private List<Node> visited = new(); 
    
    // list of unvisited nodes--this should be a p-queue but instead I am going to sort it after each remove() call
    private List<Node> unVisitedNodes = new();
    
    
    // Dijkstra's algorithm
    public List<Vector2Int> Dijkstra(Vector2Int start, Vector2Int destination)
    {

        Node startNode = nodeData.Find((n) => n.point == start);

        // setting the distance from the startingPosition to itself to 0
        startNode.distance_from_source = 0;
        
        // adding the first node (start) to the queue
        unVisitedNodes.Add(startNode);
        
        // while there are still nodes to visit
        while (unVisitedNodes.Count != 0)
        {
            
            // removing minimum
            unVisitedNodes.Sort();
            Node currNode = unVisitedNodes[0];
            unVisitedNodes.RemoveAt(0);
            
            // updating visited
            visited.Add(currNode);
            
            // getting the adjacencies from the current node
            List<Node> adjList = currNode.adjTo;
            
            // for each adj of the current node
            foreach (Node adj in adjList.Except(visited))
            {
                
                //print("curr: " + currNode.point);
                    
                // finding the distance from current node to the given adjacency
                //int newDistance = getDistance(currNode.point, adj); //always 1 under current conditions

                // adding the current Node's distance to the start
                int newDistance = currNode.distance_from_source + 1;

                // if new distance is cheaper
                if (newDistance < adj.distance_from_source)
                {
                    adj.distance_from_source = newDistance;
                    adj.prev = currNode;
                    unVisitedNodes.Add(adj);
                }
                    
            }

            if (currNode.point == destination)
            {
                break;
            }
        }

        List<Vector2Int> path = new();
        path.Add(destination);
        
        Node pathNode = nodeData.Find(n=>n.point == destination).prev;
        if (pathNode == null)
        {
            print("path not found, highlighting destination");
        }
        while (pathNode != null)
        {
            path.Add(pathNode.point);
            pathNode = pathNode.prev;
        }

        return path;
    }
    
    // utility function called in Dijkstra's
    private int getDistance(Vector2Int point1, Vector2Int point2)
    {

        int xDist = Math.Abs(point1.x - point2.x);
        int yDist = Math.Abs(point1.y - point2.y);

        int totalDist = xDist + yDist;

        return totalDist;
    }

    // resets everything so a new path can be found
    public void clearPath()
    {
        
        foreach (var node in (from node in nodeData where node.distance_from_source != int.MaxValue || node.prev != null select node))
        {
            node.distance_from_source = Int32.MaxValue;
            node.prev = null;
        }

        foreach (var tile in allWalkableTiles)
        {
            tile.GetComponent<SpriteRenderer>().color = Color.white;
        }
        visited = new List<Node>();
        unVisitedNodes = new List<Node>();
        
    }
    
    // Internal Node class used in Dijkstra's
    public class Node : IComparable
    {
        public Vector2Int point;
        public int distance_from_source;
        public Node prev;
        public List<Node> adjTo = new();

        public Node(Vector2Int point, int distanceFromSource, Node prev, List<Node> adj)
        {
            this.point = point;
            this.distance_from_source = distanceFromSource;
            this.prev = prev;
            adjTo = adj;
        }
    
        int IComparable.CompareTo(object b)
        {
            return distance_from_source - ((Node)b).distance_from_source;
        }

        public bool PartialEquals(Node oth)
        {

            bool pointcheck = oth.point.Equals(point);
            bool distCheck = distance_from_source == oth.distance_from_source;
            //bool prevCheck = oth.prev.Equals(this);   //likely infinitely recursive

            return pointcheck && distCheck;

        }

    }
    
    
    // Initial setup is performed in the Start() method
    void Start()
    {
        
        // initial setup: finding all the walkable tiles in the scene and adding them to the appropriate data structures
        allWalkableTiles = GameObject.FindGameObjectsWithTag("Walkable");
        foreach (GameObject tile in allWalkableTiles)
        {
            // storing orderedPairs and their corresponding distances (infinity for now) in the map of nodes
            Vector2Int orderedPair = new((int)tile.transform.position.x, (int)tile.transform.position.y);
            //print(orderedPair);
            Node node = new(orderedPair, Int32.MaxValue, null, new());
            nodeData.Add(node);
            //print(orderedPair);
            //tile.GetComponent<SpriteRenderer>().color = Color.yellow;
            
            
        }
        
        // storing the adjacencies in the graph
        foreach (var node in nodeData)
        {

            foreach (var dir in new List<Vector2Int>() { Vector2Int.down, Vector2Int.left, Vector2Int.right, Vector2Int.up})
            {
                Node directionNode = new(node.point + dir, int.MaxValue, null, new());
                if (nodeData.Find((oth)=>directionNode.PartialEquals(oth)) != null) {
                    node.adjTo.Add(directionNode);
                }
            }
            
        }
        
        // printing out adjacencies for testing purposes
        /*foreach (var keyValuePair in graph)
        {
            print(keyValuePair.Key + " is adjacent to: " + string.Join(", ", keyValuePair.Value));
        }*/


    }

}
