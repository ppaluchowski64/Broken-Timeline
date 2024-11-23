using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);


        if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}