using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;

/*!!!Notes: 
 * Script Version (1.0V)
 * By Chan Kwok Chun (Gul)
 * Will be reused in future projects because its perfect for copy and pasting
 * 
 * Update Log:
 * 1.0V 09/03/2023: - Refer to Intelligence component to see the base
 */

public class EnemyGoon1 : Intelligence
{
    Sentient.ControlledVector controlledRotation;
    Rigidbody rb;
    Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        ChangeStateTo(1);
    }
    public override void TurnOffIntelligence()
    {
        rb.velocity = new Vector3(0, 0, 0);
        enabled = false;
    }

    public void InitializeCommonState()
    {
        Brain.AddToAllRotationalControl(new Sentient.MaintainRotationTowardCameraY(gameObject));
    }

    public override void ChangeStateTo(int StateChoice)
    {
        switch (StateChoice)
        {
            case 1: //Initial
                ClearGarbageControls();

                InitializeCommonState();

                Sentient.ReturnClosestToFurthest returnClosestToFurthest1 = new Sentient.ReturnClosestToFurthest();
                Brain.AddTargetFilterList(returnClosestToFurthest1);
                Brain.AddToAllMovementControl(new Sentient.MoveTowardTarget(returnClosestToFurthest1, 5, 6, 3f));
               // Brain.AddToAllMovementControl(new Sentient.WanderingByTargetless(returnClosestToFurthest1, 300, 2, 4, 1, 3));
                Brain.AddToAllRotationalControl(new Sentient.FlipYRotTowardTargetRelativeToSelf(returnClosestToFurthest1));
                break;
            case 2: //After hitting 2times
                ClearGarbageControls();

                InitializeCommonState();

                Sentient.ReturnClosestToFurthest returnClosestToFurthest2 = new Sentient.ReturnClosestToFurthest();
                Brain.AddToAllRotationalControl(new Sentient.FlipYRotTowardTargetRelativeToSelf(returnClosestToFurthest2));



                break;
            case 3: //Dead
                ClearGarbageControls();

                InitializeCommonState();
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if ((Mathf.Abs(rb.velocity.x) > 0.5) || (Mathf.Abs(rb.velocity.y) > 0.5))
        {
            animator.SetBool("MovingForward", true);
        }
        else animator.SetBool("MovingForward", false);

        transform.eulerAngles = Brain.GetAllRotationalControlVector3();
    }

    private void FixedUpdate()
    {
        Brain.UpdateAllTargetFilters();
        Vector3 _FullMove = Brain.GetAllMovementControlVector3();

        //Debug.LogError(transform.eulerAngles);
        //Debug.LogError(_FullMove);

        rb.velocity = new Vector3(_FullMove.x, rb.velocity.y + _FullMove.y, _FullMove.z);
    }
}
