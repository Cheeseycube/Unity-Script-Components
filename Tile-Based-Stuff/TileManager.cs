using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    
    // This is a map of ordered pairs and their corresponding tile gameObjects
    public static Dictionary<Vector2Int, GameObject> WalkableTilePositions = new Dictionary<Vector2Int, GameObject>();
    
    private GameObject[] all_Walkable_Tiles;

    public BaseTile[] AllTiles;
    
    [SerializeField] private bool LockTilesToGrid = false;
    [SerializeField] public bool MayFindPath = false;
    
    // Start is called before the first frame update
    void Start()
    {
        // intitializing WalkableTilePositions
        all_Walkable_Tiles = GameObject.FindGameObjectsWithTag("Walkable");
        foreach (var tile in all_Walkable_Tiles)
        {
            Vector2Int orderedPair = new((int)tile.transform.position.x, (int)tile.transform.position.y);
            //print(orderedPair);
            // storing orderedPairs (offset by 0.5) and their corresponding gameObjects
            WalkableTilePositions.Add(orderedPair, tile);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // locks out of place tiles onto the grid when set in the editor
        if (LockTilesToGrid)
        {
            
            AllTiles = FindObjectsOfType<BaseTile>();
            foreach (BaseTile tile in AllTiles)
            {
                tile.transform.position = new Vector3((float)Math.Round(tile.transform.position.x, 0), (float)Math.Round(tile.transform.position.y, 0), 0);
            }
        }
    }
}
