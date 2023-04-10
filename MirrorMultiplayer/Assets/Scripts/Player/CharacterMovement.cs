using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.InputSystem.Editor;
#endif


[RequireComponent(typeof(Sentient))]
[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    //animation
    Animator animator;

    Sentient mSentient;

    Sentient.ControlledVector controlledVector;
    Sentient.ControlledVector controlledRotation;

    Sentient.Flipping FlippingCalls;

    [SerializeField]
    InputActionReference actionReferenceMovement;
    [SerializeField]
    InputActionReference actionReferenceAttack;
    [SerializeField]
    InputActionReference actionReferenceAttack2;


    // run
    public int runSpeed = 1;
    public float MoveSpeed;
    float horizontal;
    float vertical;
    bool facingRight;

    bool canMove;
    bool canAttack;

    [SerializeField]
    Vector3 Movement;

    Rigidbody rb;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        canMove = true;
        canAttack = true;
        mSentient = GetComponent<Sentient>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        controlledRotation = new Sentient.ControlledVector();
        mSentient.AddToAllRotationalControl(controlledRotation);
        mSentient.AddToAllRotationalControl(new Sentient.MaintainRotationTowardCameraY(gameObject));

        controlledVector = new Sentient.ControlledVector();
        controlledVector.Strength = 5;
        mSentient.AddToAllMovementControl(controlledVector);
        controlledVector.AddCalculator(new Sentient.VectorFilterCalculateClampToXYToXZ());
        controlledVector.AddCalculator(new Sentient.VectorFilterCalculateRelativeToCamera());
        controlledVector.AddCalculator(new Sentient.VectorFilterCalculateNormalize());
        //mSentient.AddToAllMovementControl(new Sentient.RigidbodyDifferentiation(GetComponent<Rigidbody>()));
    }

    void Update()
    {
        if (actionReferenceAttack.action.WasPressedThisFrame())
        {
            animator.SetTrigger("Attack1");
        }

        if (actionReferenceAttack2.action.WasPressedThisFrame())
        {
            animator.SetTrigger("Attack2");
        }
    }

    public void SetCanMoveFalse()
    {
        canMove = false;
    }

    public void SetCanMoveTrue()
    {
        canMove = true;
    }
    public void SetCanAttackFalse()
    {
        canAttack = false;
    }

    public void SetCanAttackTrue()
    {
        canAttack = true;
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            Vector3 _Movement = actionReferenceMovement.action.ReadValue<Vector2>();

            if (!(_Movement.x == 0) || !(_Movement.y == 0))
            {
                animator.SetBool("MovingForward", true);
            }
            else animator.SetBool("MovingForward", false);

            if (_Movement.x > 0) { controlledRotation.ChangeVector(new Vector3(0, 0, 0)); }
            else if (_Movement.x < 0) { controlledRotation.ChangeVector(new Vector3(0, 180, 0)); }
            /*
            Movement = Camera.main.transform.TransformVector(new Vector3(_Movement.x, 0, _Movement.y)); //Set movement to be relative to camera


            Movement = new Vector3(Movement.x, 0, Movement.z).normalized;
            */
            controlledVector.ChangeVector(Movement);

            transform.eulerAngles = mSentient.GetAllRotationalControlVector3();
            Vector3 _FullMove = mSentient.GetAllMovementControlVector3();
            rb.velocity = new Vector3(_FullMove.x, rb.velocity.y + _FullMove.y, _FullMove.z);
            //Flip(horizontal);
        }
    }
}
