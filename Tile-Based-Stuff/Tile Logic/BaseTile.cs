using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTile : MonoBehaviour
{
    private Vector2Int location;
    private SpriteRenderer tileSprite;
    
    
    private void Start()
    {
        tileSprite = GetComponent<SpriteRenderer>();
        location = new Vector2Int((int)transform.position.x, (int)transform.position.y);
    }

    public void SetSprite(Sprite givenSprite)
    {
        tileSprite.sprite = givenSprite;
    }

    public Vector2Int GetLocation()
    {
        return location;
    }
}
