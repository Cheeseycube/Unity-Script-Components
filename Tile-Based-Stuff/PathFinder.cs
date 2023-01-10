using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    // this list of gameobjects contains all of the tiles: see Start() for initialization
    private GameObject[] allTiles;
    
    // This represents the underlying graph structure that Dijsktra's is based off of
    private Dictionary<(int, int), List<(int, int)>> graph = new Dictionary<(int, int), List<(int, int)>>();
    
    // This is a map of ordered pairs and their node counterparts that contains info such as distance from source
    private Dictionary<(int, int), Node> nodeData = new Dictionary<(int, int), Node>();

    // list of visited nodes
    private List<Node> visited = new List<Node>(); 
    
    // list of unvisited nodes--this should be a p-queue but instead I am going to sort it after each remove() call
    private List<Node> unVisitedNodes = new List<Node>();

    // number of nodes in the graph
    private int numNodes = 0;
    
    
    // Dijkstra's algorithm
    public List<(int, int)> Dijkstra((int, int) start, (int, int) destination)
    {
        // setting the distance from the startingPosition to itself to 0
        nodeData[start] = new Node(start, 0, null);
        
        // adding the first node (start) to the queue
        unVisitedNodes.Add(nodeData[start]);
        
        // while there are still nodes to visit
        while (unVisitedNodes.Count != 0)
        {
            // for use later
            int newDistance = -1;
            
            // removing minimum
            unVisitedNodes.Sort();
            Node currNode = unVisitedNodes[0];
            unVisitedNodes.RemoveAt(0);
            
            // skip if already visited
            if (visited.Contains(currNode))
            {
                continue;
            }
            
            // updating visited
            visited.Add(currNode);
            
            // getting the adjacencies from the current node
            List<(int, int)> adjList = graph[currNode.point];
            
            // for each adj of the current node
            foreach ((int, int) adj in adjList)
            {
                // if current node has not been processesed
                if (!visited.Contains(nodeData[adj]))
                {
                     //print("curr: " + currNode.point);
                    
                    // finding the distance from current node to the given adjacency
                    newDistance = getDistance(currNode.point, adj);

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

        List<(int, int)> path = new List<(int, int)>();
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
    
    // utility function called in Dijkstra's
    private int getDistance((int, int) point1, (int, int) point2)
    {
        int x1 = point1.Item1;
        int y1 = point1.Item2;
        int x2 = point2.Item1;
        int y2 = point2.Item2;

        int xDist = Math.Abs(x1 - x2);
        int yDist = Math.Abs(y1 - y2);

        int totalDist = xDist + yDist;

        return totalDist;
    }

    // resets everything so a new path can be found
    public void clearPath()
    {
        
        foreach (var node in nodeData)
        {
            node.Value.distance_from_source = Int32.MaxValue;
            node.Value.prev = null;
        }

        foreach (var tile in allTiles)
        {
            tile.GetComponent<SpriteRenderer>().color = Color.white;
        }
        visited = new List<Node>();
        unVisitedNodes = new List<Node>();
        
    }
    
    // Internal Node class used in Dijkstra's
    public class Node : IComparable
    {
        public (int, int) point;
        public int distance_from_source;
        public Node prev;

        public Node((int, int) point, int distanceFromSource, Node prev)
        {
            this.point = point;
            this.distance_from_source = distanceFromSource;
            this.prev = prev;
        }
    
        int IComparable.CompareTo(object b)
        {
            Node _b = (Node) b;
            if (this.distance_from_source < _b.distance_from_source)
            {
                return -1;
            }

            if (this.distance_from_source > _b.distance_from_source)
            {
                return 1;
            }

            return 0;
        }
    

    }
    
    
    // Initial setup is performed in the Start() method
    void Start()
    {
        
        // initial setup: finding all the walkable tiles in the scene and adding them to the appropriate data structures
        allTiles = GameObject.FindGameObjectsWithTag("Walkable");
        foreach (GameObject tile in allTiles)
        {
            numNodes++;
            
            // storing orderedPairs and their corresponding distances (infinity for now) in the map of nodes
            (int, int) orderedPair = ((int)tile.transform.position.x, (int)tile.transform.position.y);
            nodeData.Add(orderedPair, new Node(orderedPair, Int32.MaxValue, null));
            //print(orderedPair);
            //tile.GetComponent<SpriteRenderer>().color = Color.yellow;
            
            // storing the ordered pairs and their adjacencies (empty for now) in the graph
            graph.Add(orderedPair, new List<(int, int)>());
            
        }
        
        // storing the adjacencies in the graph
        foreach (var keyValuePair in graph)
        {
            (int, int) point = keyValuePair.Key;
            keyValuePair.Value.Add(point);
            if (graph.ContainsKey((point.Item1 + 1, point.Item2)))
            {
                keyValuePair.Value.Add((point.Item1 + 1, point.Item2));
            }
            if (graph.ContainsKey((point.Item1 - 1, point.Item2)))
            {
                keyValuePair.Value.Add((point.Item1 - 1, point.Item2));
            }
            if (graph.ContainsKey((point.Item1, point.Item2 + 1)))
            {
                keyValuePair.Value.Add((point.Item1, point.Item2 + 1));
            }
            if (graph.ContainsKey((point.Item1, point.Item2 - 1)))
            {
                keyValuePair.Value.Add((point.Item1, point.Item2 - 1));
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
