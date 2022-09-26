using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour
{
    '''Required Components'''
    // Animator: By default the animation will not play.  When the player enters the collider the animation plays once.
    // Canvas: By default is disabled.  When the player enters the collider the checkpoint canvas is enabled, displaying the desired message
    // AudioClip: This serialized field contains the audio to be played when the player enters the collider
    // Could refactor to use crossFade instead using code like this: myAnim.CrossFade("animation name", 0, 0);

    '''Necessary additional scripts for this to function'''
    // GameManager: In order to set the spawn point, it needs access to a SetPlayerSpawnPoint() method in GameManager.cs  

    private Canvas CheckpointCanvas;                                  // Canvas to display desired message
    private bool CanDisplayMessage = true;
    private Animator myAnim;

    [SerializeField] private AudioClip checkpointSound;               // Sound played on checkpoint enter
    [SerializeField] private float Volume = 2f;                       // Sound volume
    [SerializeField] private float MessageDisplayTime = 1f;           // How long the canvas will be displayed for in seconds
     
    // Start is called before the first frame update
    void Start() // check curr level;
    {
        myAnim = GetComponent<Animator>();
        myAnim.SetBool("play animation", false);
        CanDisplayMessage = true;
        CheckpointCanvas = GetComponent<Canvas>();
        CheckpointCanvas.enabled = false;
    }


    private void Update()
    {
        
    }

    // When the player is detected the animation gets played, the sound gets played, the message is displayed, and the player's spawn point is changed
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && CanDisplayMessage)
        {
            myAnim.SetBool("play animation", true);
            CanDisplayMessage = false;
            StartCoroutine(SetCanvas());
            FindObjectOfType<GameManager>().SetPlayerSpawnPoint(this.gameObject.transform.position);  // References the GameManager script and attempts to set a new spawn location
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
