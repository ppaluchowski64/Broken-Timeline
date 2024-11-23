using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float speed = 20f;
    public int damage = 40;
    public Rigidbody2D rb;
    public GameObject impactEffect;

    private float hitCooldown = 0.03f; // Time delay before the bullet can hit anything
    private float nextHitTime = 0f; // When the bullet can hit something next

    void Start () {
        rb.linearVelocity = transform.right * speed;
        nextHitTime = Time.time + hitCooldown; // Set the time when the bullet can start hitting things
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // Only allow hitting things after the cooldown has passed
        if (Time.time < nextHitTime)
            return;

        Enemy enemy = hitInfo.GetComponent<Enemy>();

        if (enemy != null)
        {
            Debug.Log("Bullet hit enemy: " + enemy.name);
            enemy.TakeDamage(damage);
            Destroy(gameObject); 
        }
        else if (hitInfo.CompareTag("Ground"))
        {
            Debug.Log("Bullet hit ground.");
            Destroy(gameObject); 
        }
        else
        {
            Debug.Log("Bullet hit something else: " + hitInfo.gameObject.name);
        }
    }
}
