using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float airControlSpeed = 3f;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    [SerializeField] private LayerMask jumpableGround;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private PlayerController playerController;
    private BoxCollider2D coll;

    private Vector2 moveInput;
    private bool isCrouching = false;
    private bool grounded;

    private enum MovementState
    {
        Idle = 0,
        Jump = 1,
        Fall = 2,
        Run = 3,
        Climb = 4,
        Hurt = 5,
        Crouch = 6
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        playerController = new PlayerController();
    }

    private void OnEnable()
    {
        playerController.Enable();

        playerController.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerController.Movement.Move.canceled += ctx => moveInput = Vector2.zero;

        playerController.Movement.Jump.performed += ctx => Jump();
        playerController.Movement.Croach.performed += ctx => isCrouching = true;
        playerController.Movement.Croach.canceled += ctx => isCrouching = false;
    }

    private void OnDisable()
    {
        playerController.Disable();
    }

    private void Update()
    {
        moveInput = playerController.Movement.Move.ReadValue<Vector2>();
        grounded = IsGrounded();

        // Gravity tuning
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Keyboard.current.spaceKey.isPressed)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        float currentSpeed = grounded ? moveSpeed : airControlSpeed;
        rb.velocity = new Vector2(moveInput.x * currentSpeed, rb.velocity.y);
    }

    private void Jump()
    {
        if (grounded && !isCrouching)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, jumpableGround);
    }

    private void UpdateAnimation()
    {
        MovementState state;

        if (isCrouching && grounded)
        {
            state = MovementState.Crouch;
        }
        else if (!grounded && rb.velocity.y > 0.1f)
        {
            state = MovementState.Jump;
        }
        else if (!grounded && rb.velocity.y < -0.1f)
        {
            state = MovementState.Fall;
        }
        else if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            state = MovementState.Run;
        }
        else
        {
            state = MovementState.Idle;
        }

        // Flip sprite sesuai arah
        if (moveInput.x != 0)
            sprite.flipX = moveInput.x < 0;

        anim.SetInteger("state", (int)state);
    }
}
