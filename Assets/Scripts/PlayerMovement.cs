using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    private BoxCollider2D boxCollider;
    
    private CompositeCollider2D compositeCollider;

    private Animator playerAnimator;
    
    private GameObject camera;
    
    private GameObject terrain;

    [SerializeField]
    private float smoothingFactor;

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("Hello World!");
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerAnimator = GetComponent<Animator>();
        //tilemapCollider = GetComponent<TilemapCollider2D>();
        terrain = GameObject.Find("Terrain");
        camera = GameObject.Find("Main Camera");
        InitCompositeCollider();
        LockOnPlayer();
    }

    // Update is called once per frame
    private void Update()
    {
        float dirX = Input.GetAxisRaw("Horizontal");

        UpdatePlayerMovement(dirX);
        Debug.Log("rb.velocity: " + rb.velocity.y);
    }

    private void FixedUpdate()
    {
        LockOnPlayer();
    }

    private void UpdatePlayerMovement(float dirX)
    {

        if (dirX != 0)
        {
            playerAnimator.SetBool("isRunning", true);
        }
        else
        {
            playerAnimator.SetBool("isRunning", false);
        }

        float posX = rb.velocity.x;
        float posY = rb.velocity.y;

        rb.velocity = new Vector2(dirX * 7f, posY);

        if (Input.GetButtonDown("Jump") && boxCollider.IsTouching(compositeCollider))
        {
            rb.velocity = new Vector2(posX, 16f);
        }
        
        if(rb.velocity.y > 1)
        {
            playerAnimator.SetBool("isJumping", true);
        }
        else if(rb.velocity.y < -1)
        {
            playerAnimator.SetBool("isJumping", false);
            playerAnimator.SetBool("isFalling", true);
        }
        else if(rb.velocity.y > -1 && rb.velocity.y < 1)
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
