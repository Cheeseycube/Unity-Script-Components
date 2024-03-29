using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    // this list of gameobjects contains all of the tiles: see Start() for initialization
    private GameObject[] allWalkableTiles;
    
    // This represents the underlying graph structure that Dijsktra's is based off of
    private Dictionary<Vector2Int, List<Vector2Int>> graph = new();
    
    // This is a map of ordered pairs and their node counterparts that contains info such as distance from source
    private Dictionary<Vector2Int, Node> nodeData = new();

    // list of visited nodes
    private List<Node> visited = new List<Node>(); 
    
    // list of unvisited nodes--this should be a p-queue but instead I am going to sort it after each remove() call
    private List<Node> unVisitedNodes = new List<Node>();
    
    
    
    // Dijkstra's algorithm
    public List<Vector2Int> Dijkstra(Vector2Int start, Vector2Int destination)
    {
        // setting the distance from the startingPosition to itself to 0
        nodeData[start] = new Node(start, 0, null);
        
        // adding the first node (start) to the queue
        unVisitedNodes.Add(nodeData[start]);
        
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
            List<Vector2Int> adjList = graph[currNode.point];
            
            // for each adj of the current node
            foreach (Vector2Int adj in adjList)
            {
                // if current node has not been processesed
                if (!visited.Contains(nodeData[adj]))
                {
                     //print("curr: " + currNode.point);
                    
                    // finding the distance from current node to the given adjacency
                    //newDistance = getDistance(currNode.point, adj);
                    int newDistance = 1;

                    // adding the current Node's distance to the start
                    Node prevNode = nodeData[currNode.point];
                    newDistance += prevNode.distance_from_source;

                    // if new distance is cheaper
                    if (newDistance < nodeData[adj].distance_from_source)
                    {
                        nodeData[adj].distance_from_source = newDistance;
                        nodeData[adj].prev = currNode;
                        unVisitedNodes.Add(nodeData[adj]);
                    }
                    
                }
            }

            if (currNode.point == destination)
            {
                break;
            }
        }

        List<Vector2Int> path = new();
        path.Add(destination);
        
        Node pathNode = nodeData[destination].prev;
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
    

    // resets everything so a new path can be found
    public void clearPath()
    {
        
        foreach (var node in nodeData)
        {
            node.Value.distance_from_source = Int32.MaxValue;
            node.Value.prev = null;
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

        public Node(Vector2Int point, int distanceFromSource, Node prev)
        {
            this.point = point;
            this.distance_from_source = distanceFromSource;
            this.prev = prev;
        }
    
        int IComparable.CompareTo(object b)
        {
            return this.distance_from_source - ((Node)b).distance_from_source;

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
            nodeData.Add(orderedPair, new Node(orderedPair, Int32.MaxValue, null));
            //print(orderedPair);
            //tile.GetComponent<SpriteRenderer>().color = Color.yellow;
            
            // storing the ordered pairs and their adjacencies (empty for now) in the graph
            graph.Add(orderedPair, new List<Vector2Int>());
            
        }
        
        // storing the adjacencies in the graph
        foreach (var keyValuePair in graph)
        {
            Vector2Int point = keyValuePair.Key;
            keyValuePair.Value.Add(point);
            if (graph.ContainsKey(new(point.x + 1, point.y)))
            {
                keyValuePair.Value.Add(new(point.x + 1, point.y));
            }
            if (graph.ContainsKey(new(point.x - 1, point.y)))
            {
                keyValuePair.Value.Add(new(point.x - 1, point.y));
            }
            if (graph.ContainsKey(new(point.x, point.y + 1)))
            {
                keyValuePair.Value.Add(new(point.x, point.y + 1));
            }
            if (graph.ContainsKey(new(point.x, point.y - 1)))
            {
                keyValuePair.Value.Add(new(point.x, point.y - 1));
            }
            
        }
        
        // printing out adjacencies for testing purposes
        /*foreach (var keyValuePair in graph)
        {
            print(keyValuePair.Key + " is adjacent to: " + string.Join(", ", keyValuePair.Value));
        }*/


    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
