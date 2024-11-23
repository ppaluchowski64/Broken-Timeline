using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float speed = 20f;
    public int damage = 40;
    public Rigidbody2D rb;
    public GameObject impactEffect;

    void Start () {
        rb.linearVelocity = transform.right * speed; 
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {

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
