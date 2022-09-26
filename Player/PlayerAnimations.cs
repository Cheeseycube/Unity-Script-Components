using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This script controls which animations are active for the player at any given time
// For overrides see the override version of this script

public class PlayerAnimations : MonoBehaviour
{
    // This section of code handles the logic of setting previous animation states to false
    // when changing animations.  Any new animations need to be added to the various enums and lists
    // for more specifics, see individual comments--otherwise this section never needs to be changed

    //Currently playing animation things. Register all new in enum!
    private enum AnimationStates { IdleSide, WalkingSide, WalkingNorth, WalkingSouth, BasicAttackSide, BasicAttackSouth, BasicAttackNorth, DashSide, DashVertical }


    private void updateAnimationState(AnimationStates newvalue)
    {
        myAnim.CrossFade(StateMapping[newvalue], 0, 0);  // crossfade implementation
    }


    //state booleans. This assumes that each transition is simply "other" == true. All new states should be registered here.
    private DictWithKeySet<AnimationStates, string> StateMapping = new DictWithKeySet<AnimationStates, string>
    {
        [AnimationStates.IdleSide] = "(Fist) Idle",   
        [AnimationStates.WalkingSide] = "(Fist) Side Walk",
        [AnimationStates.WalkingNorth] = "(Fist) North Walk",
        [AnimationStates.WalkingSouth] = "(Fist) South Walk",
        [AnimationStates.BasicAttackSide] = "(Fist) Side Attack",
        [AnimationStates.BasicAttackSouth] = "(Fist) South Attack",
        [AnimationStates.BasicAttackNorth] = "(Fist) North Attack",
        [AnimationStates.DashSide] = "Side Dash",
        [AnimationStates.DashVertical] = "Vertical Dash"
    };



    //actions that should not interrupt each other should have the same priority, overrides higher, ignores lower.
    //all new animations must be registered here.
    static private DictWithKeySet<AnimationStates, int> priorityMapping = new DictWithKeySet<AnimationStates, int>
    {
        [AnimationStates.IdleSide] = 0,
        [AnimationStates.WalkingSide] = 1,
        [AnimationStates.WalkingNorth] = 2,  // make these all unique just in case
        [AnimationStates.WalkingSouth] = 3,
        [AnimationStates.BasicAttackSide] = 4,
        [AnimationStates.BasicAttackSouth] = 5,
        [AnimationStates.BasicAttackNorth] = 6,
        [AnimationStates.DashSide] = 7,
        [AnimationStates.DashVertical] = 8
    };

    Rigidbody2D rb;

    private List<AnimationStates> priority = new List<AnimationStates>(new AnimationStates[] { AnimationStates.IdleSide });


    private Comparer<AnimationStates> sorter;


    Animator myAnim;
    
    // Start is called before the first frame updates
    void Start()
    {

        myAnim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sorter = Comparer<AnimationStates>.Create((a, b) => priorityMapping[b] - priorityMapping[a]);

    }


    // Update is called once per frame   
    void Update()
    {
        
        // this is where the animation states get set
        AnimationStates newState;

        if ((rb.velocity.y > 0) && rb.velocity.x == 0)
        {
            if (!priority.Contains(AnimationStates.WalkingNorth)) // walking north
            {
                //print("north");
                priority.Add(AnimationStates.WalkingNorth);
            }
        }
        else
        {
            priority.Remove(AnimationStates.WalkingNorth);
        }

        if ((rb.velocity.y < 0) && rb.velocity.x == 0)
        {
            if (!priority.Contains(AnimationStates.WalkingSouth)) // walking south
            {
                //print("south");
                priority.Add(AnimationStates.WalkingSouth);
            }
        }
        else
        {
            priority.Remove(AnimationStates.WalkingSouth);
        }

        if (Mathf.Abs(rb.velocity.x) > 0)
        {
            if (!priority.Contains(AnimationStates.WalkingSide)) // walking side
            {
                priority.Add(AnimationStates.WalkingSide);
            }
        }
        else
        {
            priority.Remove(AnimationStates.WalkingSide);
        }


        if (rb.velocity == Vector2.zero)
        {
            if (!priority.Contains(AnimationStates.IdleSide)) // idling: using side facing as default
            {
                //print("side");
                priority.Add(AnimationStates.IdleSide);
            }
        }
        else
        {
            priority.Remove(AnimationStates.IdleSide);
        }


        // this is necessary stuff I don't touch
        priority.Sort(sorter);

        updateAnimationState(priority[0]);

    }

    // Sets the animation state to a generic side attack
    public void SetBasicSideAttackAnimationState(bool isAttacking)
    {
        if (isAttacking)
        {
            priority.Add(AnimationStates.BasicAttackSide);
        }
        else
        {
            priority.Remove(AnimationStates.BasicAttackSide);
        }
    }

    // Sets the animation state to a generic south attack
    public void SetBasicSouthAttackAnimationState(bool isAttacking)
    {
        if (isAttacking)
        {
            priority.Add(AnimationStates.BasicAttackSouth);
        }
        else
        {
            priority.Remove(AnimationStates.BasicAttackSouth);
        }
    }

    // Sets the animation state to a generic north attack
    public void SetBasicNorthAttackAnimationState(bool isAttacking)
    {
        if (isAttacking)
        {
            priority.Add(AnimationStates.BasicAttackNorth);
        }
        else
        {
            priority.Remove(AnimationStates.BasicAttackNorth);
        }
    }

    // Sets the animation state to the side dashing animation
    public void SetSideDashAnimationState(bool isDashing)
    {
        if (isDashing)
        {
            priority.Add(AnimationStates.DashSide);
        }
        else
        {
            priority.Remove(AnimationStates.DashSide);
        }
    }

    // Sets the animation state to the vertical dashing animation
    public void SetVerticalDashAnimationState(bool isDashing)
    {
        if (isDashing)
        {
            priority.Add(AnimationStates.DashVertical);
        }
        else
        {
            priority.Remove(AnimationStates.DashVertical);
        }
    }


}

