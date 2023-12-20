using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    //private TilemapCollider2D tilemapCollider;
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
        initCompositeCollider();
        lockOnPlayer();
    }

    // Update is called once per frame
    private void Update()
    {
        lockOnPlayer();
        float dirX = Input.GetAxisRaw("Horizontal");

        if(dirX != 0)
        {
            //bool isRunning = playerAnimator.GetBool("isRunning");
            //isRunning = true;
            playerAnimator.SetBool("isRunning", true);
        }
        else
        {
            playerAnimator.SetBool("isRunning", false);
        }

        float posX = rb.velocity.x;
        float posY = rb.velocity.y;

        rb.velocity = new Vector2(dirX * 7f, posY);

        // Debug.Log("box collider isTouching: " + boxCollider.IsTouching(compositeCollider));

        if (Input.GetButtonDown("Jump") && boxCollider.IsTouching(compositeCollider))
        {
            rb.velocity = new Vector2(posX, 16f);
        }
    }

    private void initCompositeCollider()
    {
        if (terrain != null)
        {
            compositeCollider = terrain.GetComponent<CompositeCollider2D>();
        }
    }

    private void lockOnPlayer()
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
