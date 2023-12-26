using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    //TODO: Add Layers to ground and walls, use LayerMasks to detect collision and add double jump. numJumps will be resetted to 2, when the player is on ground

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

    [SerializeField]
    private float smoothingFactor;

    [SerializeField]
    private LayerMask jumpableSurface;

    [SerializeField]
    private LayerMask wallLayer;

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

        UpdatePlayerMovement();
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

        float posX = rb.velocity.x;
        float posY = rb.velocity.y;

        if (dirX != 0)
        {
            if (dirX < 0)
            {
                SpriteRendererFlipX(true);
            }
            else
            {
                SpriteRendererFlipX(false);
            }
            playerAnimator.SetBool("isRunning", true);
        }
        else
        {
            playerAnimator.SetBool("isRunning", false);
        }

        rb.velocity = new Vector2(dirX * 7f, posY);

        if (Input.GetButtonDown("Jump") && CanJump())
        {
            rb.velocity = new Vector2(posX, 16f);
            numJumps--;
        }

        if (rb.velocity.y > 1)
        {
            playerAnimator.SetBool("isJumping", true);
        }
        else if (rb.velocity.y < -1)
        {
            playerAnimator.SetBool("isJumping", false);
            playerAnimator.SetBool("isFalling", true);
        }
        else if (rb.velocity.y > -1 && rb.velocity.y < 1)
        {
            playerAnimator.SetBool("isFalling", false);
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
        return (IsPlayerOnJumpableSurface() || playerAnimator.GetBool("isFalling")) && (numJumps > 0);
    }

    private bool IsPlayerOnJumpableSurface()
    {
        //RaycastHit2D has an implicit bool operator implemented. That's why we can directly use it as a boolean.
        bool isPlayerOnGround = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, .1f, jumpableSurface);
        bool isPlayerOnLeftWall = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, new Vector2(-1, 0), .1f, jumpableSurface);
        bool isPlayerOnRightWall = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, new Vector2(1, 0), .1f, jumpableSurface);

        bool isPlayerOnJumpableSurface = isPlayerOnGround || isPlayerOnLeftWall || isPlayerOnRightWall;

        Debug.Log("isPlayerOnJumpableSurface" + isPlayerOnJumpableSurface);
        Debug.Log("isPlayerOnGround: " + isPlayerOnGround);
        Debug.Log("isPlayerOnLeftWall: " + isPlayerOnLeftWall);
        Debug.Log("isPlayerOnRightWall: " + isPlayerOnRightWall);

        if(isPlayerOnJumpableSurface)
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
        // TODO: Smooth the camera movement using Vector.Lerp
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
}
