using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour
{
    '''Required Components'''
    // Animator
    // Canvas
    // Trying to make this more readable... will update later

    private Canvas CheckpointCanvas;
    private bool CanDisplayMessage = true;
    private Animator myAnim;

    [SerializeField] private AudioClip checkpointSound;
    [SerializeField] private float Volume = 2f;
    [SerializeField] private float MessageDisplayTime = 1f;

    // Start is called before the first frame update
    void Start() // check curr level;
    {
        myAnim = GetComponent<Animator>();
        myAnim.SetBool("opening", false);
        CanDisplayMessage = true;
        CheckpointCanvas = GetComponent<Canvas>();
        CheckpointCanvas.enabled = false;
        // find object of type checkpoints 
    }


    private void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && CanDisplayMessage)
        {
            myAnim.SetBool("opening", true);
            CanDisplayMessage = false;
            StartCoroutine(SetCanvas());
            FindObjectOfType<GameManager>().SetPlayerSpawnPoint(this.gameObject.transform.position);
            AudioSource.PlayClipAtPoint(checkpointSound, Camera.main.transform.position, Volume);
        }
    }

    IEnumerator SetCanvas()
    {
        CheckpointCanvas.enabled = true;
        yield return new WaitForSeconds(MessageDisplayTime);
        CheckpointCanvas.enabled = false;
        CanDisplayMessage = true;
        myAnim.SetBool("opening", false);
    }

}
