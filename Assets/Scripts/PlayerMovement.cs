using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    //TODO: Fix Wall detection in IsPlayerOnJumpableSurface. It might make more sense to check it not after player pressed "W", but when player pressed "A" or "D".
    private Rigidbody2D rb;

    private BoxCollider2D boxCollider;

    private CompositeCollider2D compositeCollider;

    private SpriteRenderer spriteRenderer;

    private Animator playerAnimator;

    private GameObject camera;

    private GameObject terrain;

    private int isRunningHash;
    private int isJumpingHash;
    private int isFallingHash;

    private int numJumps = 2;
    private float dirX = 0f;
    private float dirY = 0f;

    [SerializeField]
    private float smoothingFactor;

    [SerializeField]
    private LayerMask jumpableSurface;


    [SerializeField]
    private LayerMask wallLayer;

    [SerializeField]
    private float jumpForce = 16f;

    [SerializeField]
    private float movementSpeed = 7f;

    private enum MovementState { Idle, Running, Jumping, DoubleJumping, Falling }
    private MovementState movementState = MovementState.Idle;

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("Hello World!");
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //tilemapCollider = GetComponent<TilemapCollider2D>();
        terrain = GameObject.Find("Terrain");
        camera = GameObject.Find("Main Camera");
        //jumpableSurface = LayerMask.NameToLayer(LayerMask.LayerToName(terrain.layer));

        int isRunningHash = Animator.StringToHash("isRunning");
        int isJumpingHash = Animator.StringToHash("isJumping");
        int isFallingHash = Animator.StringToHash("isFalling");

        InitCompositeCollider();
        LockOnPlayer();
    }

    // Update is called once per frame
    private void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        dirY = Input.GetAxisRaw("Vertical");

        UpdatePlayerMovement();
        UpdateAnimationState();
    }

    private void FixedUpdate()
    {
        LockOnPlayer();
    }

    private void HandlePlayerInpts()
    {

    }

    private void UpdatePlayerMovement()
    {
        // Cache the parameter IDs using Animator.StringToHash

        float velY = rb.velocity.y;

        rb.velocity = new Vector2(dirX * movementSpeed, velY);

        float velX = rb.velocity.x;

        if (velX < -.1f)
        {
            SpriteRendererFlipX(true);
        }
        else
        {
            SpriteRendererFlipX(false);
        }


        if (Input.GetButtonDown("Jump") && CanJump())
        {
            rb.velocity = new Vector2(velX, dirY * jumpForce);
            numJumps--;
        }

    }

    private void InitCompositeCollider()
    {
        if (terrain != null)
        {
            compositeCollider = terrain.GetComponent<CompositeCollider2D>();
        }
    }

    private void Jump()
    {

    }

    private bool CanJump()
    {
        return IsPlayerOnJumpableSurface() || (numJumps > 0) || (playerAnimator.GetInteger("movementState") == 4);
    }

    private bool IsPlayerOnJumpableSurface()
    {
        //RaycastHit2D has an implicit bool operator implemented. That's why we can directly use it as a boolean.
        bool isPlayerOnGround = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, .1f, jumpableSurface);
        bool isPlayerOnLeftWall = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.left, 1f, jumpableSurface);
        bool isPlayerOnRightWall = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.right, 1f, jumpableSurface);

        bool isPlayerOnJumpableSurface = isPlayerOnGround || isPlayerOnLeftWall || isPlayerOnRightWall;

        Debug.Log("isPlayerOnJumpableSurface" + isPlayerOnJumpableSurface);
        Debug.Log("isPlayerOnGround: " + isPlayerOnGround);
        Debug.Log("isPlayerOnLeftWall: " + isPlayerOnLeftWall);
        Debug.Log("isPlayerOnRightWall: " + isPlayerOnRightWall);

        if (isPlayerOnJumpableSurface)
        {
            numJumps = 2;
        }

        return isPlayerOnJumpableSurface;
    }

    private void SpriteRendererFlipX(bool flipX)
    {
        spriteRenderer.flipX = flipX;
    }

    private void LockOnPlayer()
    {
        if (camera == null)
        {
            return;
        }

        Transform camTransform = camera.transform;
        Vector3 playerPos = transform.position; // Get the player's position
        Vector3 cameraPos = camera.transform.position;

        // Keep the camera's original z position
        float camZ = camTransform.position.z;

        // Lerp
        Vector3 newPos = Vector3.Lerp(cameraPos, playerPos, smoothingFactor * Time.deltaTime);
        newPos.z = camZ;


        // Setting the position of the camera to the new position
        camTransform.position = newPos;

    }

    private void UpdateAnimationState()
    {
        MovementState state = MovementState.Idle;

        if (dirX > 0f)
        {
            state = MovementState.Running;
        }
        else if (dirX < 0f)
        {
            state = MovementState.Running;
        }
        else
        {
            state = MovementState.Idle;
        }

        if (rb.velocity.y > .1f)
        {
            state = numJumps == 0 ? MovementState.DoubleJumping : MovementState.Jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.Falling;
        }

        playerAnimator.SetInteger("movementState", (int)state);
    }
}
