using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPathFinder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public (int, int) GetPlayerPos() 
    {
        return ((int)transform.position.x, (int)transform.position.y);
    }
}
