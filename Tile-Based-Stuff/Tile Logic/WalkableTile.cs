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
        if (FindObjectOfType<TileManager>().MayFindPath)
        {
            tileSprite.color = Color.gray;
            List<(int, int)> path = new List<(int, int)>();
            path =  FindObjectOfType<PathFinder>().Dijkstra(FindObjectOfType<PlayerPathFinder>().GetPlayerPos(),
                ((int)transform.position.x, (int)transform.position.y));

            foreach (var tilePosition in path)
            {
                TileManager.WalkableTilePositions[tilePosition].GetComponent<SpriteRenderer>().color = Color.gray;
            }
        }
    }

    private void OnMouseExit()
    {
        tileSprite.color = Color.white;
        FindObjectOfType<PathFinder>().clearPath();
    }
}
