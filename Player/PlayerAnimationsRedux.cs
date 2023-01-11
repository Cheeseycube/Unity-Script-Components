using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAnimationsRedux : MonoBehaviour
{
    void Start() {//find comps. if static rb is buggy, move priority list implementation here.
        myAnim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    Animator myAnim;
    static Rigidbody2D rb;


    //all registry here. reverse priority, so last is lowest priority, first is highest. AnimInfo impl at bottom of file.
    private static List<AnimInfo> priorityList = new()
    {
        //previously, some animations were set directly into the priority list with function calls.
        //this is no longer possible.
        //instead you can add some kind of global bool here and set that, ex 1:
        new AnimInfo("(Fist) Side Attack", ()=>attackSideBool, 0, false),

        //or the bool somewhere else and a function call here. NO FINDOBJECT(). ex 2:
        new AnimInfo("(Fist) North Attack", ()=>needNorthAttack(), .3, true),

        //or grab the keystroke, noting that the animation may not play due to queueing or other stuff. again, do not findobject! ex 3:

        new AnimInfo("Side Dash", ()=> { /*notify first! or see below for options on further notifications*/ return Input.GetKey(KeyCode.Space); }, .5, false),

        new AnimInfo("(Fist) North Walk", ()=>((rb.velocity.y > 0) && rb.velocity.x == 0), 0, false),
        new AnimInfo("(Fist) Idle", () => (true), 0, false)//high recommend last's entry to be true for safeties sake.
    };

    public static bool attackSideBool = false;

    public static bool needNorthAttack() {
        return Input.GetKey(KeyCode.Mouse0);
    }


    private AnimInfo currentState = priorityList.Last();      //if necessary for stuff like queueing actions to match animations, i can update this to add an event notifier  easily.

    private AnimInfo queued = null;
    private bool lockout;   //use if an animation MUST finish

    // Update is called once per frame
    void Update()
    {
        if (!lockout)   //nothing currently important enough to let ride
        {
            AnimInfo bestState = priorityList.Find((anim) => anim.entry.Invoke());    //first entry in the list where the entry condition returns true 

            if (!currentState.name.Equals(bestState.name))      //if it is not currently being played
            {
                updateAnimation(bestState, false);
            }
        }
        else {      //something is currently playing that must continue. we will queue ONE addtional animation to play afterwards.
            List<AnimInfo> queueableSubset = priorityList.Where((anim) => anim.queue == true).ToList();

            if (!(queueableSubset.Count == 0))//make sure that there is stuff to iterate through
            {
                

                AnimInfo bestState = queueableSubset.Find((anim) => anim.entry.Invoke());

                if (bestState != null && !currentState.name.Equals(bestState.name))
                {
                    queued = bestState;                         //goes into queue instead of direct update
                }
            }
        }

    }

    private void updateAnimation(AnimInfo newvalue, bool fromQueue)
    {
        myAnim.CrossFade(newvalue.name, 0, 0);  // crossfade implementation
        
        currentState = newvalue;
        
        if (fromQueue) {//erase queued value if i come from there
            queued = null;
        }

        StartCoroutine(swapLockout(newvalue.lockout));
    }

    IEnumerator swapLockout(double seconds) {       //lock out swapping for input seconds, then switch to queued anim if existing
        lockout = true;
        yield return new WaitForSeconds((float)seconds);
        lockout = false;
        if (queued != null) {
            updateAnimation(queued, true);
        }
    }

}


class AnimInfo
{

    public string name;         //the animations name in the animator panel
    public Func<bool> entry;    //the anims entry condition, a function that returns a bool with no inputs
    public double lockout;      //the number of seconds to lock out animation swapping for on entry. for walk cycles, probably zero.
                                //otherwise the duration of the animation +/- a few millis, or zero if you want everything to override immediately
    public bool queue;          //if the animation is allowed to queue up behind an actively played one to play immediately
                                //default should be false, use for high priority actions only

    public AnimInfo(string nameInAnimator, Func<bool> entryCond, double lockoutTime, bool canQueue) {
        entry = entryCond;
        name = nameInAnimator;
        lockout = lockoutTime;
        queue = canQueue;
    }

}
