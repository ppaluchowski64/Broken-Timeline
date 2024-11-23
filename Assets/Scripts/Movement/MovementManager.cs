using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private bool isGrounded;

    private float originalMoveSpeed;
    private Vector3 originalScale;
    private Queue<Vector3> positionHistory = new Queue<Vector3>();

    public Weapon weapon; // Reference to the Weapon script for reload time adjustments

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalMoveSpeed = moveSpeed;
        originalScale = transform.localScale;

        // Start tracking position history for "rewind" power-up
        StartCoroutine(TrackPositionHistory());
    }

    void Update()
    {
        Debug.Log(gameObject.name);
        if (gameObject.name == "player1")
        {
            HandleKeyboardMovement();
            HandleKeyboardJump();
        }
        else if (gameObject.name == "player2")
        {
            HandleControllerMovement();
            HandleControllerJump();
        }
    }

    private void HandleKeyboardMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void HandleControllerMovement()
    {
        float horizontalInput = Input.GetAxis("JoystickHorizontal");

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

    private void HandleKeyboardJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }
    }

    private void HandleControllerJump()
    {
        if (Input.GetKeyDown(KeyCode.Joystick1Button1) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PowerUpLayer"))
        {
            HandlePowerUp(collision.gameObject);
            Destroy(collision.gameObject); 
        }

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
    public bool isreloadbuffed = false;
    private void HandlePowerUp(GameObject powerUp)
{
    string powerUpType = powerUp.tag; // Identify the power-up type by its tag

    switch (powerUpType)
    {
        case "Speed":
            StartCoroutine(ApplySpeedBoost(2f, 5f)); // Speed boost 150% for 5 seconds
            break;

        case "Reload":
            if (weapon != null)
            {
                weapon.SetReloadMultiplier(0.2f, 2f); // Reduce reload time by 50% for 1 second
                isreloadbuffed = true;
                StartCoroutine(ResetReloadBuff(2f)); // Reset buff after 1 second
            }
            break;

        case "Size":
            StartCoroutine(ApplySizeChange(0.5f, 6f)); // Reduce size to 50% for 6 seconds
            break;

        case "Rewind":
            StartCoroutine(RewindPosition(4f)); // Rewind position to where the player was 4 seconds ago
            break;
    }
}

// Coroutine to reset the reload buff after duration
private IEnumerator ResetReloadBuff(float duration)
{
    yield return new WaitForSeconds(duration); // Wait for the buff duration
    isreloadbuffed = false; // Reset the reload buff
}

    private IEnumerator ApplySpeedBoost(float multiplier, float duration)
    {
        moveSpeed *= multiplier; // Increase speed
        yield return new WaitForSeconds(duration);
        moveSpeed = originalMoveSpeed; // Reset speed
    }

    private IEnumerator ApplySizeChange(float scaleMultiplier, float duration)
    {
        transform.localScale = originalScale * scaleMultiplier; // Change size
        yield return new WaitForSeconds(duration);
        transform.localScale = originalScale; // Reset size
    }

    private IEnumerator RewindPosition(float rewindTime)
    {
        // Wait for enough position history to accumulate
        yield return new WaitUntil(() => positionHistory.Count > rewindTime / 0.1f);

        Vector3 rewindPosition = positionHistory.Dequeue(); // Fetch the position from 4 seconds ago
        transform.position = rewindPosition;
    }

    private IEnumerator TrackPositionHistory()
    {
        while (true)
        {
            positionHistory.Enqueue(transform.position);

            // Keep the history length manageable
            if (positionHistory.Count > 50) // Stores positions for up to 5 seconds (50 * 0.1s)
            {
                positionHistory.Dequeue();
            }

            yield return new WaitForSeconds(0.1f); // Track position every 0.1 seconds
        }
    }
}
