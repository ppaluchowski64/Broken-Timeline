using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public Transform firePoint; 
    public GameObject bulletPrefab;
    public GameObject laserBulletPrefab; 
    public Slider chargeSlider; 
    public AgeManager ageManager; 
    public Camera mainCamera; 
    public MovementManager MovementManager;

    private bool canShoot = true; 
    public float reloadTime; // The current reload time, based on the age
    private float chargeTime = 0f; 
    private bool isCharging = false; 
    private int burstShotsRemaining = 0;
    public float reloadTimeMultiplier = 1f; // Default multiplier is 1

    private Coroutine reloadMultiplierCoroutine; // To ensure only one active multiplier coroutine

    void Start()
{
    SetReloadTimeBasedOnAge(); // Set the initial reload time based on the current age
}

void Update()
{
     if (MovementManager.isreloadbuffed == false)
    {
        reloadTimeMultiplier = 1f; 
    }
    SetReloadTimeBasedOnAge(); // Check and update the reload time every frame in case the age changes dynamically
    float adjustedReloadTime = reloadTime * reloadTimeMultiplier;
    AimTowardsCursor();

    if (ageManager.Prehistory)
    {
        HandlePrehistoryInput();
    }
    else if (ageManager.Middle_Ages)
    {
        HandleMiddleAgesInput();
    }
    else if (ageManager.SCI_FI)
    {
        HandleSciFiInput();
    }
}

    void AimTowardsCursor()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0f; 

        Vector3 worldMousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
        worldMousePosition.z = 0f; 

        Vector3 aimDirection = (worldMousePosition - transform.position).normalized;

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
    }

    void HandlePrehistoryInput()
{
    if (canShoot) // Only allow charging when the weapon is ready to shoot
    {
        if (Input.GetMouseButton(0)) 
        {
            isCharging = true;
            chargeTime += Time.deltaTime;
            chargeSlider.gameObject.SetActive(true);
            chargeSlider.value = Mathf.Clamp01(chargeTime / 2f); // Charge for a maximum of 2 seconds
        }

        if (Input.GetMouseButtonUp(0) && isCharging) 
        {
            isCharging = false;
            chargeSlider.gameObject.SetActive(false);
            int finalDamage = Mathf.RoundToInt(Mathf.Lerp(10, 100, chargeSlider.value)); // Damage based on charge
            float finalSpeed = Mathf.Lerp(10f, 50f, chargeSlider.value); // Speed based on charge

            Shoot(finalSpeed, finalDamage, bulletPrefab);
            ResetCooldown();
            chargeTime = 0f;
        }
    }
}


    void HandleMiddleAgesInput()
    {
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            Shoot(50f, 75, bulletPrefab); // Crossbow
            ResetCooldown();
        }
    }

void HandleSciFiInput()
{
    if (canShoot && burstShotsRemaining == 0 && Input.GetMouseButtonDown(0)) 
    {
        burstShotsRemaining = 3; // Start a burst of 3 shots
        StartCoroutine(BurstFire()); // Start the burst fire sequence
        canShoot = false; // Prevent firing until the burst and cooldown are finished
    }
}
    IEnumerator BurstFire()
{
    while (burstShotsRemaining > 0) 
    {
        Shoot(30f, 20, laserBulletPrefab); // Fire a shot
        burstShotsRemaining--; // Decrease the remaining shots in the burst
        yield return new WaitForSeconds(0.2f); // Wait between shots in the burst
    }

    // After the burst is finished, reset cooldown and allow the next shot
    ResetCooldown();
}

    void Shoot(float speed, int damage, GameObject prefab)
    {
        GameObject bullet = Instantiate(prefab, firePoint.position, firePoint.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.speed = speed;
            bulletScript.damage = damage;
        }
    }

 public void SetReloadMultiplier(float multiplier, float duration)
    {
        // Stop any existing multiplier coroutine
        if (reloadMultiplierCoroutine != null)
        {
            StopCoroutine(reloadMultiplierCoroutine);
        }

        // Set the reloadTimeMultiplier and start a coroutine to reset it after duration
        reloadTimeMultiplier = multiplier;
        reloadMultiplierCoroutine = StartCoroutine(ResetReloadMultiplier(duration));
    }
    private IEnumerator ResetReloadMultiplier(float duration)
    {
        yield return new WaitForSeconds(duration);
        reloadTimeMultiplier = 1f;  // Reset the multiplier after duration
        reloadMultiplierCoroutine = null; // Clear the coroutine reference
    }

private IEnumerator ApplyReloadMultiplier(float multiplier, float duration)
{
    float originalReloadTime = reloadTime; // Save the original reload time
    reloadTime *= multiplier; // Apply the multiplier
    Debug.Log("New Reload Time: " + reloadTime);  // Debug line to check multiplier effect

    // Wait for the duration of the multiplier effect
    yield return new WaitForSeconds(duration);

    reloadTime = originalReloadTime; // Restore the original reload time
    Debug.Log("Reload Time Restored: " + reloadTime);  // Debug line to check reload reset
    reloadMultiplierCoroutine = null; // Clear the coroutine reference
}


// Reset cooldown after shooting
void ResetCooldown()
{
    canShoot = false;
    StopAllCoroutines(); // Stop any currently running cooldown coroutines
    StartCoroutine(CooldownRoutine());
}

IEnumerator CooldownRoutine()
    {
        float timer = 0f;
        while (timer < reloadTime * reloadTimeMultiplier) // Use adjusted reload time
        {
            timer += Time.deltaTime;
            yield return null;
        }

        canShoot = true;  // Allow shooting again after cooldown
    }

    // Set the reload time based on the current age
     void SetReloadTimeBasedOnAge()
    {
        // Only set reloadTime based on age once
        if (ageManager.Prehistory)
        {
            reloadTime = 2f; // Prehistory reload time
        }
        else if (ageManager.Middle_Ages)
        {
            reloadTime = 4f; // Middle Ages reload time
        }
        else if (ageManager.SCI_FI)
        {
            reloadTime = 1f; // Sci-Fi reload time
        }
    }
}
