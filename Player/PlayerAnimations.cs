using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This script controls which animations are active for the player at any given time
// Remove the override stuff at some point...

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
        [AnimationStates.WalkingNorth] = 2,  // made these all unique just in case
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
    // This allows us to replace the animation clips at runtime
    [SerializeField] private AnimatorOverrideController animationsOverrideSword;
    [SerializeField] private AnimatorOverrideController animationsOverrideFist;
    [SerializeField] private AnimatorOverrideController animationsOverrideIce;
    [SerializeField] private AnimatorOverrideController animationsOverrideFire;
    [SerializeField] private AnimatorOverrideController animationsOverrideLightning;
    [SerializeField] private AnimatorOverrideController animationsOverrideCrit;
    //[SerializeField] private AnimatorOverrideController animationsOverrideAxe;


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
        // only uncomment this below code for testing purposes

        /*if (Input.GetKeyDown(KeyCode.J)){
            SwordOverride();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            FistOverride();
        }*/


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

    // Sets the animation state to a generic side attack--the given override determines the specifics
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

    // Sets the animation state to a generic south attack--the given override determines the specifics
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

    // Sets the animation state to a generic north attack--the given override determines the specifics
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

    // Sets the animation state to the side dashing animation--this is the same for all overrides
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

    // Sets the animation state to the vertical dashing animation-- this is the same for all overrides
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

    // Sets the override to sword animations--referenced in the SwordPickup script
    // References GameManager to set the player type to sword
    // The playertype variable is itself referenced in AttackManager to determine
    // how much damage the player does on mouse1
    public void SwordOverride()
    {
        myAnim.runtimeAnimatorController = animationsOverrideSword;
        FindObjectOfType<GameManager>().SetPlayerType(0);
    }

    // Sets the override to fist animations--referenced in the GameManager script to initialize the player
    // with no weapons on death
    // References GameManager to set the player type to fist
    // see above for playerType explanation
    public void FistOverride()
    {
        myAnim.runtimeAnimatorController = animationsOverrideFist;
        FindObjectOfType<GameManager>().SetPlayerType(1);
    }

    // Sets the override to ice animations--referenced in the SwordPickup script
    // References GameManager to set the player type to ice
    // see above for playerType explanation
    public void IceOverride()
    {
        myAnim.runtimeAnimatorController = animationsOverrideIce;
        FindObjectOfType<GameManager>().SetPlayerType(2);
    }

    // Sets the override to fire animations--referenced in the SwordPickup script
    // References GameManager to set the player type to fire
    // see above for playerType explanation
    public void FireOverride()
    {
        myAnim.runtimeAnimatorController = animationsOverrideFire;
        FindObjectOfType<GameManager>().SetPlayerType(3);
    }

    // Sets the override to lightning animations--referenced in the SwordPickup script
    // References GameManager to set the player type to lightning
    // see above for playerType explanation
    public void LightningOverride()
    {
        myAnim.runtimeAnimatorController = animationsOverrideLightning;
        FindObjectOfType<GameManager>().SetPlayerType(4);
    }

    // Sets the override to crit animations--referenced in the SwordPickup script
    // References GameManager to set the player type to crit
    // see above for playerType explanation
    public void CritOverride()
    {
        myAnim.runtimeAnimatorController = animationsOverrideCrit;
        FindObjectOfType<GameManager>().SetPlayerType(0);
    }

    /*
    public void AxeOverride()
    {
        myAnim.runtimeAnimatorController = animationsOverrideAxe;
        FindObjectOfType<GameManager>().SetPlayerType(6);
    }
    */




}

