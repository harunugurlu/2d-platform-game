using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("Hello World!");
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {

        rb.gravityScale = 5.0f;

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && rb.velocity.y <= 0)
        {
            rb.velocity = new Vector2(0, 16f);
        }
        if (Input.GetKey(KeyCode.A))
        { 
            rb.velocity = new Vector2(-8.0f, 4.0f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.velocity = new Vector2(8.0f, 4.0f);
        }
    }
}
