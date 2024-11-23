using System.Collections;
using System.Collections.Generic;
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

    private bool canShoot = true; 
    private float cooldownTime = 1f; 
    private float chargeTime = 0f; 
    private bool isCharging = false; 
    private int burstShotsRemaining = 0;

    void Update()
    {
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
        if (Input.GetMouseButton(0)) 
        {
            isCharging = true;
            chargeTime += Time.deltaTime;
            chargeSlider.gameObject.SetActive(true);
            chargeSlider.value = Mathf.Clamp01(chargeTime / 2f); 
        }

        if (Input.GetMouseButtonUp(0) && isCharging && canShoot) 
        {
            isCharging = false;
            chargeSlider.gameObject.SetActive(false);
            int finalDamage = Mathf.RoundToInt(Mathf.Lerp(10, 100, chargeSlider.value));
            float finalSpeed = Mathf.Lerp(10f, 50f, chargeSlider.value);

            Shoot(finalSpeed, finalDamage, bulletPrefab);
            ResetCooldown();
            chargeTime = 0f;
        }
    }

    void HandleMiddleAgesInput()
    {
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            Shoot(50f, 75, bulletPrefab); 
            ResetCooldown(4f); 
        }
    }
    
    void HandleSciFiInput()
    {
        if (canShoot && burstShotsRemaining == 0 && Input.GetMouseButtonDown(0)) 
        {
            burstShotsRemaining = 3;
            StartCoroutine(BurstFire());
            ResetCooldown();
        }
    }

    IEnumerator BurstFire()
    {
        while (burstShotsRemaining > 0)
        {
            Shoot(30f, 20, laserBulletPrefab); 
            burstShotsRemaining--;
            yield return new WaitForSeconds(0.2f); 
        }
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

    void ResetCooldown(float overrideTime = 1f)
    {
        canShoot = false;
        cooldownTime = overrideTime;
        StartCoroutine(CooldownRoutine());
    }

    IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(cooldownTime);
        canShoot = true;
    }
}
