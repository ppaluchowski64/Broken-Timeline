using UnityEngine;
using UnityEngine.UI; // For the UI loading bar

public class SlingshotShooter : MonoBehaviour
{
    public Texture2D projectileTexture; // Texture of the particle (set in Inspector)
    public Transform firePoint; // Point where the projectile spawns
    public GameObject projectilePrefab; // Prefab containing a sprite renderer
    public Slider strengthBar; // UI slider to show strength
    public float maxStrength = 50f; // Maximum strength of the shot (increase for bullet-like speed)
    public float chargeSpeed = 30f; // Speed of strength increase

    private float currentStrength = 0f;
    private bool isCharging = false;

    void Update()
    {
        HandleInput();
        UpdateStrengthBar();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButton(0)) // Left mouse button held
        {
            isCharging = true;
            currentStrength += chargeSpeed * Time.deltaTime;
            currentStrength = Mathf.Clamp(currentStrength, 0, maxStrength);
        }

        if (Input.GetMouseButtonUp(0)) // Left mouse button released
        {
            isCharging = false;
            ShootProjectile();
            currentStrength = 0; // Reset strength
        }
    }

    private void UpdateStrengthBar()
    {
        if (strengthBar != null)
        {
            strengthBar.gameObject.SetActive(isCharging); // Show bar only while charging
            strengthBar.value = currentStrength / maxStrength; // Update bar value
        }
    }

    private void ShootProjectile()
    {
        if (projectileTexture == null || firePoint == null || projectilePrefab == null)
        {
            Debug.LogWarning("Projectile Texture, Prefab, or Fire Point is not assigned!");
            return;
        }

        // Get the mouse position in world coordinates
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure the z-axis is at 0 for 2D space

        // Calculate the direction from the fire point to the mouse position
        Vector2 shootDirection = (mousePosition - firePoint.position).normalized;

        // Create the projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // Assign the texture to the projectile's SpriteRenderer
        SpriteRenderer spriteRenderer = projectile.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = Sprite.Create(projectileTexture,
                new Rect(0, 0, projectileTexture.width, projectileTexture.height),
                new Vector2(0.5f, 0.5f));
        }

        // Set the projectile's velocity based on the charge strength
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0; // Ensure no gravity for bullet-like behavior
            rb.linearVelocity = shootDirection * Mathf.Max(currentStrength, maxStrength / 2); // Ensure a minimum strength
        }

        // Rotate the projectile to face the direction it is traveling
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Destroy projectile on collision
        ProjectileDestroyer destroyer = projectile.AddComponent<ProjectileDestroyer>();
        destroyer.Setup();
    }
}

// Additional script for projectile destruction
public class ProjectileDestroyer : MonoBehaviour
{
    public void Setup()
    {
        // Add this to detect collision
        gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject); // Destroy projectile on collision
    }
}
