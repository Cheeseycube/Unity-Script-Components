using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTile : MonoBehaviour
{
    private (int, int) location;
    private SpriteRenderer tileSprite;
    
    
    private void Start()
    {
        tileSprite = GetComponent<SpriteRenderer>();
        location = ((int)transform.position.x, (int)transform.position.y);
    }

    public void SetSprite(Sprite givenSprite)
    {
        tileSprite.sprite = givenSprite;
    }

    public (int, int) GetLocation()
    {
        return location;
    }
}
