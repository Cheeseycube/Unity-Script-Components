using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkableTile : BaseTile
{
    private SpriteRenderer tileSprite;
    // Start is called before the first frame update
    void Start()
    {
        tileSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseOver()
    {
        
    }

    private void OnMouseEnter()
    {
        TileManager tm = FindObjectOfType<TileManager>();
        PathFinder pf = FindObjectOfType<PathFinder>();
        PlayerPathFinder ppf = FindObjectOfType<PlayerPathFinder>();

        if (tm.MayFindPath)
        {
            tileSprite.color = Color.gray;
            List<Vector2Int> path = pf.Dijkstra(ppf.GetPlayerPos(), new Vector2Int((int)transform.position.x, (int)transform.position.y));

            //print("path destination: " + transform.position.x + " " + transform.position.y);
            foreach (var tilePosition in path)
            {
                TileManager.WalkableTilePositions[tilePosition].GetComponent<SpriteRenderer>().color = Color.gray;
            }
            //print("Path end");
        }
    }

    private void OnMouseExit()
    {
        tileSprite.color = Color.white;
        FindObjectOfType<PathFinder>().clearPath();
    }
}
