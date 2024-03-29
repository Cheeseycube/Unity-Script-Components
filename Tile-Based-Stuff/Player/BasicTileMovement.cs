using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTileMovement : MonoBehaviour
{
    
    private bool isMoving = false;
    private Vector3 origPos;
    private Vector3 newPos;
    private float timeToMove = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W) && !isMoving)
        {
            StartCoroutine(MovePlayer(Vector3.up));
        }
        if (Input.GetKey(KeyCode.A) && !isMoving)
        {
            StartCoroutine(MovePlayer(Vector3.left));
        }
        if (Input.GetKey(KeyCode.S) && !isMoving)
        {
            StartCoroutine(MovePlayer(Vector3.down));
        }
        if (Input.GetKey(KeyCode.D) && !isMoving)
        {
            StartCoroutine(MovePlayer(Vector3.right));
        }
    }
    
    public IEnumerator MovePlayer(Vector3 direction)
    {
        //playerAudio.PlayOneShot(moveSound,moveSoundVolume);
        // decreases player time by 1 second for each tile moved
        isMoving = true;
        float elapsedTime = 0;
        origPos = transform.position;
        newPos = origPos + (direction);

        while(elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(origPos, newPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        //AudioSource.PlayClipAtPoint(moveSound, transform.position, moveSoundVolume);
        // if we don't land exactly in the middle of a tile, this will make sure you end up there
        transform.position = newPos;
        isMoving = false;
    }
}
