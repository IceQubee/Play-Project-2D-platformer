using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float BulletSpeed;
    private Rigidbody2D BulletRB;

    void Start()
    {
        BulletRB = GetComponent<Rigidbody2D>();
        BulletRB.velocity = transform.right * BulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy =  collision.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(20);
        }
        Destroy(gameObject);
    }
}
